
const CharacterUI = (() => {
    // Private mapping logic

    console.log("CharacterUI initialized");
    const updateElement = (key, value) => {
        console.log(`Updating element for key: ${key} with value: ${value}`);
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
        // Publicly accessible function called by C#
        render: (jsonString) => {
            try {
                const data = JSON.parse(jsonString);
                Object.keys(data).forEach(key => updateElement(key, data[key]));
            } catch (err) {
                console.error("Render Error:", err);
            }
        }
    };
})();

