// ══════════════════════════════════════════════════════════════════════
//  test.report.js  —  Solaris Platform · API Test Console  v2.1
//  ─────────────────────────────────────────────────────────────────────
//  Responsabilidad única: construir y descargar reportes JSON.
//  - No manipula el DOM más allá de crear/remover un <a> temporal.
//  - No ejecuta fetches.
//  Depende de: test.config.js, test.runner.js  (para leer resultados)
//
//  FIXES v2.1:
//   [BUG-1] Reemplazado acceso directo a TEST_SUITES global por
//           TestRunner.getSuites(). Elimina dependencia frágil de orden
//           de carga de scripts.
//   [BUG-3] Los tests con skipped:true ya no aparecen en la lista
//           "passed" del reporte. Se añade una sección "skipped"
//           separada con su propio conteo en el summary.
// ══════════════════════════════════════════════════════════════════════

const TestReport = (() => {

  // ── Construir el objeto de reporte completo ─────────────────────
  function _buildReport() {
    const now     = new Date();
    const baseUrl = TestRunner.getBaseUrl();
    const results = TestRunner.getResults();    // { testId: resultObj }
    const suites  = TestRunner.getSuites();     // [BUG-1 FIX] usa getSuites() en vez de TEST_SUITES global

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
          method:           t.method,
          path:             t.path,
          fullUrl:          baseUrl + t.path,
          requiresAuth:     t.auth,
          expectedStatus:   t.expect.status,
          checkDescription: t.expect.checkDesc || null,
          hint:             t.hint || null,
          // ─ Resultado ─
          passed:     res.passed,
          skipped:    res.skipped || false,   // [BUG-3 FIX] propagar el flag
          httpStatus: res.status,
          latencyMs:  res.ms,
          response:   res.data,
          // ─ Diagnóstico (solo en fallos) ─
          diagnosis:  res.passed ? null : res.diagnosis,
        });
      });
    });

    // [BUG-3 FIX] Separar correctamente passed / failed / skipped
    const failures = allTests.filter(t => !t.passed && !t.skipped);
    const passed   = allTests.filter(t =>  t.passed && !t.skipped);
    const skipped  = allTests.filter(t =>  t.skipped);

    // [BUG-1 FIX] Contar pending usando getSuites() en lugar de TEST_SUITES
    const totalDefined = suites.reduce((acc, s) => acc + s.tests.length, 0);

    return {
      // ──────────────────────────────────────────────────────────
      // _meta: contexto de la ejecución
      // ──────────────────────────────────────────────────────────
      _meta: {
        tool:        'Solaris Platform · API Test Console',
        version:     '2.1',
        generatedAt: now.toISOString(),
        generatedAtLocal: now.toLocaleString('es-EC', { timeZone: 'America/Guayaquil' }),
        baseUrl,
        summary: {
          totalDefined,                           // total de tests definidos en config
          executed: allTests.length,              // cuántos se ejecutaron (ran + skip)
          passed:   passed.length,                // pasaron de verdad (no skip)
          failed:   failures.length,
          skipped:  skipped.length,               // [BUG-3 FIX] conteo propio de skips
          pending:  totalDefined - allTests.length,
          passRate: allTests.length > 0
            ? Math.round((passed.length / (allTests.length - skipped.length || 1)) * 100) + '%'
            : '0%'
        }
      },

      // ──────────────────────────────────────────────────────────
      // suites: resultados agrupados por suite
      // ──────────────────────────────────────────────────────────
      suites: suites.map(s => {
        const sTests = allTests.filter(t => t.suiteId === s.id);
        return {
          id:    s.id,
          title: s.title,
          stats: {
            total:   sTests.length,
            passed:  sTests.filter(t =>  t.passed && !t.skipped).length,   // [BUG-3 FIX]
            failed:  sTests.filter(t => !t.passed && !t.skipped).length,   // [BUG-3 FIX]
            skipped: sTests.filter(t =>  t.skipped).length,                // [BUG-3 FIX]
          },
          tests: sTests
        };
      }),

      // ──────────────────────────────────────────────────────────
      // failures: lista plana de todos los fallos reales
      // Incluye diagnosis[] para diagnóstico rápido
      // ──────────────────────────────────────────────────────────
      failures,

      // ──────────────────────────────────────────────────────────
      // passed: solo tests que realmente pasaron (sin skips)  [BUG-3 FIX]
      // ──────────────────────────────────────────────────────────
      passed,

      // ──────────────────────────────────────────────────────────
      // skipped: tests omitidos por skipIf (ID dinámico ausente) [BUG-3 FIX]
      // ──────────────────────────────────────────────────────────
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
     * Incluye secciones: passed, failed, skipped.
     * Archivo: solaris-test-results_YYYY-MM-DDTHH-MM-SS.json
     */
    downloadAll() {
      const report   = _buildReport();
      const filename = `solaris-test-results_${_timestamp()}.json`;
      _download(filename, report);
    },

    /**
     * Descarga solo los tests fallidos (excluye skips).
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
          note: 'Este archivo contiene únicamente los tests fallidos (los skipped se omiten). Incluye diagnosis[] por test para facilitar el diagnóstico.'
        },
        // Resumen de contexto de los que sí pasaron y los que fueron skipped
        contextPassed:  `${full._meta.summary.passed} test(s) pasaron correctamente.`,
        contextSkipped: `${full._meta.summary.skipped} test(s) fueron omitidos (skipIf).`,  // [BUG-3 FIX]
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
