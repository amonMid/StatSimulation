const CLASSES = [
    {
        key: 'Novice', label: 'NOVICE', icon: '🌱', stars: 3, maxJobLv: 255, sprite: 'img/Novice_Animation.gif',
        skills: [
            { id: 'basic_skill', name: 'Basic Skill', icon: '📖', max: 9, x: 80, y: 80, req: null, desc: 'Passive: Enables the use of Basic Interface Skills such as Trading, Kafra use, Chatroom and Party creation.' },
            { id: 'first_aid', name: 'First Aid', icon: '🩹', max: 1, x: 280, y: 80, req: null, desc: 'Supportive: Consume 3 SP to restore 5 HP. Quest skill — available from the start.' },
            { id: 'play_dead', name: 'Play Dead', icon: '💀', max: 1, x: 480, y: 80, req: null, desc: 'Supportive: Pretend to fall dead on the ground, becoming immune from all attacks from Players and monsters. Quest skill — available from the start.' },
        ]
    },
    {
        key: 'Swordman', label: 'SWORDMAN', icon: '⚔️', stars: 4, maxJobLv: 255, sprite: 'img/Swordsman.gif',
        skills: [
            { id: 'sword_mastery', name: 'Sword Mastery', icon: '🗡️', max: 10, x: 80, y: 80, req: null, desc: 'Passive: +4 ATK per level with One-Handed Sword.' },
            { id: 'two_mastery', name: 'Two-Hand Mastery', icon: '⚔️', max: 10, x: 260, y: 80, req: { id: 'sword_mastery', lv: 1 }, desc: 'Passive: +4 ATK per level with Two-Handed Sword.' },
            { id: 'bash', name: 'Bash', icon: '💥', max: 10, x: 440, y: 80, req: { id: 'sword_mastery', lv: 5 }, desc: 'Heavy strike dealing 130~200% ATK. May Stun the target.' },
            { id: 'fatal_blow', name: 'Fatal Blow', icon: '☠️', max: 10, x: 620, y: 80, req: { id: 'bash', lv: 5 }, desc: 'Passive: +2% Stun chance on Bash per level.' },
            { id: 'hp_recovery', name: 'HP Recovery', icon: '💓', max: 10, x: 80, y: 230, req: null, desc: 'Passive: Increases HP regen rate while standing.' },
            { id: 'endure', name: 'Endure', icon: '🛡️', max: 10, x: 80, y: 380, req: { id: 'hp_recovery', lv: 5 }, desc: 'Ignore knockback and interruption for 10s. Adds MDEF.' },
            { id: 'provoke', name: 'Provoke', icon: '😤', max: 10, x: 260, y: 230, req: { id: 'sword_mastery', lv: 5 }, desc: 'Taunts enemy: −DEF but +ATK. Forces them to target you.' },
            { id: 'auto_berserk', name: 'Auto Berserk', icon: '😡', max: 1, x: 260, y: 380, req: { id: 'provoke', lv: 5 }, desc: 'Passive: Provoke self at Lv1 when HP drops below 25%.' },
            { id: 'magnum_break', name: 'Magnum Break', icon: '🔥', max: 10, x: 440, y: 230, req: { id: 'bash', lv: 3 }, desc: 'AoE fire attack: 100~200% fire damage, knocks back enemies.' },
        ]
    },
    {
        key: 'Mage', label: 'MAGICIAN', icon: '🔮', stars: 4, maxJobLv: 255, sprite: 'img/Magician.gif',
        skills: [
            { id: 'sp_recovery', name: 'SP Recovery', icon: '💧', max: 10, x: 80, y: 80, req: null, desc: 'Passive: Increases SP regen rate while standing still.' },
            { id: 'soul_strike', name: 'Soul Strike', icon: '👻', max: 10, x: 80, y: 230, req: { id: 'sp_recovery', lv: 3 }, desc: '1~5 bolts of spiritual energy. Very effective on Undead.' },
            { id: 'napalmbeat', name: 'Napalm Beat', icon: '💣', max: 10, x: 80, y: 380, req: { id: 'soul_strike', lv: 4 }, desc: 'Psychic explosion hitting all in 3x3 area around target.' },
            { id: 'safety_wall', name: 'Safety Wall', icon: '🧱', max: 10, x: 80, y: 520, req: { id: 'napalmbeat', lv: 7 }, desc: 'Magic barrier on a cell that blocks 1~11 melee attacks.' },
            { id: 'cold_bolt', name: 'Cold Bolt', icon: '❄️', max: 10, x: 260, y: 80, req: { id: 'sp_recovery', lv: 1 }, desc: '1~10 ice bolts. Higher levels chance to Freeze the target.' },
            { id: 'frost_diver', name: 'Frost Diver', icon: '🌊', max: 10, x: 440, y: 80, req: { id: 'cold_bolt', lv: 5 }, desc: 'Freezes the target solid. Level improves success rate.' },
            { id: 'fire_bolt', name: 'Fire Bolt', icon: '🔥', max: 10, x: 260, y: 230, req: { id: 'sp_recovery', lv: 1 }, desc: '1~10 fire bolts. Most efficient damage spell for Mages.' },
            { id: 'fire_ball', name: 'Fire Ball', icon: '🌋', max: 10, x: 440, y: 230, req: { id: 'fire_bolt', lv: 4 }, desc: 'Fire ball dealing AoE damage in 3x3 area around target.' },
            { id: 'fire_wall', name: 'Fire Wall', icon: '🧯', max: 10, x: 620, y: 230, req: { id: 'fire_ball', lv: 7 }, desc: 'Wall of fire that repeatedly burns enemies passing through.' },
            { id: 'lightning_bolt', name: 'Lightning Bolt', icon: '⚡', max: 10, x: 260, y: 380, req: { id: 'sp_recovery', lv: 1 }, desc: '1~10 lightning bolts strike the target.' },
            { id: 'thunderstorm', name: 'Thunderstorm', icon: '🌩️', max: 10, x: 440, y: 380, req: { id: 'lightning_bolt', lv: 4 }, desc: 'Repeatedly strikes an area with lightning for 5 seconds.' },
        ]
    },
    {
        key: 'Archer', label: 'ARCHER', icon: '🏹', stars: 4, maxJobLv: 255, sprite: 'img/Archer.gif',
        skills: [
            { id: 'owl_eye', name: "Owl's Eye", icon: '👁️', max: 10, x: 80, y: 80, req: null, desc: 'Passive: +1 DEX per level, improves hit and ranged damage.' },
            { id: 'vulture_eye', name: "Vulture's Eye", icon: '🦅', max: 10, x: 260, y: 80, req: { id: 'owl_eye', lv: 3 }, desc: 'Passive: +1 attack range per level with bows.' },
            { id: 'improve_conc', name: 'Improve Concentration', icon: '🎯', max: 10, x: 80, y: 230, req: { id: 'owl_eye', lv: 5 }, desc: 'Temporarily increases DEX and AGI. Reveals nearby hidden foes.' },
            { id: 'arrow_shower', name: 'Arrow Shower', icon: '🌧️', max: 10, x: 440, y: 80, req: { id: 'vulture_eye', lv: 5 }, desc: 'Volley of arrows hitting all enemies in a 5x5 area.' },
            { id: 'double_strafe', name: 'Double Strafe', icon: '✨', max: 10, x: 620, y: 80, req: { id: 'arrow_shower', lv: 5 }, desc: 'Two rapid arrows dealing 180~360% total ranged damage.' },
            { id: 'arrow_crafting', name: 'Arrow Crafting', icon: '✏️', max: 1, x: 80, y: 380, req: { id: 'improve_conc', lv: 3 }, desc: 'Craft various arrow types from looted monster items.' },
            { id: 'charge_arrow', name: 'Charge Arrow', icon: '🏹', max: 1, x: 260, y: 380, req: { id: 'arrow_crafting', lv: 1 }, desc: 'Charged arrow that pushes the target back 6 cells.' },
        ]
    },
    {
        key: 'Merchant', label: 'MERCHANT', icon: '💰', stars: 4, maxJobLv: 255, sprite: 'img/Merchant.gif',
        skills: [
            { id: 'enlarge_weight', name: 'Enlarge Weight', icon: '🏋️', max: 10, x: 80, y: 80, req: null, desc: 'Passive: +200 max carry weight per level.' },
            { id: 'overcharge', name: 'Overcharge', icon: '💸', max: 10, x: 80, y: 230, req: { id: 'enlarge_weight', lv: 3 }, desc: 'Passive: Sell items to NPCs at +2% price per level.' },
            { id: 'discount', name: 'Discount', icon: '🏷️', max: 10, x: 80, y: 380, req: { id: 'enlarge_weight', lv: 3 }, desc: 'Passive: Buy from NPCs at −2% price per level.' },
            { id: 'pushcart', name: 'Pushcart', icon: '🛒', max: 10, x: 260, y: 80, req: { id: 'enlarge_weight', lv: 1 }, desc: 'Enables pushcart with 8000 extra weight capacity.' },
            { id: 'vending', name: 'Vending', icon: '🏪', max: 10, x: 440, y: 80, req: { id: 'pushcart', lv: 3 }, desc: 'Set up a personal shop with up to 3~12 item slots.' },
            { id: 'mammonite', name: 'Mammonite', icon: '💰', max: 10, x: 260, y: 230, req: { id: 'overcharge', lv: 5 }, desc: 'Spend Zeny to deal 150~600% ATK in a powerful strike.' },
            { id: 'cart_attack', name: 'Cart Revolution', icon: '💥', max: 1, x: 440, y: 230, req: { id: 'pushcart', lv: 5 }, desc: 'Ram the cart into enemies for AoE damage scaled by weight.' },
            { id: 'trade', name: 'Trade', icon: '🤝', max: 1, x: 260, y: 380, req: { id: 'discount', lv: 1 }, desc: 'Allows direct trading of items and Zeny with other players.' },
        ]
    },
    {
        key: 'Thief', label: 'THIEF', icon: '🗡️', stars: 4, maxJobLv: 255, sprite: 'img/Thief.gif',
        skills: [
            { id: 'double_attack', name: 'Double Attack', icon: '⚡', max: 10, x: 80, y: 80, req: null, desc: 'Passive: 5~50% chance to strike twice with daggers.' },
            { id: 'steal', name: 'Steal', icon: '👜', max: 10, x: 260, y: 80, req: { id: 'double_attack', lv: 1 }, desc: 'Attempt to steal an item from a monster without killing it.' },
            { id: 'find_stone', name: 'Find Stone', icon: '🪨', max: 1, x: 440, y: 80, req: { id: 'steal', lv: 5 }, desc: 'Search the ground for throwable stones (max 200).' },
            { id: 'stone_throw', name: 'Stone Throw', icon: '🌪️', max: 1, x: 620, y: 80, req: { id: 'find_stone', lv: 1 }, desc: 'Throw a stone at range dealing 50 fixed damage.' },
            { id: 'hiding', name: 'Hiding', icon: '👤', max: 10, x: 80, y: 230, req: { id: 'double_attack', lv: 5 }, desc: 'Conceal yourself from view. Duration increases per level.' },
            { id: 'sand_attack', name: 'Sand Attack', icon: '🏜️', max: 10, x: 80, y: 380, req: { id: 'hiding', lv: 5 }, desc: 'Throws sand causing Blind status and Earth element change.' },
            { id: 'backslide', name: 'Backslide', icon: '💨', max: 10, x: 260, y: 380, req: { id: 'hiding', lv: 5 }, desc: 'Instantly dash 5 cells backward without turning around.' },
            { id: 'envenom', name: 'Envenom', icon: '☠️', max: 10, x: 260, y: 230, req: { id: 'steal', lv: 5 }, desc: 'Poison-coated attack with extra damage and chance to Poison.' },
            { id: 'detoxify', name: 'Detoxify', icon: '🧪', max: 1, x: 440, y: 230, req: { id: 'envenom', lv: 1 }, desc: 'Removes Poison status from yourself or an ally.' },
        ]
    },
    {
        key: 'Acolyte', label: 'ACOLYTE', icon: '✨', stars: 4, maxJobLv: 255, sprite: 'img/Acolyte.gif',
        skills: [
            { id: 'heal', name: 'Heal', icon: '💊', max: 10, x: 80, y: 80, req: null, desc: 'Restores HP to a target. Scales with INT. Damages Undead.' },
            { id: 'blessing', name: 'Blessing', icon: '🙏', max: 10, x: 80, y: 230, req: { id: 'heal', lv: 5 }, desc: 'Increases STR, INT, and DEX of a target for 3~10 minutes.' },
            { id: 'divine_prot', name: 'Divine Protection', icon: '🛡️', max: 10, x: 80, y: 380, req: { id: 'blessing', lv: 5 }, desc: 'Passive: +DEF against Undead and Demon type monsters.' },
            { id: 'demon_bane', name: 'Demon Bane', icon: '✝️', max: 10, x: 260, y: 380, req: { id: 'divine_prot', lv: 3 }, desc: 'Passive: +ATK against Undead and Demon type monsters.' },
            { id: 'increase_agi', name: 'Increase Agility', icon: '🌀', max: 10, x: 260, y: 80, req: { id: 'heal', lv: 3 }, desc: 'Increases AGI and movement speed of a target.' },
            { id: 'decrease_agi', name: 'Decrease Agility', icon: '🐌', max: 10, x: 260, y: 230, req: { id: 'heal', lv: 3 }, desc: 'Reduces AGI and movement speed of an enemy target.' },
            { id: 'angelus', name: 'Angelus', icon: '😇', max: 10, x: 440, y: 80, req: { id: 'increase_agi', lv: 3 }, desc: 'Increases VIT DEF and MDEF of all party members nearby.' },
            { id: 'ruwach', name: 'Ruwach', icon: '👁️', max: 1, x: 440, y: 230, req: { id: 'decrease_agi', lv: 1 }, desc: 'Reveals hidden and cloaked enemies in a 5x5 area.' },
            { id: 'pneuma', name: 'Pneuma', icon: '🌸', max: 1, x: 620, y: 230, req: { id: 'ruwach', lv: 1 }, desc: 'Cloud on a cell blocking ALL ranged physical attacks for 10s.' },
            { id: 'teleport', name: 'Teleport', icon: '🌟', max: 2, x: 440, y: 380, req: { id: 'ruwach', lv: 1 }, desc: 'Lv1: random warp. Lv2: warp to save point.' },
        ]
    },
];

