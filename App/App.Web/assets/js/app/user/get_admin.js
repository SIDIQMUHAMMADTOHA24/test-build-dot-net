var temp_sodium;
var publickey = "5mF8MNG5eV9exEgjTEeRt7bX2yycBCtmguynt9Y03Cg=";
window.sodium = {
    onload: function (sodium) {
        temp_sodium = sodium;
        $(document).ready(function () {
            ClickMenu('m_users_admin');
            ClickMenu('m_list_users_admin');
            Get();
            if (CheckCookie('_tm')) {
                var tm = GetCookie('_tm');
                var m = GetCookie('_m');
                DeleteMessage();
                if (tm === '0') {
                    //error
                    toastr.error(m);
                }
                else if (tm === '1') {
                    //success
                    toastr.success(m);
                }
            }
        });
    }
};

function Get() {
    OpenLoading();
    $('#get_table').DataTable({
        responsive: true,
        destroy: true,
        processing: true,
        serverSide: true,
        ajax: "/User/GetDataAdmin",
        columnDefs: [
            {
                targets: 4,
                render: function (data, type, row, meta) {
                    return UbahWaktu(data);
                }
            },
            {
                targets: 5,
                render: function (data, type, row, meta) {
                    return UbahWaktu(data);
                }
            },
            {
                targets: 0,
                className: 'text-center',
                orderable: false
            },
        ],
        order: [[5, 'desc']]
    });
    CloseLoading();
}

function ModalUpdateStatus(id) {
    $('#id_user_info').val(id);
    $('#model_update_status').modal('show');
}

function UpdateStatus() {
    $('#model_update_status').modal('hide');
    OpenLoading();
    var user_info = {
        id: $('#id_user_info').val(),
        status: $('#status').val()
    };
    var model = {
        user_info: user_info
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/UpdateStatusAdmin",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success update status');
                window.location.href = '/User/GetAdmin';
            }
            else {
                toastr.error('error');
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            CloseLoading();
            toastr.error(errorMessage);
        }
    });
}

function ModalAddAdmin() {
    $('#model_add_admin').modal('show');
}

function AddAdmin() {
    $('#model_add_admin').modal('hide');
    OpenLoading();

    var r1 = new RegExp('(?=.*[a-z])');
    var r2 = new RegExp('(?=.*[A-Z])');
    var r3 = new RegExp('(?=.*[0-9])');
    var r4 = new RegExp('(?=.*[!@#\$%\^&\*])');

    var pass_login = $('#pass_login').val();
    var confirm_pass_login = $('#confirm_pass_login').val();
    var result = {
        valid: true,
        message: ''
    };
    if (pass_login.length < 8) {
        result.valid = false;
        toastr.error("Password must contains at least min 8 characters");
    }

    if (!r1.test(pass_login)) {
        result.valid = false;
        toastr.error("Password must contain at least 1 lowercase alphabetical character");
    }

    if (!r2.test(pass_login)) {
        result.valid = false;
        toastr.error("Password must contain at least 1 lowercase alphabetical character");
    }

    if (!r3.test(pass_login)) {
        result.valid = false;
        toastr.error("Password must contain at least 1 numeric character");
    }

    if (!r4.test(pass_login)) {
        result.valid = false;
        toastr.error("Password must contain at least one special character (!@#$%^&*)");
    }

    if (pass_login !== confirm_pass_login) {
        result.valid = false;
        toastr.error("Confirm password not same with field Password");
    }
 
    var enkrip_pass = temp_sodium.crypto_box_seal(pass_login, temp_sodium.from_base64(publickey), "base64");
    var user_info = {
        email_address: $('#email_address').val(),
        name: $('#name').val(),
        phone_number: $('#phone_number').val()
    };

    if (user_info.email_address === '') {
        result.valid = false;
        toastr.error("Email address is not empty");
    }

    if (user_info.name === '') {
        result.valid = false;
        toastr.error("Name is not empty");
    }

    if (user_info.phone_number === '') {
        result.valid = false;
        toastr.error("Phone number is not empty");
    }

    if (!result.valid) {
        CloseLoading();
        $('#model_add_admin').modal('show');
        return;
    }

    var user_login_info = {
        password_login: enkrip_pass
    };
    var user = {
        user_info: user_info,
        user_login_info: user_login_info
    };
    var model = {
        user : user
    }
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/PostDataAdmin",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success add admin');
                window.location.href = '/User/GetAdmin';
            }
            else {
                toastr.error('error');
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            CloseLoading();
            toastr.error(errorMessage);
        }
    });
}

function ModalDeleteAdmin(id) {
    $('#id_user_info_delete').val(id);
    $('#model_delete_admin').modal('show');
}

function DeleteAdmin() {
    $('#model_delete_admin').modal('hide');
    OpenLoading();
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/DeleteAdmin?id_user_info=" + $('#id_user_info_delete').val(),
        data: null,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success delete');
                window.location.href = '/User/GetAdmin';
            }
            else {
                toastr.error('error');
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            CloseLoading();
            toastr.error(errorMessage);
        }
    });
}