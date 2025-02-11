function LoginViewModel() {

    self.usernameOrEmail = ko.observable("");
    self.password = ko.observable("");



    self.login = function () {
        debugger;
        
        var formData = {            
            UserName: self.usernameOrEmail(),            
            Password: self.password()
        };


        console.log("Form Submitted", formData);
        //alert("Form submitted successfully!");
        debugger;
        $.ajax({
            url: "/Authentication/Authentication/Login",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                debugger;
                self.fullname(""),
                    self.usernameOrEmail(""),                    
                    self.password(""),
                    
                console.log("Login successful", response);
                $.alert({
                    title: 'Success!',
                    content: 'Login was successful.',
                    type: 'green',
                    typeAnimated: true,
                    boxWidth: '300px', // Set the box width to 300px (adjust as needed)
                    useBootstrap: false, // Disable Bootstrap styling if not needed
                    buttons: {
                        OK: function () {
                            window.location.href = '/Home/Index'; // Redirect to login page or wherever you'd like
                        }
                    }
                });
            },
            error: function (error) {
                debugger;
                console.error("Login failed", error);
                $.alert({
                    title: 'Error!',
                    content: 'Login failed. Please try again.',
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

}

ko.applyBindings(new LoginViewModel());