// ── CAROUSEL ─────────────────────────────────────────────────
let carouselIdx = 0, carouselBusy = false;

const spriteTrack = document.getElementById('spriteTrack');
const dotsRow = document.getElementById('dotsRow');
const quickGrid = document.getElementById('quickGrid');

const slides = CLASSES.map((c, i) => {
    const div = document.createElement('div');
    div.className = 'sprite-slide' + (i === 0 ? ' s-current' : ' s-hidden');
    const img = document.createElement('img');
    img.className = 'cls-sprite'; img.alt = c.label; img.src = c.sprite;
    img.onerror = () => { const em = document.createElement('span'); em.className = 'sprite-emoji'; em.textContent = c.icon; div.replaceChild(em, img); };
    div.appendChild(img); spriteTrack.appendChild(div); return div;
});

CLASSES.forEach((_, i) => {
    const d = document.createElement('div');
    d.className = 'dot' + (i === 0 ? ' active' : '');
    d.onclick = () => goTo(i);
    dotsRow.appendChild(d);
});

CLASSES.forEach((c, i) => {
    const b = document.createElement('button');
    b.className = 'quick-btn' + (i === 0 ? ' active' : '');
    b.innerHTML = `<img class="qb-gif" src="${c.sprite}" alt="${c.label}" onerror="this.outerHTML='<span class=\\"qb-icon\\">${c.icon}</span><span class="qb-name">${c.label}</span>`;
    b.onclick = () => goTo(i);
    quickGrid.appendChild(b);
});

