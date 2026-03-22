$(document).ready(function () {
    $("form").on("submit", function (e) {
        e.preventDefault();

        const username = $("#Username").val().trim();
        const password = $("#Password").val().trim();
        const confirmPassword = $("#ConfirmPassword").val().trim();
        const messageDiv = $("#Message");
        const registerButton = $("#RegisterButton");

        // AJAX 요청
        $.ajax({
            url: "/Account/Register",
            type: "POST",
            data: {
                Username: username,
                Password: password,
                ConfirmPassword: confirmPassword
            },
            success: function (response) {
                if (response.success) {
                    messageDiv.css("color", "blue");
                    messageDiv.text(response.message);
                    registerButton.prop("disabled", true);
                } else {
                    messageDiv.css("color", "red");
                    messageDiv.text(response.message);
                }
            },
            error: function () {
                messageDiv.css("color", "red");
                messageDiv.text("서버와 통신 중 오류가 발생했습니다.");
            }
        });
    });
});



