(function() {

    function signIn() {
        window.location = "http://localhost:8080/Authorize";
    }

    var signInButton = document.getElementById("signInButton");

    signInButton.addEventListener("click", e => {
        signIn();
    });
}());