$(document).ready(function () {
    //ClickMenu('menu_log');
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
        ajax: "/Ekyc/GetData",
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
                targets: 6,
                render: function (data, type, row, meta) {
                    return (parseFloat(data) * 100).toFixed(3).toString() + '%';
                }
            },
            {
                targets: 0,
                className: 'text-center',
                orderable: false
            },
        ],
        order: [[4, 'desc']]
    });
    CloseLoading();
}

function ModalPreviewVideo(id) {
    OpenLoading();
    $.ajax({
        type: "GET",
        url: "/Ekyc/GetDataVideo?id_ekyc=" + id,
        success: function (msg1) {
            CloseLoading();
            var msg = JSON.parse(msg1);
            if (msg.result) {
                var base64_file = msg.objek.base64_video;
                var question = msg.objek.question;
                var answer = msg.objek.answer;
                $('#l_question').html(question);
                $('#l_answer').html(answer);

                $("#preview_video").attr('src', 'data:video/webm;base64,' + base64_file);
                $('#model_preview_video').modal('show');
            }
            else {
                toastr.error(msg.message);
            }
        }
    });
}

function ModalUpdateStatus(id) {
    $('#id_ekyc').val(id);
    $('#model_update_status').modal('show');
}

function UpdateStatus() {
    $('#model_update_status').modal('hide');
    OpenLoading();
    var model = {
        id: $('#id_ekyc').val(),
        status : $('#status').val()
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/Ekyc/UpdateStatus",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success update status');
                window.location.href = '/Ekyc/Get';
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