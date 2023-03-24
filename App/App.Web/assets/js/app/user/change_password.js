var temp_sodium;
var publickey = "5mF8MNG5eV9exEgjTEeRt7bX2yycBCtmguynt9Y03Cg=";
window.sodium = {
    onload: function (sodium) {
        temp_sodium = sodium;
    }
};

function ChangePassword() {
    OpenLoading();
    var password = document.getElementById('password').value;
    var new_password = document.getElementById('new_password').value;
    var password_confirm = document.getElementById('password_confirm').value;
    var r1 = new RegExp('(?=.*[a-z])');
    var r2 = new RegExp('(?=.*[A-Z])');
    var r3 = new RegExp('(?=.*[0-9])');
    var r4 = new RegExp('(?=.*[!@#\$%\^&\*])');

    var result = {
        valid: true,
        message: ''
    };
    if (new_password.length < 8) {
        result.valid = false;
        toastr.error("Password must contains at least min 8 characters");
    }

    if (!r1.test(new_password)) {
        result.valid = false;
        toastr.error("Password must contain at least 1 lowercase alphabetical character");
    }

    if (!r2.test(new_password)) {
        result.valid = false;
        toastr.error("Password must contain at least 1 lowercase alphabetical character");
    }

    if (!r3.test(new_password)) {
        result.valid = false;
        toastr.error("Password must contain at least 1 numeric character");
    }

    if (!r4.test(new_password)) {
        result.valid = false;
        toastr.error("Password must contain at least one special character (!@#$%^&*)");
    }

    if (new_password !== password_confirm) {
        result.valid = false;
        toastr.error("Confirm password not same with field Password");
    }

    if (!result.valid) {
        CloseLoading();
        return;
    }

    var model = {
        new_pass_login: temp_sodium.crypto_box_seal(new_password, temp_sodium.from_base64(publickey), "base64"),
        pass_login: temp_sodium.crypto_box_seal(password, temp_sodium.from_base64(publickey), "base64")
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/ChangePasswordData",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (data) {
            CloseLoading();
            if (data.result) {
                toastr.success(data.message);
            }
            else {
                toastr.error(data.message);
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            CloseLoading();
            toastr.error(errorMessage);
        }
    });
}