function slideRole(idx, cur, n) {
    const d = ((idx - cur) % n + n) % n;
    return d === 0 ? 's-current' : d === 1 ? 's-next' : d === n - 1 ? 's-prev' : d === 2 ? 's-far-next' : d === n - 2 ? 's-far-prev' : 's-hidden';
}

function goTo(idx) {
    if (carouselBusy || idx === carouselIdx) return;
    carouselBusy = true;
    carouselIdx = ((idx % CLASSES.length) + CLASSES.length) % CLASSES.length;
    slides.forEach((s, i) => { s.className = 'sprite-slide ' + slideRole(i, carouselIdx, CLASSES.length); });
    const c = CLASSES[carouselIdx];
    const plate = document.getElementById('namePlate');
    plate.classList.add('fading');
    setTimeout(() => {
        document.getElementById('className').textContent = c.label;
        document.getElementById('classStars').textContent = '★'.repeat(c.stars);
        plate.classList.remove('fading');
    }, 160);
    document.querySelectorAll('.dot').forEach((d, i) => d.classList.toggle('active', i === carouselIdx));
    document.querySelectorAll('.quick-btn').forEach((b, i) => b.classList.toggle('active', i === carouselIdx));
    currentClass = c.key; learned = {}; jobLevel = 1; fullRender();
    setTimeout(() => { carouselBusy = false; }, 420);
}

