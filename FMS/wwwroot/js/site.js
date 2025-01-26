// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    const spinner = document.getElementById('loadingSpinner');
    const chartCanvas = document.getElementById('languageChart');

    // Récupération des données via l'API
    fetch('/GitHub/GetLanguageStats')
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
});
