var googleRecaptchaValid = false;
var birthDate = document.getElementById('birthDate');

if (birthDate != null) {
    birthDate.onload(setDate());
}

function setDate() {
    var datepicker = document.getElementById("birthDate");
    datepicker.max = new Date().toJSON().split('T')[0];
    datepicker.min = (new Date() - 100).toJSON().split('T')[0];
}

function validateRecaptcha() {
    var recaptchaError = document.getElementById("captchaError");
    recaptchaError.innerHTML = "";

    googleRecaptchaValid = true;
}

function validatePassword() {
    var password = document.getElementById("password");
    var repeat = document.getElementById("repeat");

    if (password.value != repeat.value) {
        repeat.setCustomValidity("Passwords must match");
    }
    else {
        repeat.setCustomValidity("");
    }
}

function validateRegistration(event) {
    if (!googleRecaptchaValid) {
        var recaptchaError = document.getElementById("captchaError");
        recaptchaError.innerHTML = "Google reCAPTCHA is not valid!";

        event.preventDefault();
    }
}