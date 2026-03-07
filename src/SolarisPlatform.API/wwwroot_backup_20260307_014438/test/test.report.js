// ══════════════════════════════════════════════════════════════════════
//  test.report.js  —  Solaris Platform · API Test Console
//  ─────────────────────────────────────────────────────────────────────
//  Responsabilidad única: construir y descargar reportes JSON.
//  - No manipula el DOM más allá de crear/remover un <a> temporal.
//  - No ejecuta fetches.
//  Depende de: test.config.js, test.runner.js  (para leer resultados)
// ══════════════════════════════════════════════════════════════════════

const TestReport = (() => {

  // ── Construir el objeto de reporte completo ─────────────────────
  function _buildReport() {
    const now      = new Date();
    const baseUrl  = TestRunner.getBaseUrl();
    const results  = TestRunner.getResults();    // { testId: resultObj }
    const suites   = TestRunner.getSuites();     // TEST_SUITES

    // Todos los resultados ejecutados, en orden de suite
    const allTests = [];
    suites.forEach(suite => {
      suite.tests.forEach(t => {
        const res = results[t.id];
        if (!res) return;  // no ejecutado aún
        allTests.push({
          // ─ Identificación ─
          suite:      suite.title,
          suiteId:    suite.id,
          testId:     t.id,
          name:       t.name,
          // ─ Configuración del test ─
          method:         t.method,
          path:           t.path,
          fullUrl:        baseUrl + t.path,
          requiresAuth:   t.auth,
          expectedStatus: t.expect.status,
          checkDescription: t.expect.checkDesc || null,
          hint:           t.hint || null,
          // ─ Resultado ─
          passed:     res.passed,
          httpStatus: res.status,
          latencyMs:  res.ms,
          response:   res.data,
          // ─ Diagnóstico (solo en fallos) ─
          diagnosis:  res.passed ? null : res.diagnosis,
        });
      });
    });

    const failures = allTests.filter(t => !t.passed);
    const passed   = allTests.filter(t => t.passed);

    return {
      // ──────────────────────────────────────────────────────────
      // _meta: contexto de la ejecución
      // ──────────────────────────────────────────────────────────
      _meta: {
        tool:        'Solaris Platform · API Test Console',
        version:     '2.0',
        generatedAt: now.toISOString(),
        generatedAtLocal: now.toLocaleString('es-EC', { timeZone: 'America/Guayaquil' }),
        baseUrl,
        summary: {
          total:   allTests.length,
          passed:  passed.length,
          failed:  failures.length,
          pending: TEST_SUITES.reduce((acc, s) => acc + s.tests.length, 0) - allTests.length,
          passRate: allTests.length > 0
            ? Math.round((passed.length / allTests.length) * 100) + '%'
            : '0%'
        }
      },

      // ──────────────────────────────────────────────────────────
      // suites: resultados agrupados por suite
      // ──────────────────────────────────────────────────────────
      suites: suites.map(s => {
        const sTests = allTests.filter(t => t.suiteId === s.id);
        return {
          id:     s.id,
          title:  s.title,
          stats: {
            total:  sTests.length,
            passed: sTests.filter(t => t.passed).length,
            failed: sTests.filter(t => !t.passed).length,
          },
          tests: sTests
        };
      }),

      // ──────────────────────────────────────────────────────────
      // failures: lista plana de todos los fallos
      // Incluye diagnosis[] para diagnóstico rápido
      // ──────────────────────────────────────────────────────────
      failures,

      // ──────────────────────────────────────────────────────────
      // passed: lista plana de los que sí pasaron
      // ──────────────────────────────────────────────────────────
      passed
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
     * Archivo: solaris-test-results_YYYY-MM-DDTHH-MM-SS.json
     */
    downloadAll() {
      const report   = _buildReport();
      const filename = `solaris-test-results_${_timestamp()}.json`;
      _download(filename, report);
    },

    /**
     * Descarga solo los tests fallidos.
     * Ideal para compartir con el equipo de desarrollo o con Claude para diagnóstico.
     * Archivo: solaris-test-fails_YYYY-MM-DDTHH-MM-SS.json
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
          note: 'Este archivo contiene únicamente los tests fallidos. Incluye diagnosis[] por test para facilitar el diagnóstico.'
        },
        // Resumen de contexto de los que sí pasaron (sin detalle)
        contextPassed: full._meta.summary.passed + ' tests pasaron correctamente.',
        // Los fallos con diagnóstico completo
        failures: full.failures,
        // Agrupado por suite para facilitar lectura
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
