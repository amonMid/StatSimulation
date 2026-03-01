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
            bonusEl.style.color = '#4de8ff';  // Cyan glow for job bonuses
            bonusEl.style.fontWeight = '600';
        } else {
            bonusEl.textContent = '';
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
            const stats = ['Str', 'Agi', 'Vit', 'Int', 'Dex', 'Luk'];

            stats.forEach(s => {
                const input = document.querySelector(`[data-stat-input="${s.toUpperCase()}"]`);
                if (input && input.value != (data[s] || data.BaseLv)) {
                    input.value = data[s] || data.BaseLv;
                    // Add visual feedback that the stat was forced to change
                    input.classList.add('flash-reset');
                    setTimeout(() => input.classList.remove('flash-reset'), 500);
                }
            });
        }
    };
})();