document.getElementById('btnPrev').onclick = () => goTo((carouselIdx - 1 + CLASSES.length) % CLASSES.length);
document.getElementById('btnNext').onclick = () => goTo((carouselIdx + 1) % CLASSES.length);
document.addEventListener('keydown', e => { if (e.key === 'ArrowRight') goTo((carouselIdx + 1) % CLASSES.length); if (e.key === 'ArrowLeft') goTo((carouselIdx - 1 + CLASSES.length) % CLASSES.length); });
let tx0 = null;
spriteTrack.addEventListener('touchstart', e => { tx0 = e.touches[0].clientX; }, { passive: true });
spriteTrack.addEventListener('touchend', e => { if (tx0 === null) return; const dx = e.changedTouches[0].clientX - tx0; if (Math.abs(dx) > 40) dx < 0 ? goTo((carouselIdx + 1) % CLASSES.length) : goTo((carouselIdx - 1 + CLASSES.length) % CLASSES.length); tx0 = null; });

// ── SKILL TREE ────────────────────────────────────────────────
let currentClass = 'Novice', jobLevel = 1, learned = {};
let panX = 60, panY = 20, zoom = 1, dragging = false, dSX, dSY, pSX, pSY;

const clsData = () => CLASSES.find(c => c.key === currentClass);
const pts = () => Math.max(0, jobLevel - 1);
const spent = () => Object.values(learned).reduce((a, b) => a + b, 0);
const rem = () => Math.max(0, pts() - spent());
const meetsReq = sk => !sk.req || (learned[sk.req.id] || 0) >= sk.req.lv;
const getState = sk => { const lv = learned[sk.id] || 0; if (lv >= sk.max) return 'maxed'; if (!meetsReq(sk)) return 'locked'; if (lv > 0) return 'learned'; return 'available'; };
const findSk = id => clsData().skills.find(s => s.id === id);

