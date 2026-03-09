// ══════════════════════════════════════════════════════════════════════
//  test.report.js  —  Solaris Platform · API Test Console  v2.2
//  ─────────────────────────────────────────────────────────────────────
//  Responsabilidad única: construir y descargar reportes JSON.
//  - No manipula el DOM más allá de crear/remover un <a> temporal.
//  - No ejecuta fetches.
//  Depende de: test.config.js, test.runner.js  (para leer resultados)
//
//  FIXES v2.1:
//   [BUG-1] Reemplazado acceso directo a TEST_SUITES global por
//           TestRunner.getSuites().
//   [BUG-3] Los tests con skipped:true ya no aparecen en "passed".
//
//  FIXES v2.2:
//   [DIAG-1] Incluye dynamicSnapshot en cada fallo: estado exacto de
//            DYNAMIC al momento en que ese test se ejecutó.
//   [DIAG-3] Incluye serverDebug con innerException completo del backend
//            (disponible cuando el middleware está en modo Development).
//   [DIAG-4] Añade sección efCoreHints: resumen de errores de EF Core /
//            PostgreSQL detectados automáticamente entre todos los fallos.
// ══════════════════════════════════════════════════════════════════════

const TestReport = (() => {

  // ── Detectar patrones conocidos de EF Core en los fallos ────────
  function _extractEfCoreHints(failures) {
    const hints = [];
    const seen  = new Set();

    failures.forEach(f => {
      const raw = JSON.stringify(f.data || '') + JSON.stringify(f.serverDebug || '');
      const low = raw.toLowerCase();

      const add = (key, msg) => { if (!seen.has(key)) { seen.add(key); hints.push(msg); } };

      if (low.includes('kind=unspecified') || low.includes('timestamp with time zone')) {
        add('datetime', `[${f.testId}] DateTime Kind=Unspecified → campo de fecha enviado sin zona UTC. Usar 'YYYY-MM-DD' (DateOnly) o 'YYYY-MM-DDTHH:mm:ssZ' (DateTime UTC).`);
      }
      if (low.includes('foreign key') || low.includes('violates foreign key')) {
        const tbl = raw.match(/table "([^"]+)"/)?.[1] || '?';
        const fk  = raw.match(/constraint "([^"]+)"/)?.[1] || '?';
        add('fk_' + tbl, `[${f.testId}] FK violation en tabla '${tbl}', constraint '${fk}'. El registro padre no existe o create previo falló.`);
      }
      if (low.includes('duplicate key') || low.includes('unique')) {
        const uc = raw.match(/constraint "([^"]+)"/)?.[1] || '?';
        add('uq_' + uc, `[${f.testId}] Unique constraint '${uc}'. Ya existe ese valor en la BD — limpiar datos de prueba anteriores.`);
      }
      if (low.includes('not-null') || low.includes('null value in column')) {
        const col = raw.match(/column "([^"]+)"/)?.[1] || '?';
        add('nn_' + col, `[${f.testId}] NOT NULL violado en columna '${col}'. Falta campo en el body o el servicio no lo mapea.`);
      }
      if (low.includes('automapper') || low.includes('constructusing') || low.includes('mapping')) {
        add('mapper', `[${f.testId}] Error de AutoMapper. Revisar ConstructUsing() para DTOs con record posicional.`);
      }
      if (low.includes('the request field is required')) {
        add('req_' + f.testId, `[${f.testId}] "The request field is required" → el body completo no llega al controller. Revisar [FromBody] y Content-Type.`);
      }
    });

    return hints;
  }

  // ── Construir el objeto de reporte completo ─────────────────────
  function _buildReport() {
    const now     = new Date();
    const baseUrl = TestRunner.getBaseUrl();
    const results = TestRunner.getResults();
    const suites  = TestRunner.getSuites();

    const allTests = [];
    suites.forEach(suite => {
      suite.tests.forEach(t => {
        const res = results[t.id];
        if (!res) return;
        allTests.push({
          // ─ Identificación ─
          suite:      suite.title,
          suiteId:    suite.id,
          testId:     t.id,
          name:       t.name,
          // ─ Configuración del test ─
          method:           t.method,
          path:             res.path       || t.path,
          fullUrl:          res.fullUrl    || baseUrl + t.path,
          requiresAuth:     t.auth,
          expectedStatus:   t.expect.status,
          checkDescription: t.expect.checkDesc || null,
          hint:             t.hint || null,
          // ─ Resultado ─
          passed:     res.passed,
          skipped:    res.skipped || false,
          httpStatus: res.status,
          latencyMs:  res.ms,
          requestBody: res.requestBody ?? null,
          response:   res.data,
          // ─ [DIAG-1] Estado de DYNAMIC al ejecutar ─
          dynamicSnapshot: res.dynamicSnapshot ?? null,
          // ─ [DIAG-3] Debug del servidor (solo fallos, solo en dev) ─
          serverDebug: res.serverDebug ?? null,
          // ─ Diagnóstico (solo en fallos) ─
          diagnosis:  res.passed ? null : res.diagnosis,
        });
      });
    });

    const failures = allTests.filter(t => !t.passed && !t.skipped);
    const passed   = allTests.filter(t =>  t.passed && !t.skipped);
    const skipped  = allTests.filter(t =>  t.skipped);

    const totalDefined = suites.reduce((acc, s) => acc + s.tests.length, 0);

    // [DIAG-4] Resumen de errores de EF Core detectados
    const efCoreHints = _extractEfCoreHints(failures);

    return {
      _meta: {
        tool:        'Solaris Platform · API Test Console',
        version:     '2.2',
        generatedAt: now.toISOString(),
        generatedAtLocal: now.toLocaleString('es-EC', { timeZone: 'America/Guayaquil' }),
        baseUrl,
        summary: {
          totalDefined,
          executed: allTests.length,
          passed:   passed.length,
          failed:   failures.length,
          skipped:  skipped.length,
          pending:  totalDefined - allTests.length,
          passRate: allTests.length > 0
            ? Math.round((passed.length / (allTests.length - skipped.length || 1)) * 100) + '%'
            : '0%'
        }
      },

      // [DIAG-4] Errores de EF Core / PostgreSQL detectados automáticamente
      efCoreHints: efCoreHints.length > 0 ? efCoreHints : null,

      suites: suites.map(s => {
        const sTests = allTests.filter(t => t.suiteId === s.id);
        return {
          id:    s.id,
          title: s.title,
          stats: {
            total:   sTests.length,
            passed:  sTests.filter(t =>  t.passed && !t.skipped).length,
            failed:  sTests.filter(t => !t.passed && !t.skipped).length,
            skipped: sTests.filter(t =>  t.skipped).length,
          },
          tests: sTests
        };
      }),

      failures,
      passed,
      skipped
    };
  }

  // ── Trigger de descarga ─────────────────────────────────────────
  function _download(filename, payload) {
    const json = JSON.stringify(payload, null, 2);
    const blob = new Blob([json], { type: 'application/json; charset=utf-8' });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }

  function _timestamp() {
    return new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19);
  }

  // ── API pública ─────────────────────────────────────────────────
  return {

    /**
     * Descarga el reporte completo (todos los tests ejecutados).
     */
    downloadAll() {
      const report   = _buildReport();
      const filename = `solaris-test-results_${_timestamp()}.json`;
      _download(filename, report);
    },

    /**
     * Descarga solo los tests fallidos con diagnóstico completo:
     * - diagnosis[]: hints automáticos
     * - dynamicSnapshot: IDs capturados al momento del fallo
     * - serverDebug.innerException: cadena de inner exceptions del backend
     * - efCoreHints[]: resumen de errores de EF Core detectados
     */
    downloadFails() {
      const full = _buildReport();

      if (full.failures.length === 0) {
        alert('¡No hay fallos que descargar! Todos los tests pasaron.');
        return;
      }

      const payload = {
        _meta: {
          ...full._meta,
          mode: 'SOLO_FALLOS',
          note: 'Incluye diagnosis[], dynamicSnapshot, serverDebug.innerException y efCoreHints para diagnóstico preciso.'
        },
        contextPassed:  `${full._meta.summary.passed} test(s) pasaron correctamente.`,
        contextSkipped: `${full._meta.summary.skipped} test(s) fueron omitidos (skipIf).`,

        // [DIAG-4] Lo primero: resumen de problemas detectados
        efCoreHints: full.efCoreHints,

        failures: full.failures,

        failuresBySuite: (() => {
          const bySuite = {};
          full.failures.forEach(f => {
            if (!bySuite[f.suiteId]) bySuite[f.suiteId] = { suite: f.suite, tests: [] };
            bySuite[f.suiteId].tests.push(f);
          });
          return bySuite;
        })()
      };

      const filename = `solaris-test-fails_${_timestamp()}.json`;
      _download(filename, payload);
    }
  };

})();
