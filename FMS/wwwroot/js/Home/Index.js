document.addEventListener("DOMContentLoaded", function () {
    const token = localStorage.getItem("token");
    const languageSelect = document.getElementById('languageSelect');
    const languageChartCanvas = document.getElementById('languageChart');

    let languageChart;
    let allLanguagesData = [];

    if (!token) return;

    fetch('/GitHub/GetLanguageStats', {
        method: 'GET',
        headers: {'Authorization': `Bearer ${token}`}
    })
        .then(response => response.json())
        .then(data => {
            allLanguagesData = data;
            updateLanguageOptions(data);
            drawChart(data);
        })
        .catch(error => console.error("Erreur lors de la récupération des données :", error));

    function updateLanguageOptions(data) {
        languageSelect.innerHTML = '<option value="all">Tous les langages</option>';
        data.forEach(item => {
            const option = document.createElement('option');
            option.value = item.nom_langage;
            option.textContent = item.nom_langage;
            languageSelect.appendChild(option);
        });
    }

    function drawChart(data) {
        const labels = data.map(item => item.nom_langage);
        const values = data.map(item => item.nombre_repertoire);

        if (languageChart) languageChart.destroy();

        const ctx = languageChartCanvas.getContext('2d');
        languageChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Nombre de dépôts GitHub par langage',
                    data: values,
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {y: {beginAtZero: true}}
            }
        });
    }

    languageSelect.addEventListener('change', function () {
        const selectedLanguage = languageSelect.value;
        const filteredData = selectedLanguage === "all"
            ? allLanguagesData
            : allLanguagesData.filter(item => item.nom_langage === selectedLanguage);

        drawChart(filteredData);
    });

    const currentPath = window.location.pathname;


    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add("visible");
                observer.unobserve(entry.target);
            }
        });
    });
    document.querySelectorAll(".fade-in").forEach(el => observer.observe(el));

    function redirectTo(path) {
        if (window.location.pathname !== path) {
            window.location.href = path;
        }
    }

    const logoutButton = document.getElementById("logoutButton");
    if (logoutButton) {
        logoutButton.addEventListener("click", function () {
            localStorage.removeItem("token");
            redirectTo('/Home/LoginPage');
        });
    }

    if (!token) {
        if (currentPath !== "/Home/LoginPage" && currentPath !== "/Home/RegisterPage") {
            redirectTo('/Home/LoginPage');
            return;
        }
    } else {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const isTokenExpired = payload.exp * 1000 < Date.now();

        if (isTokenExpired) {
            alert("Votre session a expiré. Veuillez vous reconnecter.");
            localStorage.removeItem("token");
            redirectTo('/Home/LoginPage');
            return;
        }

        if (currentPath === "/Home/LoginPage" || currentPath === "/Home/RegisterPage") {
            redirectTo('/GitHub/Index');
            return;
        }

        const spinner = document.getElementById('loadingSpinner');
        const languageChartCanvas = document.getElementById('languageChart');
        const yearChartCanvas = document.getElementById('yearChart'); // Nouveau canvas pour les stats annuelles

        // 🔹 Récupérer les statistiques des langages GitHub
        fetch('/GitHub/GetLanguageStats', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Erreur ${response.status}: ${response.statusText}`);
                }
                return response.json();
            })
            .then(data => {
                spinner.style.display = 'none';
                languageChartCanvas.style.display = 'block';

                console.log("Données des langages reçues :", data); // Debug

                const labels = data.map(item => item.nom_langage);
                const values = data.map(item => item.nombre_repertoire);

                const ctx = languageChartCanvas.getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Nombre de dépôts GitHub par langage',
                            data: values,
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });
            })
            .catch(error => {
                console.error("Erreur lors de la récupération des données :", error);
                spinner.innerHTML = '<p class="text-danger">Erreur lors du chargement des données.</p>';
            });

        // 🔹 Récupérer les statistiques du nombre de dépôts par année
        fetch('/GitHub/GetRepoSumByYear', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Erreur ${response.status}: ${response.statusText}`);
                }
                return response.json();
            })
            .then(data => {
                yearChartCanvas.style.display = 'block';

                console.log("Données des dépôts par année reçues :", data); // Debug

                const labels = data.map(item => item.year);
                const values = data.map(item => item.nombre_repertoire);

                const ctx = yearChartCanvas.getContext('2d');

                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Nombre de dépôts GitHub par année',
                            data: values,
                            backgroundColor: 'rgba(255, 99, 132, 0.2)',
                            borderColor: 'rgba(255, 99, 132, 1)',
                            borderWidth: 2,
                            fill: true,
                            tension: 0.1
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true
                            }
                        }
                    }
                });
            })
            .catch(error => {
                console.error("Erreur lors de la récupération des données par année :", error);
            });
    }

    // 🔹 Gestion du bouton "Mettre à jour les données des langages"
    const updateLangagesButton = document.getElementById("updateLangagesButton");
    if (updateLangagesButton) {
        updateLangagesButton.addEventListener("click", function () {
            fetch('/GitHub/UpdateAllLangageData', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Erreur ${response.status}: ${response.statusText}`);
                    }
                    return response.json();
                })
                .then(data => {
                    toastr.success("Les données des langages ont été mises à jour avec succès.", "Succès");
                    location.reload();
                })
                .catch(error => {
                    console.error("Erreur lors de la mise à jour des données :", error);
                    toastr.error("Erreur lors de la mise à jour des données des langages.", "Erreur");
                });
        });
    }

    // 🔹 Gestion du bouton "Mettre à jour les données des dépôts par année"
    const updateYearlyStatsButton = document.getElementById("updateYearlyStatsButton");
    if (updateYearlyStatsButton) {
        updateYearlyStatsButton.addEventListener("click", function () {
            fetch('/GitHub/UpdateRepoSumByYear', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Erreur ${response.status}: ${response.statusText}`);
                    }
                    return response.json();
                })
                .then(data => {
                    toastr.success("Les données des dépôts par année ont été mises à jour avec succès.", "Succès");
                    location.reload();
                })
                .catch(error => {
                    console.error("Erreur lors de la mise à jour des statistiques annuelles :", error);
                    toastr.error("Erreur lors de la mise à jour des statistiques annuelles.", "Erreur");
                });
        });
    }
});
