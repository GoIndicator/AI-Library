document.addEventListener("DOMContentLoaded", () => {
    // ----------------------------------------------------
    // Theme Switcher Logic
    // ----------------------------------------------------
    const themeButtons = document.querySelectorAll(".theme-btn");
    
    // Apply selected theme
    const applyTheme = (theme) => {
        const root = document.documentElement;
        
        // Remove existing theme classes
        root.classList.remove("light-theme", "dark-theme");
        
        // Clear active states on buttons
        themeButtons.forEach(btn => btn.classList.remove("active"));
        
        // Find and highlight matching button
        const activeBtn = document.querySelector(`.theme-btn[data-theme="${theme}"]`);
        if (activeBtn) activeBtn.classList.add("active");

        if (theme === "light") {
            root.classList.add("light-theme");
            localStorage.setItem("app-theme", "light");
        } else if (theme === "dark") {
            root.classList.add("dark-theme");
            localStorage.setItem("app-theme", "dark");
        } else {
            // System default
            localStorage.setItem("app-theme", "system");
            const systemPrefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
            if (!systemPrefersDark) {
                root.classList.add("light-theme");
            }
        }
    };

    // Load theme on startup
    const savedTheme = localStorage.getItem("app-theme") || "system";
    applyTheme(savedTheme);

    // Watch for system theme changes if set to system
    window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", (e) => {
        if (localStorage.getItem("app-theme") === "system") {
            const root = document.documentElement;
            if (e.matches) {
                root.classList.remove("light-theme");
            } else {
                root.classList.add("light-theme");
            }
        }
    });

    // Bind event listeners to theme switcher buttons
    themeButtons.forEach(btn => {
        btn.addEventListener("click", () => {
            const theme = btn.getAttribute("data-theme");
            applyTheme(theme);
        });
    });

    // ----------------------------------------------------
    // Sidebar Collapse Logic
    // ----------------------------------------------------
    const sidebar = document.querySelector(".sidebar");
    const mainContent = document.querySelector(".main-content");
    const collapseBtn = document.querySelector(".collapse-sidebar-btn");
    
    if (sidebar && collapseBtn) {
        const toggleSidebar = (isCollapsed) => {
            if (isCollapsed) {
                sidebar.classList.add("sidebar-collapsed");
                if (mainContent) mainContent.style.marginLeft = "var(--sidebar-collapsed-width)";
                localStorage.setItem("sidebar-collapsed", "true");
            } else {
                sidebar.classList.remove("sidebar-collapsed");
                if (mainContent) mainContent.style.marginLeft = "var(--sidebar-width)";
                localStorage.setItem("sidebar-collapsed", "false");
            }
        };

        // Load initial state
        const savedSidebarState = localStorage.getItem("sidebar-collapsed") === "true";
        toggleSidebar(savedSidebarState);

        // Click event
        collapseBtn.addEventListener("click", () => {
            const currentState = sidebar.classList.contains("sidebar-collapsed");
            toggleSidebar(!currentState);
        });
    }

    // ----------------------------------------------------
    // Copy Prompt to Clipboard
    // ----------------------------------------------------
    const copyBtns = document.querySelectorAll(".copy-prompt-btn");
    copyBtns.forEach(btn => {
        btn.addEventListener("click", async () => {
            const targetId = btn.getAttribute("data-target");
            const textarea = document.getElementById(targetId);
            
            if (textarea) {
                try {
                    await navigator.clipboard.writeText(textarea.value || textarea.innerText);
                    
                    // Show visual feedback
                    const originalHTML = btn.innerHTML;
                    btn.innerHTML = `
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" width="14" height="14">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" />
                        </svg>
                        COPIADO!
                    `;
                    btn.style.backgroundColor = "var(--badge-new)";
                    btn.style.color = "var(--badge-new-text)";
                    
                    setTimeout(() => {
                        btn.innerHTML = originalHTML;
                        btn.style.backgroundColor = "";
                        btn.style.color = "";
                    }, 2000);
                } catch (err) {
                    console.error("Failed to copy text: ", err);
                    alert("Não foi possível copiar o prompt automaticamente.");
                }
            }
        });
    });
});
