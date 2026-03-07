// ══════════════════════════════════════════════════════════════════════
//  test.ui.js  —  Solaris Platform · API Test Console
//  ─────────────────────────────────────────────────────────────────────
//  Responsabilidad única: manipular el DOM.
//  - Genera el HTML de suites y test-rows dinámicamente desde TEST_SUITES.
//  - Escucha los callbacks del TestRunner para actualizar la UI.
//  - No ejecuta fetches ni conoce lógica de negocio.
//  Depende de: test.config.js, test.runner.js
// ══════════════════════════════════════════════════════════════════════

const TestUI = (() => {

  // ── Contadores por suite ────────────────────────────────────────
  const _counters = {};

  // ── Helpers de escape ───────────────────────────────────────────
  function esc(s) {
    return String(s)
      .replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
  }

  // ── JSON coloreado ──────────────────────────────────────────────
  function colorJson(data) {
    if (data === null || data === undefined) return '<span class="json-null">null</span>';
    const raw = JSON.stringify(data, null, 2);
    return esc(raw).replace(
      /("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g,
      m => {
        if (/^"/.test(m)) {
          return /:$/.test(m)
            ? `<span class="json-key">${m}</span>`
            : `<span class="json-str">${m}</span>`;
        }
        if (/true|false/.test(m)) return `<span class="json-bool">${m}</span>`;
        if (/null/.test(m))       return `<span class="json-null">${m}</span>`;
        return `<span class="json-num">${m}</span>`;
      }
    );
  }

  // ── Render de una fila de test ──────────────────────────────────
  function _buildTestRow(t) {
    const badge = `<span class="method-badge m-${t.method.toLowerCase()}">${t.method}</span>`;
    const nameNoMethod = t.name.replace(/^(GET|POST|PUT|PATCH|DELETE)\s/, '');
    return `
      <div class="test-row" id="t-${t.id}">
        <span class="test-icon">⏳</span>
        <div class="test-body">
          <span class="test-name">${badge} ${esc(nameNoMethod)}</span>
          <span class="test-desc">${esc(t.path || "")}</span>
          <div class="test-detail" id="d-${t.id}">
            <div class="detail-tabs">
              <button class="detail-tab active" onclick="TestUI.switchTab('${t.id}','response',this)">Respuesta</button>
              <button class="detail-tab"        onclick="TestUI.switchTab('${t.id}','info',this)">Info</button>
            </div>
            <div class="detail-pane active" id="pane-response-${t.id}">
              <pre class="detail-response" id="r-${t.id}">—</pre>
              <div class="error-hint" id="hint-${t.id}" style="display:none"></div>
            </div>
            <div class="detail-pane" id="pane-info-${t.id}">
              <div class="detail-info" id="info-${t.id}">—</div>
            </div>
          </div>
        </div>
        <div class="test-meta">
          <span class="test-status-code" id="c-${t.id}">—</span>
          <span class="test-ms"          id="m-${t.id}"></span>
          <button class="toggle-detail"  onclick="TestUI.toggleDetail('${t.id}')">ver</button>
        </div>
      </div>`;
  }

  // ── Render de suites ────────────────────────────────────────────
  function render() {
    const container = document.getElementById('suitesContainer');
    const nav       = document.getElementById('suiteNav');
    if (!container || !nav) return;

    container.innerHTML = '';
    nav.innerHTML       = '';

    TEST_SUITES.forEach(suite => {
      _counters[suite.id] = { ok: 0, err: 0, skip: suite.tests.length };

      // Nav pill
      const pill = document.createElement('button');
      pill.className = 'suite-pill';
      pill.id        = `nav-${suite.id}`;
      pill.innerHTML = `${suite.icon} ${esc(suite.title)} <span class="pill-count" id="pill-count-${suite.id}">·${suite.tests.length}</span>`;
      pill.onclick   = () => TestUI.runSuite(suite.id);
      nav.appendChild(pill);

      // Suite card
      const card = document.createElement('div');
      card.className = 'suite';
      card.id        = `suite-${suite.id}`;

      card.innerHTML = `
        <div class="suite-header" onclick="TestUI.toggleSuite('${suite.id}')">
          <span class="suite-icon">${suite.icon}</span>
          <div>
            <div class="suite-title">${esc(suite.title)}</div>
            <div class="suite-desc">${esc(suite.desc)}</div>
          </div>
          <div class="suite-stats">
            <span class="stat-ok"   id="${suite.id}-ok">✓0</span>
            <span class="stat-err"  id="${suite.id}-err">✗0</span>
            <span class="stat-skip" id="${suite.id}-skip">—${suite.tests.length}</span>
            <button class="suite-run-btn"
              onclick="event.stopPropagation(); TestUI.runSuite('${suite.id}')">▶ run</button>
            <span class="suite-chevron">▼</span>
          </div>
        </div>
        <div class="test-list">${suite.tests.map(_buildTestRow).join('')}</div>`;

      container.appendChild(card);
    });
  }

  // ── Actualizar fila de test ─────────────────────────────────────
  function _applyResult(testId, result, testDef) {
    const row    = document.getElementById(`t-${testId}`);
    if (!row) return;

    row.className = 'test-row ' + (result.passed ? 'state-ok' : 'state-fail');
    row.querySelector('.test-icon').textContent = result.passed ? '✅' : '❌';

    // Código HTTP
    const codeEl = document.getElementById(`c-${testId}`);
    codeEl.textContent = result.status;
    codeEl.className   = 'test-status-code ' + (result.passed ? 'ok' : 'err');

    // Tiempo
    document.getElementById(`m-${testId}`).textContent = result.ms + 'ms';

    // Respuesta JSON coloreada
    document.getElementById(`r-${testId}`).innerHTML = colorJson(result.data);

    // Auto-expandir en fallo
    if (!result.passed) {
      document.getElementById(`d-${testId}`).classList.add('visible');
    }

    // Hint / diagnóstico
    const hintEl = document.getElementById(`hint-${testId}`);
    if (!result.passed && result.diagnosis?.length) {
      const expectedStr = Array.isArray(testDef.expect.status)
        ? testDef.expect.status.join(' o ')
        : testDef.expect.status;
      let html = `<b>❌ Fallo — HTTP ${result.status} (esperado: ${expectedStr})</b>`;
      result.diagnosis.forEach(line => { html += `<br>• ${esc(line)}`; });
      hintEl.innerHTML      = html;
      hintEl.style.display  = '';
    } else {
      hintEl.style.display = 'none';
    }

    // Info tab
    if (testDef) {
      document.getElementById(`info-${testId}`).innerHTML = `
        <span><b>URL</b> ${esc(result.fullUrl)}</span>
        <span><b>Método</b> ${result.method}</span>
        <span><b>Auth</b> ${result.requiresAuth ? 'JWT Bearer' : 'Sin auth'}</span>
        <span><b>HTTP recibido</b> ${result.status}</span>
        <span><b>Tiempo</b> ${result.ms}ms</span>
        ${result.checkDesc ? `<span><b>Check</b> ${esc(result.checkDesc)}</span>` : ''}
      `;
    }
  }

  // ── Actualizar contadores de suite ──────────────────────────────
  function _updateSuiteStats(suiteId) {
    const c    = _counters[suiteId];
    const okEl   = document.getElementById(`${suiteId}-ok`);
    const errEl  = document.getElementById(`${suiteId}-err`);
    const skipEl = document.getElementById(`${suiteId}-skip`);
    if (okEl)   okEl.textContent   = `✓${c.ok}`;
    if (errEl)  errEl.textContent  = `✗${c.err}`;
    if (skipEl) skipEl.textContent = `—${c.skip}`;

    // Color nav pill
    const pill = document.getElementById(`nav-${suiteId}`);
    if (pill) {
      if (c.err > 0) {
        pill.style.borderColor = 'var(--red)';   pill.style.color = 'var(--red)';
      } else if (c.ok > 0) {
        pill.style.borderColor = 'var(--green)'; pill.style.color = 'var(--green)';
      }
    }
  }

  // ── Barra de progreso ───────────────────────────────────────────
  let _progressTotal = 0;
  let _progressDone  = 0;
  let _progressFails = 0;

  function _updateProgress() {
    const pct = _progressTotal > 0 ? Math.round((_progressDone / _progressTotal) * 100) : 0;
    const bar  = document.getElementById('progressBar');
    if (!bar) return;
    bar.style.width = pct + '%';
    bar.className   = 'progress-bar' + (_progressFails > 0 ? ' has-errors' : '');
  }

  // ── Resumen final ───────────────────────────────────────────────
  function _showSummary(summary) {
    const el  = document.getElementById('summary');
    if (!el) return;
    const allOk = summary.failed === 0;
    el.className = 'summary show ' + (allOk ? 'pass' : 'fail');
    document.getElementById('summaryText').textContent =
      allOk
        ? `✅ Todos los tests pasaron (${summary.passed}/${summary.total})`
        : `❌ ${summary.failed} test(s) fallaron`;
    document.getElementById('summaryDetail').textContent =
      `${summary.passed} correctos · ${summary.failed} fallidos · ${summary.total} total`;

    // Habilitar botones de descarga
    const btnAll   = document.getElementById('btnDlAll');
    const btnFails = document.getElementById('btnDlFails');
    if (btnAll)   btnAll.disabled   = false;
    if (btnFails) btnFails.disabled = (summary.failed === 0);
  }

  // ── API pública ─────────────────────────────────────────────────
  const _ui = {

    // Inicializar: render + vincular callbacks del runner
    init() {
      render();

      // Contar total de tests para la barra de progreso
      _progressTotal = TEST_SUITES.reduce((acc, s) => acc + s.tests.length, 0);

      // Obtener el mapa testId → testDef para los callbacks
      const testMap = {};
      TEST_SUITES.forEach(s => s.tests.forEach(t => { testMap[t.id] = { ...t, suiteId: s.id }; }));

      // Vincular TestRunner
      TestRunner
        .on('onTestStart', (testId) => {
          const row = document.getElementById(`t-${testId}`);
          if (row) { row.className = 'test-row'; row.querySelector('.test-icon').textContent = '⏳'; }
        })
        .on('onTestEnd', (testId, result) => {
          const def = testMap[testId];
          if (!def) return;
          _applyResult(testId, result, def);

          if (result.passed) { _counters[def.suiteId].ok++; }
          else               { _counters[def.suiteId].err++; _progressFails++; }
          _counters[def.suiteId].skip = Math.max(0, _counters[def.suiteId].skip - 1);
          _updateSuiteStats(def.suiteId);

          _progressDone++;
          _updateProgress();
        })
        .on('onRunComplete', _showSummary)
        .on('onTokenCapture', (token, truncated) => {
          const el = document.getElementById('tokenDisplay');
          if (el) { el.textContent = truncated; el.className = 'token-val'; }
        });

      // Proveer credenciales al runner
      TestRunner.getCredentials = () => ({
        email:    document.getElementById('email')?.value    || '',
        password: document.getElementById('password')?.value || ''
      });
      TestRunner.getBaseUrl = () =>
        (document.getElementById('baseUrl')?.value || 'http://localhost:5180').replace(/\/$/, '');
    },

    // Limpiar toda la UI
    clearAll() {
      TestRunner.reset();
      _progressDone = 0; _progressFails = 0;

      TEST_SUITES.forEach(s => {
        _counters[s.id] = { ok: 0, err: 0, skip: s.tests.length };
        _updateSuiteStats(s.id);
        // Reset color pill
        const pill = document.getElementById(`nav-${s.id}`);
        if (pill) { pill.style.borderColor = ''; pill.style.color = ''; }
      });

      document.querySelectorAll('.test-row').forEach(el => {
        el.className = 'test-row';
        el.querySelector('.test-icon').textContent = '⏳';
      });
      document.querySelectorAll('.test-status-code').forEach(el => { el.textContent = '—'; el.className = 'test-status-code'; });
      document.querySelectorAll('.test-ms').forEach(el => el.textContent = '');
      document.querySelectorAll('.detail-response').forEach(el => el.innerHTML = '—');
      document.querySelectorAll('.test-detail').forEach(el => el.classList.remove('visible'));
      document.querySelectorAll('.error-hint').forEach(el => el.style.display = 'none');

      const td = document.getElementById('tokenDisplay');
      if (td) { td.textContent = 'Sin token — ejecuta los tests primero'; td.className = 'token-val empty'; }

      const summary = document.getElementById('summary');
      if (summary) summary.className = 'summary';

      const bar = document.getElementById('progressBar');
      if (bar) { bar.style.width = '0%'; bar.className = 'progress-bar'; }
      const wrap = document.getElementById('progressWrap');
      if (wrap) wrap.classList.remove('visible');

      const btnAll   = document.getElementById('btnDlAll');
      const btnFails = document.getElementById('btnDlFails');
      if (btnAll)   btnAll.disabled   = true;
      if (btnFails) btnFails.disabled = true;
    },

    // Ejecutar desde el botón Run All
    async runAll() {
      this.clearAll();
      const btn = document.getElementById('btnRunAll');
      if (btn) { btn.disabled = true; btn.innerHTML = '<span class="spin"></span> Ejecutando…'; }
      document.getElementById('progressWrap')?.classList.add('visible');

      await TestRunner.runAll();

      if (btn) { btn.disabled = false; btn.innerHTML = '▶ Ejecutar todos los tests'; }
    },

    // Ejecutar una sola suite
    async runSuite(suiteId) {
      const btn = document.getElementById('btnRunAll');
      if (btn) btn.disabled = true;
      document.getElementById('progressWrap')?.classList.add('visible');
      await TestRunner.runSuite(suiteId);
      if (btn) btn.disabled = false;
    },

    // Toggle detalle de un test
    toggleDetail(id) {
      document.getElementById(`d-${id}`)?.classList.toggle('visible');
    },

    // Cambiar tab dentro de un test
    switchTab(id, pane, btn) {
      const detail = document.getElementById(`d-${id}`);
      if (!detail) return;
      detail.querySelectorAll('.detail-tab').forEach(t => t.classList.remove('active'));
      detail.querySelectorAll('.detail-pane').forEach(p => p.classList.remove('active'));
      btn.classList.add('active');
      document.getElementById(`pane-${pane}-${id}`)?.classList.add('active');
    },

    // Toggle colapsar suite
    toggleSuite(id) {
      document.getElementById(`suite-${id}`)?.classList.toggle('collapsed');
    }
  };

  return _ui;

})();