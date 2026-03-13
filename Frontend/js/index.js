class StatBridge {
    static LEVEL_MIN = 1;
    static LEVEL_MAX = 99;

    constructor() {
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
        // 1. Select all text when focusing an input
        document.addEventListener('focus', (e) => {
            if (e.target.dataset.statInput !== undefined) {
                e.target.select();
                // Store the current value as a "safe" value for your validation logic
                e.target.dataset.oldValue = e.target.value;
            }
        }, true); // Use capture phase to ensure it catches focus events

        // 2. Block special characters IMMEDIATELY while typing
        document.addEventListener('keydown', (e) => {
            const statName = e.target.dataset.statInput;
            if (!statName) return;

            if (['-', '+', 'e', '.'].includes(e.key)) {
                e.preventDefault();
            }
        });

        // 3. Only send to C# when the user LEAVES the input field
        document.addEventListener('focusout', (e) => {
            const statName = e.target.dataset.statInput;
            if (!statName) return;

            let val = parseInt(e.target.value);

            // If they left it empty or typed something invalid, snap to 1
            if (isNaN(val) || val < 1) {
                val = 1;
                e.target.value = val;
                this.sendToCSharp(statName, val);
            }
        });


        // ── Allow empty input while typing ───────────────────────────
        document.addEventListener('input', (e) => {
            const statName = e.target.dataset.statInput;
            if (!statName) return;

            if (e.target.value === '') return;

            const val = parseInt(e.target.value);

            // Level cap guard for BASELV
            if (statName === 'BASELV') {
                const clamped = this.#clampLevel(val);
                if (val !== clamped) e.target.value = clamped;
                this.sendToCSharp(statName, clamped);
                return;
            }

            this.sendToCSharp(statName, val);
        });

        // 5. Handle button clicks for +/-
        document.addEventListener('click', (e) => {
            const btn = e.target.closest('button');
            if (!btn || !btn.dataset.stat) return;

            const statName = btn.dataset.stat;
            const action = btn.dataset.type;
            const input = document.querySelector(`[data-stat-input="${statName}"]`);

            if (input) {
                let val = parseInt(input.value) || 0;
                if (action === 'plus') val++;
                else if (action === 'minus' && val > 1) val--;

                if (statName === 'BASELV') {
                    val = this.#clampLevel(val);
                }

                input.value = val;
                input.dispatchEvent(new Event('input', { bubbles: true }));
            }
        });
    }

    #clampLevel(value) {
        const n = parseInt(value) || StatBridge.LEVEL_MIN;
        return Math.min(Math.max(n, StatBridge.LEVEL_MIN), StatBridge.LEVEL_MAX);
    }

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
        const newValue = parseInt(e.target.value) || 1;
        const oldValue = parseInt(e.target.dataset.oldValue || 1);

        // ── Level cap guard (on blur / tab-out) ──────────────
        if (stat === 'BASELV') {
            //newValue = Math.min(
            //    Math.max(newValue || StatBridge.LEVEL_MIN, StatBridge.LEVEL_MIN),
            //    StatBridge.LEVEL_MAX
            //);

            const clampedValue = Math.min(Math.max(newValue, StatBridge.LEVEL_MIN), StatBridge.LEVEL_MAX);
            // If lowering level, check if we'd go negative
            if (clampedValue < oldValue) {
                console.log("Checking if level reduction is valid...");
            }

            //e.target.value = newValue;
            window.chrome.webview.postMessage({ type: 'STAT_CHANGE', stat, value: clampedValue });
            //e.target.dataset.oldValue = newValue;
            return;
        }



        // If the user is trying to INCREASE the stat
        if (newValue > oldValue) {
            // Get current calculation state from the UI
            const remainingPoints = parseInt(document.querySelector('[data-display="StatusPoints"]').innerText);

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

