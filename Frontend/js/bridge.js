const CharacterUI = (() => {
    // Private helper to update individual text values
    const updateElement = (key, value) => {
        const targets = document.querySelectorAll(`[data-display="${key}"]`);

        targets.forEach(el => {
            if (el.tagName === 'P') {
                const label = el.innerText.split(':')[0];
                el.innerText = `${label}: ${value}`;
            } else {
                el.innerText = value;
            }
        });
    };

    return {
        render: (jsonString) => {
            try {
                const data = JSON.parse(jsonString);

                // Update all text/combat displays
                Object.keys(data).forEach(key => updateElement(key, data[key]));

                // Point Restriction Logic (Run once per render)
                const remainingPoints = data.StatusPoints;
                const stats = ['Str', 'Agi', 'Vit', 'Int', 'Dex', 'Luk'];

                stats.forEach(stat => {
                    const upperStat = stat.toUpperCase();
                    // Get the cost from C# property, e.g., data["NextStrCost"]
                    const cost = data[`Next${stat}Cost`];

                    const plusBtn = document.querySelector(`.btn-plus[data-stat="${upperStat}"]`);

                    if (plusBtn) {
                        // DISABLE if cost is higher than what we have
                        if (remainingPoints < cost) {
                            plusBtn.disabled = true;
                            plusBtn.style.opacity = "0.5";
                            plusBtn.style.cursor = "not-allowed";
                        } else {
                            plusBtn.disabled = false;
                            plusBtn.style.opacity = "1";
                            plusBtn.style.cursor = "pointer";
                        }
                    }
                });

                // Visual warning for Status Points
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
            // data contains the current valid stats from C#
            const stats = ['Str', 'Agi', 'Vit', 'Int', 'Dex', 'Luk'];
            stats.forEach(s => {
                const input = document.querySelector(`[data-stat-input="${s.toUpperCase()}"]`);
                if (input) {
                    // This sets the input value to the C# value (ignoring the invalid typed value)
                    input.value = data[s] || input.value;
                }
            });
        }

    };
})();