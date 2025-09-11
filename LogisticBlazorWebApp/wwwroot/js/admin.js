window.admin = (function () {
  const THEME_KEY = "admin-theme";

  function setTheme(theme) {
    document.documentElement.setAttribute("data-bs-theme", theme);
    try { localStorage.setItem(THEME_KEY, theme); } catch {}
  }

  function toggleTheme() {
    const current = document.documentElement.getAttribute("data-bs-theme") || "light";
    setTheme(current === "light" ? "dark" : "light");
  }

  function restoreTheme() {
    try {
      const saved = localStorage.getItem(THEME_KEY);
      if (saved) document.documentElement.setAttribute("data-bs-theme", saved);
    } catch {}
  }
  (function () {
  if (!window.bootstrap) return;
  const triggers = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
  triggers.forEach(el => new bootstrap.Tooltip(el));
    })();

  return { toggleTheme, restoreTheme };
})();
