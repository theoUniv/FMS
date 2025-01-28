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
                throw new Error('Login échoué');
            }
            return response.json();
        })
        .then(data => {
            localStorage.setItem("token", data.token); // Enregistrer le token
            console.log("Connexion réussie, token sauvegardé :", data.token);
        })
        .catch(error => console.error(error));

});