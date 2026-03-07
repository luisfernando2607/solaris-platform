/**
 * health.js
 * Consulta /health/data y actualiza todas las cards, badges y métricas de DB.
 */

const latencyHistory = [];

/* ── Helpers de DOM ─────────────────────────────────── */
const el = (id) => document.getElementById(id);

function setClass(id, cls)  { el(id).className = cls; }
function setText(id, text)  { el(id).textContent = text; }
function setHtml(id, html)  { el(id).innerHTML = html; }

/* ── Barras de latencia ─────────────────────────────── */
function updateLatencyBars() {
  const bars   = el('latencyBars');
  const maxMs  = Math.max(...latencyHistory, 50);
  const allBars = bars.querySelectorAll('.l-bar');

  latencyHistory.forEach((ms, i) => {
    const bar = allBars[i];
    if (!bar) return;
    const pct      = Math.max(8, Math.min(100, (ms / maxMs) * 100));
    bar.style.height = pct + '%';
    bar.className    = 'l-bar' + (ms > 200 ? ' err' : ms > 100 ? ' warn' : '');
  });
}

/* ── Conectores de arquitectura ─────────────────────── */
function litConnectors() {
  document.querySelectorAll('.arch-connector-line, .arch-connector-label')
    .forEach((node, i) => setTimeout(() => node.classList.add('lit'), i * 90));
}

/* ── Lógica principal del health check ─────────────── */
async function checkHealth() {
  try {
    const res  = await fetch('/health/data');
    const data = await res.json();
    const ok   = data.status === 'Healthy';

    // ── Global badge ──────────────────────────────────
    el('globalBadge').className = 'status-badge ' + (ok ? 'online' : 'offline');
    setText('globalBadgeText', ok ? 'ONLINE' : 'DEGRADED');

    // ── API card ──────────────────────────────────────
    setText('apiVersion', `v${data.version || '1.0.0'} · .NET 10`);

    // ── DB card ───────────────────────────────────────
    const db   = data.checks?.find(c => c.name === 'database');
    const dbOk = db?.status === 'Healthy';

    el('dbBadge').textContent = dbOk ? 'HEALTHY' : 'ERROR';
    el('dbBadge').className   = 'card-badge ' + (dbOk ? 'badge-ok' : 'badge-err');
    setHtml('dbDetail', dbOk
      ? `PostgreSQL 16 &nbsp;·&nbsp; <strong>${db.database}</strong> &nbsp;·&nbsp; ${db.latencyMs}ms`
      : `<span style="color:var(--red)">Sin conexión</span>`
    );

    // ── Env card ──────────────────────────────────────
    const env = data.environment || 'Development';
    el('envBadge').textContent = env.toUpperCase();
    el('envBadge').className   = 'card-badge badge-ok';
    setText('envDetail', new Date(data.timestamp).toLocaleString('es-EC'));

    // ── Environment Inspector ─────────────────────────
    const envClass = env === 'Production' ? 'env-prod'
                   : env === 'Staging'    ? 'env-stg'
                   : 'env-dev';
    setHtml('ei-env', `<span class="env-badge ${envClass}">${env.toUpperCase()}</span>`);
    setText('envInspBadge', env.toUpperCase());
    el('envInspBadge').style.color = env === 'Production' ? 'var(--green)' : 'var(--yellow)';
    setText('ei-version',   `v${data.version || '1.0.0'}`);
    setText('ei-lastcheck', new Date().toLocaleTimeString('es-EC'));

    // ── DB Console ────────────────────────────────────
    if (db) {
      const ms   = db.latencyMs ?? 0;
      const qual = ms < 50 ? 'excelente' : ms < 200 ? 'normal' : 'lento';

      el('dbc-conn').textContent = dbOk ? '✓ Conectado' : '✗ Error';
      el('dbc-conn').className   = 'db-stat-val ' + (dbOk ? 'ok' : 'err');
      setText('dbc-name',    db.database || 'solaris_db');
      setText('dbc-latency', `${ms}ms — ${qual}`);
      el('dbc-latency').className = 'db-stat-val ' + (ms < 50 ? 'ok' : ms < 200 ? '' : 'warn');
      setText('dbConsoleBadge', `${ms}ms`);

      // Historial de latencia (máx. 12)
      latencyHistory.push(ms);
      if (latencyHistory.length > 12) latencyHistory.shift();
      const avg = Math.round(latencyHistory.reduce((a, b) => a + b, 0) / latencyHistory.length);
      setText('dbc-avg', `${avg}ms`);
      updateLatencyBars();

      // ── Arch layer DB ─────────────────────────────
      setText('dbArchBadge',   `PostgreSQL · ${ms}ms`);
      setText('dbArchTagline', `${db.database || 'solaris_db'} · latencia ${ms}ms — ${qual}`);
      const latEl = el('dbArchLatency');
      if (latEl) latEl.textContent = `Latencia en tiempo real: ${ms}ms — ${qual}`;
    }

    if (ok) litConnectors();

  } catch {
    el('globalBadge').className   = 'status-badge offline';
    setText('globalBadgeText',      'OFFLINE');
    setText('dbBadge',              'ERROR');
    el('dbBadge').className        = 'card-badge badge-err';
    setHtml('dbDetail', `<span style="color:var(--red)">No se pudo alcanzar la API</span>`);
    setText('dbc-conn',             '✗ Sin conexión');
    el('dbc-conn').className       = 'db-stat-val err';
  }
}

checkHealth();
setInterval(checkHealth, 30_000);