// Build ordered list of prerequisite chain needed to learn a skill
function getPrereqChain(id) {
    const chain = [];
    function walk(skillId) {
        const s = findSk(skillId);
        if (!s || !s.req) return;
        walk(s.req.id); // depth-first so root comes first
        const reqSk = findSk(s.req.id);
        if (reqSk && (learned[reqSk.id] || 0) < s.req.lv) {
            chain.push({ id: reqSk.id, targetLv: s.req.lv });
        }
    }
    walk(id);
    return chain;
}

// Calculate total SP needed to auto-fulfil prereqs + the skill itself
function spNeededToLearn(id) {
    const chain = getPrereqChain(id);
    let cost = 0;
    const sim = Object.assign({}, learned);
    for (const { id: pid, targetLv } of chain) {
        const cur = sim[pid] || 0;
        cost += targetLv - cur;
        sim[pid] = targetLv;
    }
    const finalLv = sim[id] || 0;
    if (finalLv < (findSk(id)?.max || 0)) cost += 1;
    return cost;
}

function increase(id) {
    const s = findSk(id); if (!s) return;
    const lv = learned[id] || 0;
    if (lv >= s.max) return;

    if (meetsReq(s)) {
        // Normal case — just spend 1 SP
        if (rem() <= 0) return;
        learned[id] = lv + 1;
    } else {
        // Auto-fulfil prerequisites chain
        const needed = spNeededToLearn(id);
        if (needed > rem()) return; // can't afford even with auto-fill

        const chain = getPrereqChain(id);
        for (const { id: pid, targetLv } of chain) {
            learned[pid] = targetLv;
        }
        // Now learn the skill itself (1 SP)
        learned[id] = (learned[id] || 0) + 1;
    }
    fullRender();
}

// Right-click on a node: reduce level by 1, cascade-unlearn dependents if needed
function decrease(id) {
    const lv = learned[id] || 0; if (lv <= 0) return;
    const newLv = lv - 1;
    if (newLv === 0) {
        delete learned[id];
    } else {
        learned[id] = newLv;
    }
    // Cascade: unlearn any skill whose req on THIS skill is now unmet
    cascadeUnlearn(id, newLv);
    fullRender();
}

// Right-click on a CHIP (learned list): same reduce behaviour
function decreaseFromChip(id) { decrease(id); }

function cascadeUnlearn(changedId, newLv) {
    for (const s of clsData().skills) {
        if (s.req && s.req.id === changedId && (learned[s.id] || 0) > 0) {
            if (newLv < s.req.lv) {
                delete learned[s.id];
                cascadeUnlearn(s.id, 0);
            }
        }
    }
}

