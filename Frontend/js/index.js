class StatBridge {
    constructor() {
        // 1. Define the debounced function first so it's ready for listeners
        this.sendToCSharp = this.debounce((statName, value) => {
            if (window.chrome?.webview) {
                window.chrome.webview.postMessage({
                    stat: statName,
                    newValue: parseInt(value) || 0
                });
            }
        }, 50);

        this.initListeners();
    }

    initListeners() {
        // Listen for typing/changes in input fields
        document.addEventListener('input', (e) => {
            const statName = e.target.dataset.statInput;
            if (statName) {
                this.sendToCSharp(statName, e.target.value);
            }
        });

        // Handle button clicks for +/-
        document.addEventListener('click', (e) => {
            const btn = e.target.closest('button');
            if (!btn || !btn.dataset.stat) return;

            const statName = btn.dataset.stat;
            const action = btn.dataset.type; // 'plus' or 'minus'
            const input = document.querySelector(`[data-stat-input="${statName}"]`);

            if (input) {
                let val = parseInt(input.value) || 0;

                // Update the value based on the button type
                if (action === 'plus') val++;
                else if (action === 'minus' && val > 1) val--;

                input.value = val;

                // Manually trigger 'input' so the listener above catches the change
                input.dispatchEvent(new Event('input', { bubbles: true }));
            }

        });
    }

    // Helper: Debounce prevents spamming C# while the user types rapidly
    debounce(func, timeout = 100) {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => {
                func.apply(this, args);
            }, timeout);
        };
    }
}

// Initialize
const bridge = new StatBridge();

// Inside your input event listener
document.querySelectorAll('[data-stat-input]').forEach(input => {
    // Store the current value as a "safe" value
    input.addEventListener('focus', (e) => {
        e.target.dataset.oldValue = e.target.value;
    });

    input.addEventListener('change', async (e) => {
        const stat = e.target.getAttribute('data-stat-input');
        const newValue = parseInt(e.target.value);
        const oldValue = parseInt(e.target.dataset.oldValue || 1);

        // Get current calculation state from the UI
        const remainingPoints = parseInt(document.querySelector('[data-display="StatusPoints"]').innerText);

        // If the user is trying to INCREASE the stat
        if (newValue > oldValue) {
            // Check if we have enough points (Simple check for immediate UI response)
            if (remainingPoints <= 0) {
                alert("Not enough status points!");
                e.target.value = oldValue; // Revert the UI
                return;
            }
        }

        // If it passes initial check, send to C# for formal validation
        window.chrome.webview.postMessage({
            type: "STAT_CHANGE",
            stat: stat,
            value: newValue
        });

        // Update the "safe" value for the next interaction
        e.target.dataset.oldValue = e.target.value;
    });
});