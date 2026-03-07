/**
 * theme.js
 * Maneja el toggle dark/light y persiste la preferencia.
 */

const html        = document.documentElement;
const themeToggle = document.getElementById('themeToggle');

// Aplica el tema guardado (o 'dark' por defecto)
const saved = localStorage.getItem('solaris-theme') || 'dark';
html.setAttribute('data-theme', saved);
themeToggle.checked = saved === 'light';

themeToggle.addEventListener('change', () => {
  const theme = themeToggle.checked ? 'light' : 'dark';
  html.setAttribute('data-theme', theme);
  localStorage.setItem('solaris-theme', theme);
});
