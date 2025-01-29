document.getElementById("loginForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    const loginData = { username, password };

    fetch('/api/auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(loginData)
    })
        .then(response => {
            if (!response.ok) {
                toastr.error("Identifiants incorrects. Veuillez réessayer.", "Erreur de connexion");
                throw new Error('Login échoué');
            }
            return response.json();
        })
        .then(data => {
            localStorage.setItem("token", data.token); // Enregistrer le token
            console.log("Connexion réussie, token sauvegardé :", data.token);
            window.location.href = "/Home/Index"; // Redirection
        })
        .catch(error => {
            console.error(error);
        });
});
