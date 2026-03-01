
'use strict';

// ── CHARACTER REGISTRY ───────────────────────────────────────
// Matches C# JobRegistry exactly
const CHARACTERS = {
    Novice: { sprite: 'img/Novice.png', name: 'Novice', stars: 3, maxJobLv: 10 },
    Swordsman: { sprite: 'img/Swordman.png', name: 'Swordsman', stars: 4, maxJobLv: 50 },
    Mage: { sprite: 'img/Mage.png', name: 'Mage', stars: 4, maxJobLv: 50 },
    Archer: { sprite: 'img/Archer.png', name: 'Archer', stars: 4, maxJobLv: 50 },
    Acolyte: { sprite: 'img/Acolyte.png', name: 'Acolyte', stars: 4, maxJobLv: 50 },
    Merchant: { sprite: 'img/Merchant.png', name: 'Merchant', stars: 4, maxJobLv: 50 },
    Thief: { sprite: 'img/Thief.png', name: 'Thief', stars: 4, maxJobLv: 50 },
};

// ── DOM REFS ─────────────────────────────────────────────────
const classSelect = document.querySelector('.hsr-select');
const sprite = document.querySelector('.hsr-sprite');
const namePlate = document.querySelector('.character-name-plate h2');
const starsEl = document.querySelector('.rarity-stars');
const pathLabel = document.querySelector('.job-info .label');  
const jobLvSelect = document.querySelector('.job-level-badge select');

// ── UPDATER ──────────────────────────────────────────────────
function applyCharacter(className) {
    const char = CHARACTERS[className];
    if (!char) return;

    // Sprite — brief fade-swap 
    sprite.style.opacity = '0';
    sprite.style.transform = 'translateY(12px)';

    setTimeout(() => {
        sprite.src = char.sprite;
        sprite.alt = char.name;
        sprite.style.opacity = '1';
        sprite.style.transform = '';
    }, 200);

    // Name plate
    if (namePlate) namePlate.textContent = char.name;

    // Stars
    if (starsEl) starsEl.textContent = '★'.repeat(char.stars);

    // Populate job level dropdown
    populateJobLevels(char.maxJobLv);

    // Notify C# of class change
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage({
            type: 'CLASS_CHANGE',
            class: className
        });
    }

}

// ── POPULATE JOB LEVEL DROPDOWN ──────────────────────────────
function populateJobLevels(maxLevel) {
    if (!jobLvSelect) return;

    const currentValue = parseInt(jobLvSelect.value) || 1;
    jobLvSelect.innerHTML = '';

    for (let i = 1; i <= maxLevel; i++) {
        const opt = document.createElement('option');
        opt.value = i;
        opt.textContent = i;
        jobLvSelect.appendChild(opt);
    }

    // Restore previous value if still valid, otherwise reset to 1
    jobLvSelect.value = currentValue <= maxLevel ? currentValue : 1;

    // Trigger change event so C# gets the corrected value
    jobLvSelect.dispatchEvent(new Event('change'));
}


// ── SPRITE TRANSITION STYLE ───────────────────────────────────
sprite.style.transition = 'opacity 0.2s ease, transform 0.2s ease';

// ── INIT + LISTENER ──────────────────────────────────────────
applyCharacter(classSelect.value);

classSelect.addEventListener('change', (e) => {
    applyCharacter(e.target.value);
});

// Job level change
if (jobLvSelect) {
    jobLvSelect.addEventListener('change', (e) => {
        if (window.chrome?.webview) {
            window.chrome.webview.postMessage({
                type: 'JOB_LEVEL_CHANGE',
                value: parseInt(e.target.value) || 1
            });
        }
    });
}