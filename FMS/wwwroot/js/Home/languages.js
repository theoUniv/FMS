document.addEventListener("DOMContentLoaded", async function () {
    const loadingSpinner = document.getElementById("loadingSpinner");
    const errorMessage = document.getElementById("errorMessage");
    const languagesList = document.getElementById("languagesList");
    const listGroup = languagesList.querySelector(".list-group");
    const searchInput = document.createElement("input");
    searchInput.type = "text";
    searchInput.classList.add("form-control", "mb-3");
    searchInput.placeholder = "🔍 Rechercher un langage...";
    languagesList.prepend(searchInput);
    const token = localStorage.getItem("token");

    if (!token) return;

    try {
        const [languages, userLanguages] = await Promise.all([
            fetch('/GitHub/GetLanguages', {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }
            }).then(response => {
                if (!response.ok) throw new Error("Erreur lors de la récupération des langages.");
                return response.json();
            }),
            fetch('/GitHub/GetUserLangage', {
                method: 'GET',
                headers: { 'Authorization': `Bearer ${token}` }
            }).then(response => {
                if (!response.ok) throw new Error("Erreur lors de la récupération des langages utilisateur.");
                return response.json();
            })
        ]);

        const userLanguageNames = userLanguages.map(lang => lang.nom_langage);

        loadingSpinner.classList.add("d-none");
        languagesList.classList.remove("d-none");

        function renderLanguages(filter = "") {
            listGroup.innerHTML = "";
            languages.filter(lang => lang.toLowerCase().includes(filter.toLowerCase())).forEach(lang => {
                const li = document.createElement("li");
                li.classList.add("list-group-item", "d-flex", "justify-content-between", "align-items-center", "fw-bold");

                const userHasLanguage = userLanguageNames.includes(lang);

                li.innerHTML = `
                    <span>${lang || 'Langage inconnu'}</span>
                    ${userHasLanguage ? '<span class="text-success">✔ Déjà ajouté</span>' :
                    `<button class="btn btn-sm btn-primary add-language-btn" data-language="${lang || 'Langage inconnu'}">
                            ➕ Ajouter
                        </button>`}
                `;
                listGroup.appendChild(li);
            });

            document.querySelectorAll(".add-language-btn").forEach(button => {
                button.addEventListener("click", function () {
                    const language = this.getAttribute("data-language");
                    addLanguageForUser(language, this);
                });
            });
        }

        searchInput.addEventListener("input", () => {
            renderLanguages(searchInput.value);
        });

        renderLanguages();
    } catch (error) {
        console.error(error);
        loadingSpinner.classList.add("d-none");
        errorMessage.classList.remove("d-none");
    }

    async function addLanguageForUser(language, button) {
        button.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';
        button.disabled = true;
        try {
            const response = await fetch('/GitHub/RetrieveLanguageDataForUser', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({ language })
            });

            if (!response.ok) throw new Error(`Erreur lors de l'ajout du langage ${language}.`);

            button.outerHTML = '<span class="text-success">✔ Déjà ajouté</span>';
        } catch (error) {
            console.error(error);
            alert(`❌ Impossible d'ajouter ${language}.`);
            button.innerHTML = '➕ Ajouter';
            button.disabled = false;
        }
    }
});
