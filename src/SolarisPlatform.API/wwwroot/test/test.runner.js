// ══════════════════════════════════════════════════════════════════════
//  test.runner.js  —  Solaris Platform · API Test Console  v3.3
//  ─────────────────────────────────────────────────────────────────────
//  Motor de ejecución de tests.
//  - No manipula el DOM.
//  - Se comunica con el exterior mediante callbacks/eventos.
//  - Depende de: test.config.js  (TEST_SUITES debe estar cargado antes)
//
//  FIXES v3.1:
//   [1] Implementado DYNAMIC: diccionario que persiste IDs capturados
//       (captureId) y los inyecta en paths/bodies (useDynamic, bodyFn).
//   [2] Implementado skipIf: omite tests si el ID requerido no existe.
//   [3] Soporte para paths con dos IDs dinámicos: {DYN} + {secondaryKey}.
//   [4] Eliminada la duplicación de getBaseUrl.
//   [5] captureId ahora captura data.id | data.data.id | data.data[0].id
//
//  FIXES v3.2:
//   [BUG-2] _baseUrl se resuelve SIEMPRE desde getBaseUrl() en el momento
//           de ejecutar, no solo al inicio de runAll/runSuite.
//
//  FIXES v3.3:
//   [DIAG-1] Al fallar un test, se captura snapshot de DYNAMIC en ese
//            momento exacto → permite ver qué IDs estaban disponibles.
//   [DIAG-2] Se captura responseRaw (texto completo sin parsear) para
//            casos donde res.json() falla o trunca información.
//   [DIAG-3] Se expone debug.innerException del backend cuando está
//            disponible en la respuesta (modo Development del middleware).
//   [DIAG-4] _buildDiagnosis enriquecido con hints específicos de EF Core
//            (DateTime Kind=Unspecified, FK violations, unique constraints).
// ══════════════════════════════════════════════════════════════════════

