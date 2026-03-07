// ══════════════════════════════════════════════════════════════════════
//  test.runner.js  —  Solaris Platform · API Test Console  v3.1
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
//       Ejemplo: /proyectos/{DYN}/fases/{newFaseId}
//   [4] Eliminada la duplicación de getBaseUrl.
//   [5] captureId ahora captura data.id  | data.data.id | data.data[0].id
// ══════════════════════════════════════════════════════════════════════

const TestRunner = (() => {

  // ── Estado interno ──────────────────────────────────────────────
  let _jwt          = '';
  let _refreshToken = '';
  let _baseUrl      = '';

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
    const headers = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = 'Bearer ' + token;

    const t0 = performance.now();
    try {
      const res = await fetch(_baseUrl + path, {
        method,
        headers,
        body: body ? JSON.stringify(body) : undefined
      });
      const ms = Math.round(performance.now() - t0);
      let data;
      try { data = await res.json(); } catch { data = null; }
      return { status: res.status, data, ms };
    } catch (e) {
      const ms = Math.round(performance.now() - t0);
      return { status: 'ERR', data: { error: e.message, hint: 'No se pudo conectar. Verifica que el servidor esté corriendo y la Base URL sea correcta.' }, ms };
    }
  }

  // ── Check de expectativas ───────────────────────────────────────
  function _checkExpect(expect, status, data) {
    const expectedStatuses = Array.isArray(expect.status) ? expect.status : [expect.status];
    if (!expectedStatuses.includes(status)) return false;
    if (expect.check) return !!expect.check(data);
    return true;
  }

  // ── Diagnóstico automático al fallar ───────────────────────────
  function _buildDiagnosis(testDef, res) {
    const expected = Array.isArray(testDef.expect.status) ? testDef.expect.status : [testDef.expect.status];
    const statusOk = expected.includes(res.status);
    const lines    = [];

    if (!statusOk) {
      lines.push(`HTTP status incorrecto: esperado [${expected.join(' | ')}], recibido ${res.status}.`);
      if (res.status === 'ERR' || res.status === 0) {
        lines.push('La API no respondió. Verifica que el servidor esté corriendo y la Base URL sea correcta.');
      } else if (res.status === 500) {
        lines.push('Error 500 en el servidor. Revisar logs del backend para ver la excepción.');
      } else if (res.status === 401 && testDef.auth) {
        lines.push('401 con auth=true: el JWT puede haber expirado o no se capturó en el login. Asegúrate de que "login-ok" haya pasado primero.');
      } else if (res.status === 404 && !testDef.path.includes('99999')) {
        lines.push('404 inesperado: la ruta puede ser incorrecta o el controller no está registrado en Program.cs.');
      } else if (res.status === 403) {
        lines.push('403 Forbidden: el usuario autenticado no tiene el rol requerido para este endpoint.');
      }
    } else if (testDef.expect.check) {
      lines.push(`HTTP ${res.status} correcto, pero el check de contenido falló: "${testDef.expect.checkDesc}".`);
      lines.push('Revisar la estructura del objeto de respuesta — el campo esperado puede tener un nombre diferente.');
    }

    if (testDef.hint)        lines.push(`Pista: ${testDef.hint}`);
    if (res.data?.errors)    lines.push(`Errores de validación: ${JSON.stringify(res.data.errors)}`);
    if (res.data?.message)   lines.push(`Mensaje del servidor: ${res.data.message}`);
    if (res.data?.error)     lines.push(`Error del servidor: ${res.data.error}`);
    if (res.data?.title)     lines.push(`Título del error (ProblemDetails): ${res.data.title}`);

    return lines;
  }

  // ── [FIX 3] Resolver paths con uno o dos IDs dinámicos ─────────
  // Soporta:
  //   {DYN}        → DYNAMIC[testDef.useDynamic]
  //   {newFaseId}  → DYNAMIC['newFaseId']   (cualquier clave en el path)
  function _resolvePath(rawPath, testDef) {
    let path = rawPath;

    // Reemplazar {DYN} con el valor de useDynamic
    if (testDef.useDynamic && path.includes('{DYN}')) {
      const val = DYNAMIC[testDef.useDynamic];
      path = path.replace('{DYN}', val !== undefined ? val : 'MISSING_ID');
    }

    // Reemplazar cualquier otro {placeholder} con DYNAMIC[placeholder]
    path = path.replace(/\{([^}]+)\}/g, (match, key) => {
      const val = DYNAMIC[key];
      return val !== undefined ? val : match; // si no existe, lo deja para que falle con 404 visible
    });

    return path;
  }

  // ── Ejecutar un test individual ─────────────────────────────────
  async function _runTest(testDef, suiteId) {

    // [FIX 2] skipIf: omitir si el ID requerido no fue capturado
    if (testDef.skipIf && !DYNAMIC[testDef.skipIf]) {
      const result = {
        suiteId,
        testId:        testDef.id,
        passed:        true,   // se cuenta como skip (no como fallo)
        skipped:       true,
        status:        'SKIP',
        ms:            0,
        data:          { skipped: true, reason: `ID dinámico "${testDef.skipIf}" no disponible. El test previo de creación pudo haber fallado.` },
        diagnosis:     null,
        method:        testDef.method,
        path:          testDef.path,
        fullUrl:       _baseUrl + testDef.path,
        requiresAuth:  testDef.auth,
        expectedStatus: testDef.expect.status,
        checkDesc:     testDef.expect.checkDesc || null,
        hint:          testDef.hint || null,
        name:          testDef.name,
      };
      _cb.onTestStart(testDef.id);
      _results[testDef.id] = result;
      _cb.onTestEnd(testDef.id, result);
      return result;
    }

    _cb.onTestStart(testDef.id);

    // [FIX 3] Resolver path con IDs dinámicos
    const resolvedPath = _resolvePath(testDef.path, testDef);

    // Resolver body especial
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

    // [FIX 1] Resolver bodyFn con DYNAMIC y SEED
    if (testDef.bodyFn && BODY_FACTORIES[testDef.bodyFn]) {
      body = BODY_FACTORIES[testDef.bodyFn](DYNAMIC, SEED);
    }

    const res = await _fetch(testDef.method, resolvedPath, body, token);

    // Capturar JWT y refreshToken del login-ok
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

    // [FIX 1] Capturar ID de la respuesta en DYNAMIC
    if (testDef.captureId) {
      const capturedId =
        res.data?.data?.id   ??   // { data: { id: X } }
        res.data?.id         ??   // { id: X }
        (Array.isArray(res.data?.data) && res.data.data[0]?.id) ?? // { data: [{ id:X }] }
        null;
      if (capturedId !== null) {
        DYNAMIC[testDef.captureId] = capturedId;
      }
    }

    const passed    = _checkExpect(testDef.expect, res.status, res.data);
    const diagnosis = passed ? null : _buildDiagnosis(testDef, res);

    // Sanitizar token en la respuesta mostrada
    let displayData = res.data;
    if (res.data?.data?.token) {
      displayData = JSON.parse(JSON.stringify(res.data));
      displayData.data.token = displayData.data.token.substring(0, 50) + '…[truncado]';
    }

    const result = {
      suiteId,
      testId:   testDef.id,
      passed,
      skipped:  false,
      status:   res.status,
      ms:       res.ms,
      data:     displayData,
      diagnosis,
      method:        testDef.method,
      path:          resolvedPath,
      fullUrl:       _baseUrl + resolvedPath,
      requiresAuth:  testDef.auth,
      expectedStatus: testDef.expect.status,
      checkDesc:     testDef.expect.checkDesc || null,
      hint:          testDef.hint || null,
      name:          testDef.name,
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
    const all    = Object.values(_results).filter(r => !r.skipped);
    const total  = all.length;
    const passed = all.filter(r => r.passed).length;
    const failed = total - passed;
    const skipped = Object.values(_results).filter(r => r.skipped).length;
    return { total, passed, failed, skipped };
  }

  // ── API pública ─────────────────────────────────────────────────
  const _runner = {

    // El UI llama esto para proveer las credenciales
    getCredentials: () => ({ email: '', password: '' }),

    // Registrar callbacks
    on(event, fn) { if (_cb[event] !== undefined) _cb[event] = fn; return this; },

    // Configurar base URL
    setBaseUrl(url) { _baseUrl = url.replace(/\/$/, ''); return this; },

    // Limpiar estado (incluyendo DYNAMIC)
    reset() {
      _jwt = ''; _refreshToken = '';
      Object.keys(_results).forEach(k => delete _results[k]);
      Object.keys(DYNAMIC).forEach(k => delete DYNAMIC[k]);  // [FIX 1]
    },

    // Exponer resultados y DYNAMIC para test.report.js
    getResults()  { return { ..._results }; },
    getSuites()   { return TEST_SUITES; },
    getDynamic()  { return { ...DYNAMIC }; },  // útil para debug

    // Ejecutar una suite por id
    async runSuite(suiteId) {
      const suite = TEST_SUITES.find(s => s.id === suiteId);
      if (!suite) return;
      _baseUrl = _runner.getBaseUrl().replace(/\/$/, '');
      await _runSuite(suite);
      const summary = _countResults();
      _cb.onRunComplete(summary);
    },

    // Ejecutar todas las suites
    async runAll() {
      _baseUrl = _runner.getBaseUrl().replace(/\/$/, '');
      for (const suite of TEST_SUITES) {
        await _runSuite(suite);
        await _delay(350);
      }
      const summary = _countResults();
      _cb.onRunComplete(summary);
    },

    // [FIX 4] Una sola definición — el UI la sobreescribe en init()
    getBaseUrl: () => 'http://localhost:5180',
  };

  return _runner;

})();