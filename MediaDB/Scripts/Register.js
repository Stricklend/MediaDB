(function ($) {
    function formatTimestamp(date) {
        function pad(value) {
            return value < 10 ? "0" + value : value.toString();
        }

        return date.getFullYear() + "-" +
            pad(date.getMonth() + 1) + "-" +
            pad(date.getDate()) + " " +
            pad(date.getHours()) + ":" +
            pad(date.getMinutes()) + ":" +
            pad(date.getSeconds());
    }

    function setFeedback($message, text, stateClass) {
        $message.removeClass("is-error is-success");

        if (!text) {
            $message.text("");
            return;
        }

        if (stateClass) {
            $message.addClass(stateClass);
        }

        $message.text(text);
    }

    function updateRegistrationMeta(user) {
        var submittedAt = formatTimestamp(new Date());

        $("#RegisterMetaId").text(user && user.id ? user.id : "Automatic");
        $("#RegisterMetaCreated").text(user && user.created_at ? user.created_at : submittedAt);
        $("#RegisterMetaUpdated").text(user && user.updated_at ? user.updated_at : submittedAt);
    }

    function getValidationMessage(payload) {
        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!payload.user_id) {
            return "ID is required.";
        }

        if (!payload.user_password) {
            return "Password is required.";
        }

        if (!payload.confirm_user_password) {
            return "Please confirm your password.";
        }

        if (payload.user_password !== payload.confirm_user_password) {
            return "Password and confirmation do not match.";
        }

        if (!payload.username) {
            return "Username is required.";
        }

        if (!payload.email) {
            return "Email is required.";
        }

        if (!emailPattern.test(payload.email)) {
            return "Please enter a valid email address.";
        }

        if (payload.user_id.length > 50) {
            return "ID must be 50 characters or less.";
        }

        if (payload.user_password.length > 255) {
            return "Password must be 255 characters or less.";
        }

        if (payload.username.length > 50) {
            return "Username must be 50 characters or less.";
        }

        if (payload.email.length > 100) {
            return "Email must be 100 characters or less.";
        }

        return "";
    }

    $(function () {
        var $form = $("#registerForm");

        if (!$form.length) {
            return;
        }

        var $message = $("#Message");
        var $registerButton = $("#RegisterButton");

        $form.on("submit", function (e) {
            e.preventDefault();

            $("#user_id").val($.trim($("#user_id").val()));
            $("#username").val($.trim($("#username").val()));
            $("#email").val($.trim($("#email").val()));

            var payload = {
                user_id: $("#user_id").val() || "",
                user_password: $("#user_password").val() || "",
                confirm_user_password: $("#confirm_user_password").val() || "",
                username: $("#username").val() || "",
                email: $("#email").val() || ""
            };

            var validationMessage = getValidationMessage(payload);

            if (validationMessage) {
                setFeedback($message, validationMessage, "is-error");
                return;
            }

            $registerButton.prop("disabled", true);
            setFeedback($message, "", "");

            $.ajax({
                url: $form.attr("action"),
                type: "POST",
                data: $form.serialize(),
                dataType: "json",
                success: function (response) {
                    if (response && response.success) {
                        setFeedback($message, response.message, "is-success");
                        updateRegistrationMeta(response.user);
                        return;
                    }

                    $registerButton.prop("disabled", false);
                    setFeedback($message, response && response.message ? response.message : "Registration could not be completed.", "is-error");
                },
                error: function () {
                    $registerButton.prop("disabled", false);
                    setFeedback($message, "A server error occurred while submitting the form.", "is-error");
                }
            });
        });
    });
}(jQuery));