function renderStats() {
    document.getElementById('remainPts').textContent = rem();
    document.getElementById('learnedCount').textContent = Object.keys(learned).length;
    document.getElementById('spentPts').textContent = spent();
    document.getElementById('jobLvVal').textContent = jobLevel;

    document.getElementById('jobLvMax').textContent = '/ 255';
    const sl = document.getElementById('jobLvSlider'); sl.max = 255; sl.value = jobLevel;
}

function renderLearnedList() {
    const list = document.getElementById('learnedList');
    const ls = clsData().skills.filter(s => (learned[s.id] || 0) > 0);
    if (!ls.length) { list.innerHTML = '<div class="empty-msg">No skills learned yet.<br>Click a skill node to begin.</div>'; return; }
    list.innerHTML = '';
    ls.forEach(s => {
        const lv = learned[s.id];
        const d = document.createElement('div'); d.className = 'skill-chip';
        d.title = 'Left-click: +level | Right-click: −1 level';
        d.innerHTML = `<div class="sc-icon">${s.icon}</div><div class="sc-info"><div class="sc-name">${s.name}</div><div class="sc-sub">${lv >= s.max ? '★ Maxed' : 'Learned'}</div></div><div class="sc-lv">Lv ${lv}</div>`;
        d.onclick = () => increase(s.id);
        d.oncontextmenu = e => { e.preventDefault(); decreaseFromChip(s.id); };
        list.appendChild(d);
    });
}

function renderTree() {
    const inner = document.getElementById('canvasInner');
    const svg = document.getElementById('connSvg');
    inner.querySelectorAll('.skill-node').forEach(el => el.remove());
    svg.innerHTML = '';
    const skills = clsData().skills;

    // connector paths
    skills.forEach(sk => {
        if (!sk.req) return;
        const from = skills.find(s => s.id === sk.req.id); if (!from) return;
        const H = 33, x1 = from.x + H, y1 = from.y + H, x2 = sk.x + H, y2 = sk.y + H;
        const isLearned = (learned[sk.id] || 0) > 0;
        const isMet = (learned[from.id] || 0) >= sk.req.lv;
        const mx = (x1 + x2) / 2;

        const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        path.setAttribute('d', `M${x1},${y1} C${mx},${y1} ${mx},${y2} ${x2},${y2}`);
        path.setAttribute('fill', 'none');
        path.setAttribute('stroke', isLearned ? '#d4a73a' : isMet ? '#8b6f47' : '#c8b89a');
        path.setAttribute('stroke-width', isLearned ? '3' : '2');

        svg.appendChild(path);

        // req badge on connector
        const bg = document.createElementNS('http://www.w3.org/2000/svg', 'rect');
        const midX = (x1 + x2) / 2, midY = (y1 + y2) / 2;
        bg.setAttribute('x', midX - 14); bg.setAttribute('y', midY - 12);
        bg.setAttribute('width', 28); bg.setAttribute('height', 16);
        bg.setAttribute('rx', 3); bg.setAttribute('fill', isMet ? '#dcc896' : '#e8dcc8');
        bg.setAttribute('stroke', isMet ? '#8b6f47' : '#b8a888'); bg.setAttribute('stroke-width', '1.5');
        svg.appendChild(bg);

        const txt = document.createElementNS('http://www.w3.org/2000/svg', 'text');
        txt.setAttribute('x', midX); txt.setAttribute('y', midY - 1);
        txt.setAttribute('text-anchor', 'middle'); txt.setAttribute('dominant-baseline', 'middle');
        txt.setAttribute('font-size', '9'); txt.setAttribute('font-weight', '700');
        txt.setAttribute('fill', isMet ? '#5d4a30' : '#9a8868');
        txt.setAttribute('font-family', 'Noto Sans, sans-serif');
        txt.textContent = 'Lv' + sk.req.lv;
        svg.appendChild(txt);
    });

    const maxX = Math.max(...skills.map(s => s.x)) + 160;
    const maxY = Math.max(...skills.map(s => s.y)) + 160;
    svg.setAttribute('width', maxX); svg.setAttribute('height', maxY);

    skills.forEach(sk => {
        const lv = learned[sk.id] || 0;
        const state = getState(sk);
        const fc = state === 'maxed' ? 'maxed' : state === 'learned' ? 'learned' : state === 'available' ? 'available' : 'locked';
        const node = document.createElement('div');
        node.className = 'skill-node';
        node.style.left = sk.x + 'px'; node.style.top = sk.y + 'px';
        node.innerHTML = `<div class="node-frame ${fc}">${sk.icon}<div class="node-badge ${lv > 0 ? 'on' : ''}">${lv}/${sk.max}</div></div><div class="node-name ${lv > 0 ? 'on' : ''}">${sk.name}</div>`;
        node.onclick = e => { e.stopPropagation(); increase(sk.id); };
        node.oncontextmenu = e => { e.preventDefault(); decrease(sk.id); };
        node.onmouseenter = e => showTT(e, sk);
        node.onmousemove = moveTT;
        node.onmouseleave = hideTT;
        inner.appendChild(node);
    });
}