const TestRunner = (() => {

  // ── Estado interno ──────────────────────────────────────────────
  let _jwt          = '';
  let _refreshToken = '';

  function _resolveBaseUrl() {
    return _runner.getBaseUrl().replace(/\/$/, '');
  }

  // [FIX 1] Diccionario de IDs capturados en tiempo de ejecución
  const DYNAMIC = {};

  // Resultados guardados por testId para que test.report.js los consulte
  const _results = {};

  // Callbacks que el UI registra para recibir notificaciones
  const _cb = {
    onTestStart:    () => {},   // (testId)
    onTestEnd:      () => {},   // (testId, result)
    onSuiteEnd:     () => {},   // (suiteId)
    onRunComplete:  () => {},   // (summary)
    onTokenCapture: () => {},   // (token, truncated)
  };

  // ── API HTTP helper ─────────────────────────────────────────────
  async function _fetch(method, path, body, token) {
    const baseUrl = _resolveBaseUrl();
    const headers = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = 'Bearer ' + token;

    const t0 = performance.now();
    try {
      const res = await fetch(baseUrl + path, {
        method,
        headers,
        body: body ? JSON.stringify(body) : undefined
      });
      const ms = Math.round(performance.now() - t0);

      // [DIAG-2] Capturar texto raw antes de parsear
      const rawText = await res.text();
      let data = null;
      try { data = JSON.parse(rawText); } catch { data = null; }

      return { status: res.status, data, rawText, ms, baseUrl };
    } catch (e) {
      const ms = Math.round(performance.now() - t0);
      return {
        status: 'ERR',
        data: { error: e.message, hint: 'No se pudo conectar. Verifica que el servidor esté corriendo y la Base URL sea correcta.' },
        rawText: e.message,
        ms,
        baseUrl
      };
    }
  }

  // ── Check de expectativas ───────────────────────────────────────
  function _checkExpect(expect, status, data) {
    const expectedStatuses = Array.isArray(expect.status) ? expect.status : [expect.status];
    if (!expectedStatuses.includes(status)) return false;
    if (expect.check) return !!expect.check(data);
    return true;
  }

  // ── [DIAG-4] Diagnóstico enriquecido con hints de EF Core ──────
  function _buildDiagnosis(testDef, res, dynamicSnapshot) {
    const expected = Array.isArray(testDef.expect.status) ? testDef.expect.status : [testDef.expect.status];
    const statusOk = expected.includes(res.status);
    const lines    = [];

    if (!statusOk) {
      lines.push(`HTTP status incorrecto: esperado [${expected.join(' | ')}], recibido ${res.status}.`);

      if (res.status === 'ERR' || res.status === 0) {
        lines.push('La API no respondió. Verifica que el servidor esté corriendo y la Base URL sea correcta.');
      } else if (res.status === 500) {
        lines.push('Error 500 en el servidor. Revisar debug.innerException en esta respuesta.');

        // [DIAG-4] Hints específicos de EF Core / PostgreSQL basados en el mensaje
        const msg = JSON.stringify(res.data || '').toLowerCase();
        if (msg.includes('kind=unspecified') || msg.includes('timestamp with time zone')) {
          lines.push('⚠️  EF Core / Npgsql: DateTime con Kind=Unspecified. Las fechas deben enviarse con zona UTC (formato ISO 8601 con Z) o el servicio debe usar DateTime.SpecifyKind(..., DateTimeKind.Utc).');
        }
        if (msg.includes('foreign key') || msg.includes('violates foreign key')) {
          // Extraer nombre de tabla si es posible
          const tableMatch = JSON.stringify(res.data || '').match(/table "([^"]+)"/);
          const fkMatch    = JSON.stringify(res.data || '').match(/constraint "([^"]+)"/);
          lines.push(`⚠️  FK violation: tabla ${tableMatch?.[1] || '?'}, constraint ${fkMatch?.[1] || '?'}. El ID referenciado no existe en la tabla padre.`);
        }
        if (msg.includes('unique') || msg.includes('duplicate key')) {
          const uniqMatch = JSON.stringify(res.data || '').match(/constraint "([^"]+)"/);
          lines.push(`⚠️  Unique constraint violado: ${uniqMatch?.[1] || 'desconocido'}. Ya existe un registro con esos valores únicos.`);
        }
        if (msg.includes('not-null') || msg.includes('null value in column')) {
          const colMatch = JSON.stringify(res.data || '').match(/column "([^"]+)"/);
          lines.push(`⚠️  Columna NOT NULL violada: ${colMatch?.[1] || 'desconocida'}. Revisar que el campo se envíe en el body o tenga valor por defecto.`);
        }
        if (msg.includes('automapper') || msg.includes('mapping')) {
          lines.push('⚠️  Error de AutoMapper. Revisar que el MappingProfile tenga configuración para este DTO. Posiblemente falta ConstructUsing() para un record posicional.');
        }
      } else if (res.status === 401 && testDef.auth) {
        lines.push('401 con auth=true: el JWT puede haber expirado o no se capturó en el login.');
      } else if (res.status === 404 && !testDef.path.includes('99999')) {
        lines.push('404 inesperado: la ruta puede ser incorrecta o el controller no está registrado en Program.cs.');
      } else if (res.status === 403) {
        lines.push('403 Forbidden: el usuario autenticado no tiene el rol requerido para este endpoint.');
      } else if (res.status === 400) {
        lines.push('400 Bad Request: error de validación o deserialización del body.');
      }
    } else if (testDef.expect.check) {
      lines.push(`HTTP ${res.status} correcto, pero el check de contenido falló: "${testDef.expect.checkDesc}".`);
    }

    if (testDef.hint)        lines.push(`Pista: ${testDef.hint}`);
    if (res.data?.errors)    lines.push(`Errores de validación: ${JSON.stringify(res.data.errors)}`);
    if (res.data?.message)   lines.push(`Mensaje del servidor: ${res.data.message}`);
    if (res.data?.error)     lines.push(`Error del servidor: ${res.data.error}`);
    if (res.data?.title)     lines.push(`Título del error (ProblemDetails): ${res.data.title}`);

    // [DIAG-1] Mostrar qué IDs dinámicos había en el momento del fallo
    const dynKeys = Object.keys(dynamicSnapshot);
    if (dynKeys.length > 0) {
      lines.push(`DYNAMIC snapshot al fallar: ${JSON.stringify(dynamicSnapshot)}`);
    } else {
      lines.push('DYNAMIC snapshot: vacío (ningún ID dinámico capturado aún).');
    }

    return lines;
  }

  // ── [FIX 3] Resolver paths con uno o dos IDs dinámicos ─────────
  function _resolvePath(rawPath, testDef) {
    let path = rawPath;
    if (testDef.useDynamic && path.includes('{DYN}')) {
      const val = DYNAMIC[testDef.useDynamic];
      path = path.replace('{DYN}', val !== undefined ? val : 'MISSING_ID');
    }
    path = path.replace(/\{([^}]+)\}/g, (match, key) => {
      const val = DYNAMIC[key];
      return val !== undefined ? val : match;
    });
    return path;
  }

  // ── Ejecutar un test individual ─────────────────────────────────
  async function _runTest(testDef, suiteId) {

    if (testDef.skipIf && !DYNAMIC[testDef.skipIf]) {
      const baseUrl = _resolveBaseUrl();
      const result = {
        suiteId,
        testId:         testDef.id,
        passed:         true,
        skipped:        true,
        status:         'SKIP',
        ms:             0,
        data:           { skipped: true, reason: `ID dinámico "${testDef.skipIf}" no disponible.` },
        diagnosis:      null,
        dynamicSnapshot: { ...DYNAMIC },
        method:         testDef.method,
        path:           testDef.path,
        fullUrl:        baseUrl + testDef.path,
        requiresAuth:   testDef.auth,
        expectedStatus: testDef.expect.status,
        checkDesc:      testDef.expect.checkDesc || null,
        hint:           testDef.hint || null,
        name:           testDef.name,
      };
      _cb.onTestStart(testDef.id);
      _results[testDef.id] = result;
      _cb.onTestEnd(testDef.id, result);
      return result;
    }

    _cb.onTestStart(testDef.id);

    const resolvedPath = _resolvePath(testDef.path, testDef);

    let body  = testDef.body  ?? null;
    let token = testDef.auth  ? _jwt : undefined;

    if (body === '__USE_CREDS__') {
      const creds = _runner.getCredentials();
      body = { email: creds.email, password: creds.password };
    }
    if (body === '__USE_REFRESH__') {
      body = { Token: _jwt, RefreshToken: _refreshToken };
    }
    if (testDef._fakeToken) {
      token = testDef._fakeToken;
    }
    if (testDef.bodyFn && BODY_FACTORIES[testDef.bodyFn]) {
      body = BODY_FACTORIES[testDef.bodyFn](DYNAMIC, SEED);
    }

    // [DIAG-1] Snapshot de DYNAMIC justo antes de ejecutar
    const dynamicSnapshot = { ...DYNAMIC };

    const res = await _fetch(testDef.method, resolvedPath, body, token);

    // Capturar JWT del login-ok
    if (testDef.id === 'login-ok' && res.status === 200) {
      const t  = res.data?.data?.token || res.data?.token || res.data?.accessToken;
      const rt = res.data?.data?.refreshToken || res.data?.refreshToken;
      if (t)  { _jwt = t;  _cb.onTokenCapture(t, t.substring(0, 70) + '…'); }
      if (rt) { _refreshToken = rt; }
    }

    // Capturar JWT renovado del refresh
    if (testDef.id === 'refresh' && res.status === 200) {
      const nt = res.data?.data?.token || res.data?.data?.accessToken || res.data?.token || res.data?.accessToken;
      if (nt) { _jwt = nt; _cb.onTokenCapture(nt, nt.substring(0, 70) + '…'); }
    }

    // Capturar ID de la respuesta en DYNAMIC
    if (testDef.captureId) {
      const capturedId =
        res.data?.data?.id   ??
        res.data?.id         ??
        (Array.isArray(res.data?.data) && res.data.data[0]?.id) ??
        null;
      if (capturedId !== null) {
        DYNAMIC[testDef.captureId] = capturedId;
      }
    }

    const passed    = _checkExpect(testDef.expect, res.status, res.data);
    const diagnosis = passed ? null : _buildDiagnosis(testDef, res, dynamicSnapshot);

    // Sanitizar token en la respuesta mostrada
    let displayData = res.data;
    if (res.data?.data?.token) {
      displayData = JSON.parse(JSON.stringify(res.data));
      displayData.data.token = displayData.data.token.substring(0, 50) + '…[truncado]';
    }

    // Sanitizar contraseña del body enviado
    let displayBody = body;
    if (body?.password) {
      displayBody = { ...body, password: '***' };
    }

    // [DIAG-3] Extraer innerException del backend si está presente
    const innerException = res.data?.debug?.innerException ?? null;
    const exceptionType  = res.data?.debug?.exceptionType  ?? null;
    const stackTrace     = res.data?.debug?.stackTrace     ?? null;

    const result = {
      suiteId,
      testId:         testDef.id,
      passed,
      skipped:        false,
      status:         res.status,
      ms:             res.ms,
      data:           displayData,
      requestBody:    displayBody,
      diagnosis,
      // ── [DIAG-1] Estado de DYNAMIC al ejecutar este test ──────
      dynamicSnapshot,
      // ── [DIAG-3] Info de excepción del backend (solo en dev) ──
      ...(passed ? {} : {
        serverDebug: {
          exceptionType,
          innerException,
          stackTrace,
        }
      }),
      method:         testDef.method,
      path:           resolvedPath,
      fullUrl:        res.baseUrl + resolvedPath,
      requiresAuth:   testDef.auth,
      expectedStatus: testDef.expect.status,
      checkDesc:      testDef.expect.checkDesc || null,
      hint:           testDef.hint || null,
      name:           testDef.name,
    };

    _results[testDef.id] = result;
    _cb.onTestEnd(testDef.id, result);
    return result;
  }

  // ── Ejecutar una suite ──────────────────────────────────────────
  async function _runSuite(suite) {
    for (const testDef of suite.tests) {
      await _runTest(testDef, suite.id);
      await _delay(280);
    }
    _cb.onSuiteEnd(suite.id);
  }

  // ── Utils ───────────────────────────────────────────────────────
  function _delay(ms) { return new Promise(r => setTimeout(r, ms)); }

  function _countResults() {
    const all     = Object.values(_results).filter(r => !r.skipped);
    const total   = all.length;
    const passed  = all.filter(r => r.passed).length;
    const failed  = total - passed;
    const skipped = Object.values(_results).filter(r => r.skipped).length;
    return { total, passed, failed, skipped };
  }

  // ── API pública ─────────────────────────────────────────────────
  const _runner = {

    getCredentials: () => ({ email: '', password: '' }),

    on(event, fn) { if (_cb[event] !== undefined) _cb[event] = fn; return this; },

    setBaseUrl(url) {
      this.getBaseUrl = () => url;
      return this;
    },

    reset() {
      _jwt = ''; _refreshToken = '';
      Object.keys(_results).forEach(k => delete _results[k]);
      Object.keys(DYNAMIC).forEach(k => delete DYNAMIC[k]);
    },

    getResults()  { return { ..._results }; },
    getSuites()   { return TEST_SUITES; },
    getDynamic()  { return { ...DYNAMIC }; },

    async runSuite(suiteId) {
      const suite = TEST_SUITES.find(s => s.id === suiteId);
      if (!suite) return;
      await _runSuite(suite);
      const summary = _countResults();
      _cb.onRunComplete(summary);
    },

    async runAll() {
      for (const suite of TEST_SUITES) {
        await _runSuite(suite);
        await _delay(350);
      }
      const summary = _countResults();
      _cb.onRunComplete(summary);
    },

    getBaseUrl: () => 'http://localhost:5180',
  };

  return _runner;

})();
