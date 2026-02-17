
'use strict';

// ── CHARACTER REGISTRY ───────────────────────────────────────
const CHARACTERS = {
    Novice: { sprite: 'img/Novice.png', name: 'TRAILBLAZER', stars: 3 },
    Swordsman: { sprite: 'img/Swordman.png', name: 'BLADEHEART', stars: 4 },
    Magician: { sprite: 'img/Mage.png', name: 'STELLARUNE', stars: 4 },
    Archer: { sprite: 'img/Archer.png', name: 'VOIDSHOT', stars: 4 },
    Acolyte: { sprite: 'img/Acolyte.png', name: 'LUMENVEIL', stars: 4 },
    Merchant: { sprite: 'img/Merchant.png', name: 'GOLDMIRE', stars: 4 },
    Thief: { sprite: 'img/Thief.png', name: 'ASHSTRIDE', stars: 4 },
};

// ── DOM REFS ─────────────────────────────────────────────────
const classSelect = document.querySelector('.hsr-select');
const sprite = document.querySelector('.hsr-sprite');
const namePlate = document.querySelector('.character-name-plate h2');
const starsEl = document.querySelector('.rarity-stars');
const pathLabel = document.querySelector('.job-info .label');  

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
}

// ── SPRITE TRANSITION STYLE ───────────────────────────────────
sprite.style.transition = 'opacity 0.2s ease, transform 0.2s ease';

// ── INIT + LISTENER ──────────────────────────────────────────
applyCharacter(classSelect.value);

classSelect.addEventListener('change', (e) => {
    applyCharacter(e.target.value);
});