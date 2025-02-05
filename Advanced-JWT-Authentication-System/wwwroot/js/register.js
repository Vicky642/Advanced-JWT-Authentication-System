
function RegisterViewModel() {
    var self = this;

    // Observables for form fields
    self.fullname = ko.observable("");    
    self.username = ko.observable("");
    self.email = ko.observable("");
    self.PhoneNumber = ko.observable("");
    self.password = ko.observable("");
    self.fullPhoneNumber = ko.observable("");
   
    // Observable for Google OAuth user data
    self.googleEmail = ko.observable("");
    self.googleFirstName = ko.observable("");
    self.googleLastName = ko.observable("");

    // Function to handle form submission
    self.register = function () {
        debugger;
        var phoneNumber = self.fullPhoneNumber(); // Get the value from the observable
        console.log("Phone from observable:", phone); // Debugging line to ensure the observable is set

        var formData = {            
            FullName: self.fullname(),            
            UserName: self.username(),
            Email: self.email(),
            PhoneNumber: phoneNumber, // Get full international number
            Password: self.password()
        };


        console.log("Form Submitted", formData);
        //alert("Form submitted successfully!");
        debugger;
        $.ajax({
            url: "/Authentication/Authentication/Register",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                debugger;
                    self.fullname(""),                    
                    self.username(""),
                    self.email(""),
                    self.password(""),
                    self.PhoneNumber("")
                console.log("Registration successful", response);
                $.alert({
                    title: 'Success!',
                    content: 'Registration was successful.',
                    type: 'green',
                    typeAnimated: true,
                    boxWidth: '300px', // Set the box width to 300px (adjust as needed)
                    useBootstrap: false, // Disable Bootstrap styling if not needed
                    buttons: {
                        OK: function () {
                            window.location.href = '/Authentication/Login'; // Redirect to login page or wherever you'd like
                        }
                    }
                });
            },
            error: function (error) {
                debugger;
                console.error("Registration failed", error);
                $.alert({
                    title: 'Error!',
                    content: 'Registration failed. Please try again.',
                    type: 'red',
                    typeAnimated: true,
                    boxWidth: '300px', // Set the box width to 300px (adjust as needed)
                    useBootstrap: false, // Disable Bootstrap styling if not needed
                    buttons: {
                        OK: function () {
                            // Additional logic for retrying or debugging can go here
                        }
                    }
                });
            }
        });
    };

    // Function to initiate Google OAuth
    self.initiateGoogleOAuth = function () {
        debugger;
        const clientId = "928086659887-rn2ug3up95s1bk4hooavpj2patc468j0.apps.googleusercontent.com";
        const redirectUri = encodeURIComponent("https://localhost:44327/Authentication/Authentication/signin-google"); // e.g., http://localhost:5000/signin-google
        const scope = encodeURIComponent("openid profile email");
        const state = "STATE_STRING"; // Optional: Add a state parameter for security
        const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientId}&redirect_uri=${redirectUri}&response_type=code&scope=${scope}&state=${state}`;

        // ✅ Store a flag in localStorage to track sign-in initiation
        localStorage.setItem("googleSignInInitiated", "true");

        // Redirect the user to Google's authorization page
        window.location.href = authUrl;
    };

    // ✅ Function to handle Google callback
    self.handleGoogleCallback = function () {
        debugger;
        const urlParams = new URLSearchParams(window.location.search);
        const code = urlParams.get('code');
        const state = urlParams.get('state');

        // ✅ Only proceed if sign-in was initiated
        if (localStorage.getItem("googleSignInInitiated") === "true" && code) {
            debugger;
            $.ajax({
                url: `/Authentication/Authentication/signin-google?code=${code}&state=${state}`,
                type: 'GET',
                success: function (response) {
                    if (response.success) {
                        // ✅ Clear the flag after successful sign-in
                        localStorage.removeItem("googleSignInInitiated");
                        window.location.href = '/Home/Index';
                    } else {
                        alert("Google Sign-In failed: " + response.message);
                        localStorage.removeItem("googleSignInInitiated"); // Clear the flag even on failure
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error:", error);
                    alert("An error occurred during Google Sign-In.");
                    localStorage.removeItem("googleSignInInitiated"); // Clear the flag on error
                }
            });
        }
    };

}

// Apply bindings
ko.applyBindings(new RegisterViewModel());

//// Handle Google callback when the page is loaded
//$(document).ready(function () {
//    // Use 'self' to refer to the RegisterViewModel instance
//    var viewModel = ko.dataFor(document.body);  // Retrieve the current instance of RegisterViewModel
//    if (viewModel && typeof viewModel.handleGoogleCallback === 'function') {
//        viewModel.handleGoogleCallback(); // Call the method correctly
//    }
//});
