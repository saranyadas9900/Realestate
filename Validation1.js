document.addEventListener("DOMContentLoaded", function () {
    // Select the form element
    const form = document.querySelector("form");

    // Function to validate password
    function validatePassword(password) {
        // Define the regex pattern
        const pattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
        return pattern.test(password);
    }

    // Function to handle form submission
    function handleSubmit(event) {
        const passwordInput = document.querySelector("input[name='Password']");
        const password = passwordInput.value;
        const errorMessage = document.querySelector("#password-error");

        // Check if password is valid
        if (!validatePassword(password)) {
            event.preventDefault(); // Prevent form submission
            if (!errorMessage) {
                const error = document.createElement("p");
                error.id = "password-error";
                error.style.color = "red";
                error.textContent = "Password must be at least 6 characters long, contain at least one uppercase letter, one special character, and one number.";
                passwordInput.parentElement.appendChild(error);
            }
        } else {
            // Remove error message if validation is successful
            if (errorMessage) {
                errorMessage.remove();
            }
        }
    }

    // Attach submit event listener to the form
    form.addEventListener("submit", handleSubmit);
});
