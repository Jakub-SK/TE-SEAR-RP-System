document.addEventListener("DOMContentLoaded", () => {

    const tabs = document.querySelectorAll(".tab");
    const contents = document.querySelectorAll(".tab-content");
    const classItems = document.querySelectorAll(".class-item");
    const indicator = document.querySelector(".tab-indicator");

    // ===== INDICATOR FUNCTION =====
    function moveIndicator(tab) {

        // If stacked = indicator becomes full-width underline
        if (isTabsStacked()) {
            indicator.style.width = "100%";
            indicator.style.left = "0";
            indicator.style.top = tab.offsetTop + tab.offsetHeight + "px";
            return;
        }

        // normal horizontal behavior
        const tabRect = tab.getBoundingClientRect();
        const parentRect = tab.parentElement.getBoundingClientRect();

        indicator.style.top = "";   // reset vertical mode
        indicator.style.width = tabRect.width + "px";
        indicator.style.left = (tabRect.left - parentRect.left) + "px";
    }


    // helps when Media-Queries turn the nav menu to stacked on top instead of next to each other

    function isTabsStacked() {
        return window.getComputedStyle(document.querySelector(".tabs-header"))
            .flexDirection === "column";
    }

    // initial position, the class goes to the selected manual 
    // (exp. Shocktrooper picked in menu when selecting Shocktrooper and site reloads)

    moveIndicator(document.querySelector(".tab.active"));


    // ===== TAB SWITCHING =====
    tabs.forEach((tab, index) => {
        tab.addEventListener("click", () => {

            tabs.forEach(t => t.classList.remove("active"));
            contents.forEach(c => c.classList.remove("active"));

            tab.classList.add("active");
            document
                .querySelector(`.tab-content[data-tab="${index}"]`)
                .classList.add("active");

            moveIndicator(tab);
        });
    });


    // ===== CLASS SELECTION WITH FLIP ANIMATION =====
    classItems.forEach(item => {
        item.addEventListener("click", () => {

            const current = document.querySelector(".active-item");
            if (current === item) return;

            const firstRects = [...document.querySelectorAll(".class-item")]
                .map(el => ({ el, rect: el.getBoundingClientRect() }));


            document.querySelectorAll(".class-item")
                .forEach(i => i.classList.remove("active-item"));

            item.classList.add("active-item");


            firstRects.forEach(({ el, rect }) => {
                const newRect = el.getBoundingClientRect();

                const dx = rect.left - newRect.left;
                const dy = rect.top - newRect.top;

                // ignore tiny noise
                if (Math.abs(dx) < 1 && Math.abs(dy) < 1) return;

                el.style.transform = `translate(${dx}px, ${dy}px)`;
                el.style.transition = "none";

                requestAnimationFrame(() => {
                    el.style.transform = "";
                    el.style.transition = "";
                });
            });

        });
    });

    window.addEventListener("resize", () => {
        moveIndicator(document.querySelector(".tab.active"));
    });

});