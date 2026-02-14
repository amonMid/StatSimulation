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

///**
// * Updates the UI dynamically based on JSON keys.
// * @param {string} encodedJson - Encoded for safety, should be a JSON string with keys matching data-display attributes.
// */
//function updateUiFromResult(encodedJson) {
//    try {
//        // Decode and parse
//        const data = JSON.parse(decodeURIComponent(encodedJson));

//        Object.keys(data).forEach(key => {
//            const elements = document.querySelectorAll(`[data-display="${key}"]`);

//            elements.forEach(el => {
//                // If it's a paragraph (Regen info), we might want to preserve the label
//                if (el.tagName === 'P') {
//                    const label = el.innerText.split(':')[0]; 
//                    el.innerText = `${label}: ${data[key]}`;
//                } else {
//                    // Otherwise just update the value (ATK, DEF, etc.)
//                    el.innerText = data[key];
//                }
//            });
//        });
//    } catch (e) {
//        console.error("Failed to update UI:", e);
//    }
//}