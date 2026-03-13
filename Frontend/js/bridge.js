if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        // C# sends a JSON string or object
        const resultData = event.data;
        const jsonString = typeof resultData === 'string' ? resultData : JSON.stringify(resultData);

        // 1. Update all the labels, bonuses, and button states
        CharacterUI.render(jsonString);

        // 2. Force the <input> fields to match the C# values 
        // This is what reverts the text if the user overspent!
        CharacterUI.syncInputs(jsonString);
    });
}


const CharacterUI = (() => {
    // ── Private helper to update individual text values ──────
    const updateElement = (key, value) => {
        const targets = document.querySelectorAll(`[data-display="${key}"]`);
        console.log(`Updating ${key} to ${value} on ${targets.length} elements`);
        targets.forEach(el => {
            if (el.tagName === 'P') {
                const label = el.innerText.split(':')[0];
                el.innerText = `${label}: ${value}`;
            } else {
                el.innerText = value;
            }
        });
    };

    // ── Update stat bonus display ────────────────────────────
    const updateStatBonus = (stat, bonus) => {
        const bonusEl = document.querySelector(`[data-stat-bonus="${stat}"]`);
        if (!bonusEl) return;

        if (bonus > 0) {
            bonusEl.textContent = `+${bonus}`;
            bonusEl.style.color = '#5fb05f'; 
            bonusEl.style.fontWeight = '600';
            bonusEl.style.display = 'inline'; 
        } else {
            bonusEl.textContent = '0';
        }
    };

    return {
        render: (jsonString) => {
            try {
                const data = JSON.parse(jsonString);



                // Update all text/combat displays
                Object.keys(data).forEach(key => updateElement(key, data[key]));

                // ── Display job bonuses ───────────────────────────
                updateStatBonus('STR', data.BonusStr || 0);
                updateStatBonus('AGI', data.BonusAgi || 0);
                updateStatBonus('VIT', data.BonusVit || 0);
                updateStatBonus('INT', data.BonusInt || 0);
                updateStatBonus('DEX', data.BonusDex || 0);
                updateStatBonus('LUK', data.BonusLuk || 0);


                // ── Point Restriction Logic ───────────────────────
                const remainingPoints = data.StatusPoints;
                const stats = ['Str', 'Agi', 'Vit', 'Int', 'Dex', 'Luk'];

                stats.forEach(stat => {
                    const upperStat = stat.toUpperCase();
                    const cost = data[`Next${stat}Cost`];
                    const plusBtn = document.querySelector(`.btn-plus[data-stat="${upperStat}"]`);

                    if (plusBtn) {
                        const disabled = remainingPoints < cost;
                        plusBtn.disabled = disabled;
                        plusBtn.style.opacity = disabled ? '0.5' : '1';
                        plusBtn.style.cursor = disabled ? 'not-allowed' : 'pointer';
                    }
                });

                // ── Level cap: disable + button at max ───────────
                const lvInput = document.querySelector('[data-stat-input="BASELV"]');
                const lvPlusBtn = document.querySelector('.btn-plus[data-stat="BASELV"]');
                if (lvInput && lvPlusBtn) {
                    const atMax = parseInt(lvInput.value) >= 99;
                    lvPlusBtn.disabled = atMax;
                    lvPlusBtn.style.opacity = atMax ? '0.5' : '1';
                    lvPlusBtn.style.cursor = atMax ? 'not-allowed' : 'pointer';
                }

                // ── Visual warning for Status Points ──────────────
                const pointsDisplay = document.querySelector('[data-display="StatusPoints"]');
                if (pointsDisplay) {
                    pointsDisplay.style.color = remainingPoints <= 0 ? '#ff4d4d' : 'inherit';
                }

            } catch (err) {
                console.error("Render Error:", err);
            }
        },

        syncInputs: (jsonString) => {
            const data = JSON.parse(jsonString);
            // Include all fields that have inputs
            const fieldMap = {
                'STR': data.Str,
                'AGI': data.Agi,
                'VIT': data.Vit,
                'INT': data.Int,
                'DEX': data.Dex,
                'LUK': data.Luk,
                'BASELV': data.BaseLv,
                'JOBLV': data.JobLv
            };

            Object.keys(fieldMap).forEach(key => {
                const input = document.querySelector(`[data-stat-input="${key}"]`);
                const backendValue = fieldMap[key];

                if (input && backendValue !== undefined) {
                    // Only update and flash if the UI is out of sync with C#
                    if (parseInt(input.value) !== backendValue) {
                        input.value = backendValue;

                        // Visual feedback
                        input.classList.add('flash-reset');
                        setTimeout(() => input.classList.remove('flash-reset'), 500);
                    }

                    // Also update the 'oldValue' used by your StatBridge logic
                    input.dataset.oldValue = backendValue;
                }
            });
        }
    };
})();