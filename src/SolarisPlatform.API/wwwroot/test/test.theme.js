// ══════════════════════════════════════════════════════════════════════
//  test.theme.js  —  Solaris Platform
//  ─────────────────────────────────────────────────────────────────────
//  Manejo del tema light/dark.
//  Reutilizable en cualquier página del wwwroot (health.html, api.html…)
//
//  Uso:
//    SolarisTheme.init('themeToggle');   // id del <input type="checkbox">
// ══════════════════════════════════════════════════════════════════════

const SolarisTheme = (() => {
  const STORAGE_KEY = 'solaris-theme';
  const DEFAULT     = 'dark';

  function _apply(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    const toggle = document.getElementById(_toggleId);
    if (toggle) toggle.checked = (theme === 'light');
    localStorage.setItem(STORAGE_KEY, theme);
  }

  let _toggleId = 'themeToggle';

  return {
    init(toggleId = 'themeToggle') {
      _toggleId = toggleId;
      const saved = localStorage.getItem(STORAGE_KEY) || DEFAULT;
      _apply(saved);

      const toggle = document.getElementById(toggleId);
      if (toggle) {
        toggle.addEventListener('change', () => {
          _apply(toggle.checked ? 'dark' : 'light');
        });
      }
    },
    current() {
      return document.documentElement.getAttribute('data-theme') || DEFAULT;
    }
  };
})();