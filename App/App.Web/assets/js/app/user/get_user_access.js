$(document).ready(function () {
    ClickMenu('m_users');
    ClickMenu('m_access');
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

function Get() {
    OpenLoading();
    $('#get_table').DataTable({
        responsive: true,
        destroy: true,
        processing: true,
        serverSide: true,
        ajax: "/User/GetDataUserAccess",
        columnDefs: [
            {
                targets: 3,
                render: function (data, type, row, meta) {
                    return UbahWaktu(data);
                }
            },
            {
                targets: 0,
                className: 'text-center',
                orderable: false
            }
        ],
        order: [[3, 'desc']]
    });
    CloseLoading();
}

function ModalUpdateUserAccess(id) {
    $('#modal_update_user_access').modal('show');
    $('#id_user_package').val(id);
}

function UpdateUserAccess() {
    OpenLoading();
    var user_access = {
        id_user_package: $('#id_user_package').val(),
        sum_access: $('#sum_access').val()
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/PostUserAccess",
        data: user_access,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success update user access');
                window.location.href = '/User/GetUserAccess';
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