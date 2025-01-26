document.addEventListener("DOMContentLoaded", function () {
    const token = localStorage.getItem("token");
    const currentPath = window.location.pathname;

    // Si aucun jeton n'est trouvé et que l'utilisateur n'est pas déjà sur la page d'enregistrement ou de login, rediriger vers LoginPage
    if (!token && currentPath !== "/Home/LoginPage" && currentPath !== "/Home/RegisterPage") {
        window.location.href = '/Home/LoginPage';  // Redirige vers la page de login si l'utilisateur n'est pas connecté
        return;
    }

    // Si un jeton est trouvé et que l'utilisateur est sur la page de login ou d'enregistrement, redirige vers la page principale
    if (token && (currentPath === "/Home/LoginPage" || currentPath === "/Home/RegisterPage")) {
        window.location.href = '/GitHub/Index';  // Redirige vers la page principale si l'utilisateur est déjà connecté
        return;
    }

    // Si un jeton est trouvé et l'utilisateur n'est pas sur la page login ou register, afficher le graphique
    if (token) {
        const spinner = document.getElementById('loadingSpinner');
        const chartCanvas = document.getElementById('languageChart');

        // Récupération des données via l'API protégée
        fetch('/GitHub/GetLanguageStats', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`  // Ajouter le jeton d'authentification
            }
        })
            .then(response => response.json())
            .then(data => {
                // Masquer le spinner et afficher le canvas
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
                spinner.innerHTML = '<p class="text-danger">Une erreur est survenue lors du chargement des données.</p>';
            });
    }
});
