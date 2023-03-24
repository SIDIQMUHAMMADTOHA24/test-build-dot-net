$(document).ready(function () {
    ClickMenu('m_users');
    ClickMenu('m_log');
   
    var from_date = $('#from_date').val();
    var to_date = $('#to_date').val();
    if (from_date !== "" && to_date !== "") {
        Get(DateServer(from_date), DateServer(to_date));
    }
    else {
        Get(from_date, to_date);
    }

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

function Get(from_date, to_date) {
    OpenLoading();
    $('#get_table').DataTable({
        responsive: true,
        destroy: true,
        processing: true,
        serverSide: true,
        ajax: "/User/GetDataLog?from_date=" + from_date + "&to_date=" + to_date,
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
            },
        ],
        order: [[3, 'desc']],
        headerCallback: function (thead, data, start, end, display) {
            thead.getElementsByTagName('th')[0].innerHTML = `<label class="checkbox checkbox-single"><input type="checkbox" class="group-checkable" id="check_all" onclick=CheckAll() /><span></span></label>`;
        },
        dom: 'Blfrtip',
        buttons: [
            'print'
        ]
    });
    $('.buttons-print').detach().prependTo('.card-toolbar')
    CloseLoading();
}

function CheckAll() {
    var list_check_all = [];
    $("input[class='group-checkable']:checked").each(function () {
        list_check_all.push($(this).attr("id"));
    });
    if (list_check_all.length !== 0) {
        $(".checkable").prop("checked", "true");
    }
    else {
        $("input[class='checkable']:checked").each(function () {
            document.getElementById($(this).attr("id")).checked = false;
        });
    }
}

function ModalUpdateLogStatus() {
    OpenLoading();
    var list_check = [];
    $("input[class='checkable']:checked").each(function () {
        list_check.push($(this).attr("id"));
    });
    var ll = list_check.length;
    if (ll === 0) {
        CloseLoading();
        toastr.error('Please select to update status');
        return;
    }

    $('#model_update_log_status').modal('show');
    CloseLoading();
}

function UpdateLogStatus() {
    $('#model_update_log_status').modal('hide');
    OpenLoading();
    var list_check = [];
    $("input[class='checkable']:checked").each(function () {
        list_check.push(`'${$(this).attr("id")}'`);
    });
    let model = {
        status_log: list_check.join(',') 
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/UpdateUserLog",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success update log status');
                window.location.href = '/User/GetLog';
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

function SearchByDate() {
    var from_date = $('#from_date').val();
    var to_date = $('#to_date').val();

    if (from_date === "" && to_date === "") {
        CloseLoading();
        toastr.error("Choose date");
        return;
    }
    $('#document_box_table').DataTable().destroy();
    Get(DateServer(from_date), DateServer(to_date));
}