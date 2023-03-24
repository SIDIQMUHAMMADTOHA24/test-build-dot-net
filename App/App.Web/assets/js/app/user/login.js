var temp_sodium;
var publickey = "5mF8MNG5eV9exEgjTEeRt7bX2yycBCtmguynt9Y03Cg=";
window.sodium = {
    onload: function (sodium) {
        $(document).ready(function () {
            temp_sodium = sodium;

            document.getElementById('user_login')
                .addEventListener('keyup', function (event) {
                    if (event.code === 'Enter') {
                        event.preventDefault();
                        Login();
                    }
                });

            document.getElementById('pass_login')
                .addEventListener('keyup', function (event) {
                    if (event.code === 'Enter') {
                        event.preventDefault();
                        Login();
                    }
                });
        });
    }
};

function Login() {
    OpenLoading();
    var pass_login = document.getElementById("pass_login").value;
    var user_login = document.getElementById("user_login").value;

    if (user_login === "") {
        CloseLoading();
        toastr.error("Input user name");
        return;
    }

    if (pass_login === "") {
        CloseLoading();
        toastr.error("Input password");
        return;
    }

    var enkrip_pass = temp_sodium.crypto_box_seal(pass_login, temp_sodium.from_base64(publickey), "base64");
    var model = {
        user_login: user_login,
        pass_login: enkrip_pass
    }

    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/PostLogin",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {

                $('#div_modal_token').html(html_modal_token);

                document.getElementById('token')
                    .addEventListener('keyup', function (event) {
                        if (event.code === 'Enter') {
                            event.preventDefault();
                            SubmitToken();
                        }
                    });

                $('#modal_input_token').modal('show');
            }
            else {
                toastr.error(msg.message);
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            CloseLoading();
            toastr.error(errorMessage);
        }
    });
}

function SubmitToken() {
    OpenLoading();
    var publickey = "5mF8MNG5eV9exEgjTEeRt7bX2yycBCtmguynt9Y03Cg=";
    var token = document.getElementById("token").value;

    if (token === '') {
        toastr.error("Please input token");
        return;
    }

    var enkrip_token = sodium.crypto_box_seal(token, sodium.from_base64(publickey), "base64");

    var model = {
        value_token: enkrip_token,
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/ValidateToken",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                window.location.href = '/Home/Index';
            }
            else {
                toastr.error(msg.message);
            }
        }
    });
}

let html_modal_token = `<div class="modal fade" id="modal_input_token" data-backdrop="static" tabindex="-1" role="dialog" aria-labelledby="staticBackdrop" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="exampleModalLabel">Input Token</h5>
            </div>
            <div class="modal-body">
            <label class="modal-body">Get token from your email</label>
            <div class="form">
                 <div class="form-group form-group-last row">
                        <label class="col-lg-3 col-form-label text-lg-right">Token:</label>
                        <div class="col-lg-7">
                            <input type="text" class="form-control form-control-lg form-control-solid" id="token" />
                        </div>
                    </div>
            </div>
            <div class="modal-footer">
                <button type="submit" class="btn btn-primary font-weight-bold" onclick="SubmitToken()">Submit</button>
            </div>
        </div>
    </div>
</div>`;