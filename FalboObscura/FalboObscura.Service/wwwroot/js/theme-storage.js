// Theme storage and DOM utilities
window.themeStorage = {
    getTheme: function () {
        return localStorage.getItem('theme') || 'dark';
    },
    setTheme: function (theme) {
        localStorage.setItem('theme', theme);
    },
    applyThemeToBody: function (themeName) {
        // Remove all theme classes
        document.body.classList.remove('theme-dark', 'theme-light');

        // Add the new theme class
        if (themeName && themeName !== 'dark') {
            document.body.classList.add('theme-' + themeName);
        } else {
            // Dark is default, no class needed since :root has dark theme
            document.body.classList.add('theme-dark');
        }
    },
    initializeTheme: function () {
        const savedTheme = this.getTheme();
        this.applyThemeToBody(savedTheme);
        return savedTheme;
    }
};