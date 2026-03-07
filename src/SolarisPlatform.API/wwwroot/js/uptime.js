/**
 * uptime.js
 * Contador de uptime desde que se cargó la página.
 */

const startTime = Date.now();

function updateUptime() {
  const totalSeconds = Math.floor((Date.now() - startTime) / 1000);
  const h   = String(Math.floor(totalSeconds / 3600)).padStart(2, '0');
  const m   = String(Math.floor((totalSeconds % 3600) / 60)).padStart(2, '0');
  const sec = String(totalSeconds % 60).padStart(2, '0');
  document.getElementById('uptimeVal').textContent = `${h}:${m}:${sec}`;
}

updateUptime();
setInterval(updateUptime, 1000);
