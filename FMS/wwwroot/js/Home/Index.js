document.addEventListener("DOMContentLoaded", function () {
    const token = localStorage.getItem("token");
    const currentPath = window.location.pathname;

    // Fonction pour rediriger vers une page spécifique
    function redirectTo(path) {
        if (window.location.pathname !== path) {
            window.location.href = path;
        }
    }

    // Gestion de la déconnexion
    const logoutButton = document.getElementById("logoutButton");
    if (logoutButton) {
        logoutButton.addEventListener("click", function () {
            localStorage.removeItem("token"); // Supprimer le token
            redirectTo('/Home/LoginPage'); // Rediriger vers la page de connexion
        });
    }

    // Vérification du token
    if (!token) {
        if (currentPath !== "/Home/LoginPage" && currentPath !== "/Home/RegisterPage") {
            redirectTo('/Home/LoginPage');
            return;
        }
    } else {
        const payload = JSON.parse(atob(token.split('.')[1])); // Décoder le token
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
        const chartCanvas = document.getElementById('languageChart');

        // Récupérer les stats des langages GitHub
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
                chartCanvas.style.display = 'block';

                const labels = Object.keys(data);
                const values = Object.values(data);

                const ctx = chartCanvas.getContext('2d');
                new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: labels,
                        datasets: [{
                            label: 'Nombre de dépôts GitHub',
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
    }

    // Gestion du bouton "Mettre à jour les données des langages"
    const updateLangagesButton = document.getElementById("updateLangagesButton");
    if (updateLangagesButton) {
        updateLangagesButton.addEventListener("click", function () {
            // Afficher le spinner pendant la mise à jour
            //spinner.style.display = 'block';
            //chartCanvas.style.display = 'none';

            // Appeler l'API pour mettre à jour les données des langages
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
                    // Afficher un message de succès
                    toastr.success("Les données des langages ont été mises à jour avec succès.", "Succès");
                    location.reload();
                    //spinner.style.display = 'none';
                })
                .catch(error => {
                    console.error("Erreur lors de la mise à jour des données :", error);
                    toastr.error("Erreur lors de la mise à jour des données des langages.", "Erreur");
                    //spinner.style.display = 'none';
                });
        });
    }
});
