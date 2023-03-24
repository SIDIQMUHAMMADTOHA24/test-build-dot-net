$(document).ready(function () {
    ClickMenu('m_users_admin');
    ClickMenu('m_log_admin');
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
        ajax: "/User/GetDataLogAdmin",
        columnDefs: [
            {
                targets: 2,
                render: function (data, type, row, meta) {
                    return UbahWaktu(data);
                }
            }
        ],
        order: [[2, 'desc']]
    });
    CloseLoading();
}