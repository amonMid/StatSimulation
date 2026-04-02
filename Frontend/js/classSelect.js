'use strict';

let isInternalUpdate = false;
window.isInternalUpdate = false; // Make it globally accessible for index.js

    // ── CHARACTER REGISTRY ───────────────────────────────────────
    const CHARACTERS = {
        Novice: {
            sprite: 'img/Novice_Animation.gif',
            name: 'NOVICE',
            stars: 3,
            maxJobLv: 9,
            weapons: ['Hand', 'Dagger', 'One-handed Sword', 'One-handed Axe', 'One-handed Mace', 'Two-handed Mace', 'Rod & Staff', 'Two-handed  Staff']
        },
        Swordsman: {
            sprite: 'img/Swordsman.gif',
            name: 'SWORDMAN',
            stars: 4,
            maxJobLv: 50,
            weapons: ['Hand', 'Dagger', 'One-handed sword', 'Two-handed Sword', 'One-handed Spear', 'Two-handed Spear', 'One-handed Axe', 'Two-handed Axe', 'One-handed Mace', 'Two-handed Mace']
        },
        Magician: {
            sprite: 'img/Magician.gif',
            name: 'MAGICIAN',
            stars: 4,
            maxJobLv: 50,
            weapons: ['Hand', 'Dagger', 'Rod & Staff', 'Two-handed Staff']
        },
        Archer: {
            sprite: 'img/Archer.gif',
            name: 'ARCHER',
            stars: 4,
            maxJobLv: 50,
            weapons: ['Hand', 'Dagger', 'Bow']
        },
        Acolyte: {
            sprite: 'img/Acolyte.gif',
            name: 'ACOLYTE',
            stars: 4,
            maxJobLv: 50,
            weapons: ['Hand', 'One-handed Mace', 'Two-handed Mace', 'Rod & Staff', 'Two-handed Staff']
        },
        Merchant: {
            sprite: 'img/Merchant.gif',
            name: 'MERCHANT',
            stars: 4,
            maxJobLv: 50,
            weapons: ['Hand', 'Dagger', 'One-handed Sword', 'One-handed Axe', 'Two-handed Axe', 'One-handed Mace', 'Two-handed Mace']
        },
        Thief: {
            sprite: 'img/Thief.gif',
            name: 'THIEF',
            stars: 4,
            maxJobLv: 50,
            weapons: ['Hand', 'Dagger', 'One-handed Sword', 'One-handed Axe', 'Bow']
        },
    };

// ── DOM REFS ─────────────────────────────────────────────────
const classSelect = document.querySelector('.hsr-select');
const sprite = document.querySelector('.hsr-sprite');
const namePlate = document.querySelector('.character-name-plate h2');
const starsEl = document.querySelector('.rarity-stars');
const jobLvSelect = document.querySelector('.job-level-badge select');
const weaponSelect = document.getElementById('weaponSelect');

// ── UPDATER ──────────────────────────────────────────────────
function applyCharacter(className) {
    const char = CHARACTERS[className];
    if (!char) return;

    // Sprite fade-swap
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

    // Populate weapon dropdown
    populateWeapons(char.weapons);

    // Notify C# of class change
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage({
            type: 'CLASS_CHANGE',
            ClassName: className
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

    jobLvSelect.value = currentValue <= maxLevel ? currentValue : 1;
    jobLvSelect.dispatchEvent(new Event('change'));
}

// ── POPULATE WEAPON DROPDOWN ─────────────────────────────────
function populateWeapons(weapons) {
    if (!weaponSelect) return;

    const currentValue = weaponSelect.value;
    weaponSelect.innerHTML = '';

    weapons.forEach(weapon => {
        const opt = document.createElement('option');
        opt.value = weapon.toLowerCase().replace(/\s+/g, '_');
        opt.textContent = weapon;
        weaponSelect.appendChild(opt);
    });

    // Restore previous value if still valid, otherwise default to first
    const stillValid = weapons.some(w => w.toLowerCase().replace(/\s+/g, '_') === currentValue);
    weaponSelect.value = stillValid ? currentValue : weapons[0].toLowerCase().replace(/\s+/g, '_');

    // Trigger change event
    weaponSelect.dispatchEvent(new Event('change'));
}

// ── SPRITE TRANSITION STYLE ───────────────────────────────────
sprite.style.transition = 'opacity 0.2s ease, transform 0.2s ease';

// ── INIT + LISTENERS ──────────────────────────────────────────
// REMOVED: applyCharacter(classSelect.value); 
// This prevents the page from resetting the class to "Novice" on load.
// C# will push the correct state via window.syncFromBackend.

classSelect.addEventListener('change', (e) => {
    applyCharacter(e.target.value);
});

// Job level change
if (jobLvSelect) {
    jobLvSelect.addEventListener('change', (e) => {

        if (isInternalUpdate || window.isInternalUpdate) return;

        if (window.chrome?.webview) {
            window.chrome.webview.postMessage({
                type: 'JOB_LEVEL_CHANGE',
                value: parseInt(e.target.value) || 1
            });
        }
    });
}

// ── SYNC FROM BACKEND ─────────────────────────────────────────
window.syncFromBackend = function(data) {
    if (!data.Job) return;
    
    // Prevent infinite loop if we are already on this class
    // But we still might need to update levels or weapons
    isInternalUpdate = true;
    
    const isSameJob = classSelect.value === data.Job;
    if (!isSameJob || jobLvSelect.options.length === 0) {
        classSelect.value = data.Job;
        applyCharacter(data.Job);
    }
    
    // Also set window flag to be safe
    window.isInternalUpdate = true;
    isInternalUpdate = true;
    
    if (data.JobLv && jobLvSelect.value !== String(data.JobLv)) {
        jobLvSelect.value = data.JobLv;
    }

    if (data.Weapon && weaponSelect.value !== data.Weapon) {
        // Ensure weapon exists in current dropdown
        const options = Array.from(weaponSelect.options).map(o => o.value);
        if (options.includes(data.Weapon)) {
            weaponSelect.value = data.Weapon;
            weaponSelect.dispatchEvent(new Event('change'));
        }
    }
    
    isInternalUpdate = false;
    window.isInternalUpdate = false;
};

// ── NOTIFY BACKEND READY ─────────────────────────────────────
if (window.chrome?.webview) {
    window.chrome.webview.postMessage({ type: 'PAGE_READY' });
}

// Weapon change
if (weaponSelect) {
    weaponSelect.addEventListener('change', (e) => {
        if (window.chrome?.webview) {
            window.chrome.webview.postMessage({
                type: 'WEAPON_CHANGE',
                weapon: e.target.value
            });
        }
    });
}