const ttEl = document.getElementById('tooltip');
function showTT(e, sk) {
    const lv = learned[sk.id] || 0, state = getState(sk);
    document.getElementById('ttIcon').textContent = sk.icon;
    document.getElementById('ttName').textContent = sk.name;
    document.getElementById('ttMaxLv').textContent = 'MAX LV ' + sk.max;
    document.getElementById('ttDesc').textContent = sk.desc;
    document.getElementById('ttCurLv').textContent = lv + ' / ' + sk.max;
    const req = document.getElementById('ttReq');
    if (sk.req) { const rs = findSk(sk.req.id); const met = meetsReq(sk); req.className = 'tt-rv ' + (met ? 'ok' : 'warn'); req.textContent = (rs ? rs.name : sk.req.id) + ' Lv' + sk.req.lv; }
    else { req.className = 'tt-rv ok'; req.textContent = 'None'; }
    const st = document.getElementById('ttStatus');
    st.textContent = { learned: '✦ Learned', available: 'Click to Learn', locked: 'Requirements Not Met', maxed: '★ Maxed Out' }[state];
    st.className = 'tt-status ' + (state === 'maxed' ? 'maxed' : state);
    document.getElementById('ttHint').textContent = (state === 'learned' || state === 'maxed') ? 'Right-click node to reduce by 1 level' : state === 'available' ? (rem() <= 0 ? 'Not enough skill points' : 'Left-click — will auto-fulfill prerequisites') : 'Left-click to auto-fulfill prerequisites (if SP allows)';
    moveTT(e); ttEl.classList.add('show');
}
function moveTT(e) {
    let x = e.clientX + 18, y = e.clientY + 18;
    if (x + 280 > window.innerWidth) x = e.clientX - 286;
    if (y + 370 > window.innerHeight) y = e.clientY - 370;
    ttEl.style.left = x + 'px'; ttEl.style.top = y + 'px';
}
function hideTT() { ttEl.classList.remove('show'); }

function applyTransform() { document.getElementById('canvasInner').style.transform = `translate(${panX}px,${panY}px) scale(${zoom})`; }
function zoomBy(d) { zoom = Math.min(2.5, Math.max(0.2, zoom + d)); applyTransform(); }

document.getElementById('zoomIn').onclick = () => zoomBy(0.15);
document.getElementById('zoomOut').onclick = () => zoomBy(-0.15);
document.getElementById('zoomFit').onclick = () => { panX = 60; panY = 20; zoom = 1; applyTransform(); };
document.getElementById('resetBtn').onclick = () => { learned = {}; fullRender(); };

const panel = document.getElementById('treePanel');
panel.addEventListener('mousedown', e => { if (e.target.closest('.skill-node,.zoom-btns,.tree-topbar')) return; dragging = true; dSX = e.clientX; dSY = e.clientY; pSX = panX; pSY = panY; });
window.addEventListener('mousemove', e => { if (!dragging) return; panX = pSX + (e.clientX - dSX); panY = pSY + (e.clientY - dSY); applyTransform(); });
window.addEventListener('mouseup', () => { dragging = false; });
panel.addEventListener('wheel', e => { e.preventDefault(); zoomBy(e.deltaY > 0 ? -0.08 : 0.08); }, { passive: false });
document.getElementById('jobLvSlider').addEventListener('input', e => { jobLevel = parseInt(e.target.value); renderStats(); renderTree(); renderLearnedList(); });

function fullRender() { renderStats(); renderTree(); renderLearnedList(); applyTransform(); }

// init
slides.forEach((s, i) => { s.className = 'sprite-slide ' + slideRole(i, 0, CLASSES.length); });
fullRender();