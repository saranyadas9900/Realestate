$(document).ready(function () {
    // Predefined list of emails (for uniqueness check)
    var existingEmails = ["test@example.com", "user@example.com"]; // Replace with a server-side check

    // Validation on form submission
    $('form').on('submit', function (e) {
        var isValid = true;

        // Clear previous error messages
        $('.text-danger').remove();

        // Validate First Name
        if (!$('#FirstName').val()) {
            isValid = false;
            $('#FirstName').after('<span class="text-danger">First Name is required.</span>');
        }

        // Validate Last Name
        if (!$('#LastName').val()) {
            isValid = false;
            $('#LastName').after('<span class="text-danger">Last Name is required.</span>');
        }

        // Validate Date of Birth
        if (!$('#DateOfBirth').val()) {
            isValid = false;
            $('#DateOfBirth').after('<span class="text-danger">Date of Birth is required.</span>');
        }

        // Validate Gender
        if (!$('input[name="Gender"]:checked').val()) {
            isValid = false;
            $('input[name="Gender"]').last().after('<span class="text-danger">Gender is required.</span>');
        }

        // Validate Phone Number
        var phoneNumber = $('#PhoneNumber').val();
        if (!phoneNumber) {
            isValid = false;
            $('#PhoneNumber').after('<span class="text-danger">Phone Number is required.</span>');
        } else if (!/^\d{10}$/.test(phoneNumber)) {
            isValid = false;
            $('#PhoneNumber').after('<span class="text-danger">Phone Number must be 10 digits.</span>');
        }

        // Validate Email Address
        var email = $('#EmailAddress').val();
        if (!email) {
            isValid = false;
            $('#EmailAddress').after('<span class="text-danger">Email Address is required.</span>');
        } else if (existingEmails.includes(email)) {
            isValid = false;
            $('#EmailAddress').after('<span class="text-danger">Email Address is already taken.</span>');
        }

        // Validate Address
        if (!$('#Address').val()) {
            isValid = false;
            $('#Address').after('<span class="text-danger">Address is required.</span>');
        }

        // Validate State
        if ($('#stateDropdown').val() === "Select State") {
            isValid = false;
            $('#stateDropdown').after('<span class="text-danger">State is required.</span>');
        }

        // Validate City
        if ($('#cityDropdown').val() === "Select City") {
            isValid = false;
            $('#cityDropdown').after('<span class="text-danger">City is required.</span>');
        }

        // Validate Username
        if (!$('#Username').val()) {
            isValid = false;
            $('#Username').after('<span class="text-danger">Username is required.</span>');
        }

        // Validate Password
        var password = $('#Password').val();
        var passwordRegex = /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;
        if (!password) {
            isValid = false;
            $('#Password').after('<span class="text-danger">Password is required.</span>');
        } else if (!passwordRegex.test(password)) {
            isValid = false;
            $('#Password').after('<span class="text-danger">Password must be at least 6 characters long, contain one uppercase letter, one digit, and one special character.</span>');
        }

        // Validate Confirm Password
        if (!$('#ConfirmPassword').val()) {
            isValid = false;
            $('#ConfirmPassword').after('<span class="text-danger">Confirm Password is required.</span>');
        } else if ($('#ConfirmPassword').val() !== password) {
            isValid = false;
            $('#ConfirmPassword').after('<span class="text-danger">Passwords do not match.</span>');
        }

        // Prevent form submission if validation fails
        if (!isValid) {
            e.preventDefault();
        }
    });
});
