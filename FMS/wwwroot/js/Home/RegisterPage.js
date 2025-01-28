document.getElementById("registerForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const username = document.getElementById("newUsername").value;
    const password = document.getElementById("newPassword").value;

    const registerData = { username, password };

    fetch("/api/auth/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(registerData)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = "/Home/LoginPage";
            } else {
                document.getElementById("registerError").style.display = "block";
                document.getElementById("registerError").textContent = "Erreur lors de l'inscription.";
            }
        })
        .catch(error => {
            document.getElementById("registerError").style.display = "block";
            document.getElementById("registerError").textContent = "Erreur lors de l'inscription.";
        });
});