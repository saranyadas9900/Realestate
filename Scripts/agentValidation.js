$(document).ready(function () {
    $("#AddNewAgentForm").submit(function (event) {
        var isValid = true;

        // Validate Username
        var username = $("#Username").val().trim();
        if (username === "") {
            $("#Username").next(".text-danger").text("Username is required.");
            isValid = false;
        } else {
            $("#Username").next(".text-danger").text("");
        }

        // Validate Password (Minimum 6 characters, 1 uppercase letter, 1 digit, and 1 special character)
        var password = $("#Password").val();
        var passwordPattern = /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{6,}$/;
        if (password === "" || !passwordPattern.test(password)) {
            $("#Password").next(".text-danger").text("Password must be at least 6 characters, contain 1 uppercase letter, 1 digit, and 1 special character.");
            isValid = false;
        } else {
            $("#Password").next(".text-danger").text("");
        }

        // Validate Name
        var name = $("#Name").val().trim();
        if (name === "") {
            $("#Name").next(".text-danger").text("Name is required.");
            isValid = false;
        } else {
            $("#Name").next(".text-danger").text("");
        }

        // Validate Phone Number (must be 10 digits)
        var phoneNumber = $("#PhoneNumber").val().trim();
        var phonePattern = /^\d{10}$/;
        if (phoneNumber === "" || !phonePattern.test(phoneNumber)) {
            $("#PhoneNumber").next(".text-danger").text("Phone number must be 10 digits.");
            isValid = false;
        } else {
            $("#PhoneNumber").next(".text-danger").text("");
        }

        // Validate Email (must be unique - assume uniqueness check on server-side)
        var email = $("#Email").val().trim();
        if (email === "") {
            $("#Email").next(".text-danger").text("Email is required.");
            isValid = false;
        } else if (!validateEmail(email)) {
            $("#Email").next(".text-danger").text("Enter a valid email address.");
            isValid = false;
        } else {
            $("#Email").next(".text-danger").text("");
        }

        // If any field is invalid, prevent form submission
        if (!isValid) {
            event.preventDefault();
        }
    });

    function validateEmail(email) {
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return emailPattern.test(email);
    }
});
