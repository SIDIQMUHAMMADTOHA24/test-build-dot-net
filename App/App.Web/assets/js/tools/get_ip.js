$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "https://api.ipify.org?format=json",
        success: function (data) {
            if (ValidateIPaddress(data.ip)) {
                $('#_ip_user').val(data.ip);
            }
            else {
                $('#_ip_user').val("");
            }
        },
    });
});

function ValidateIPaddress(inputText) {
    var ipformat = /^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
    if (inputText.match(ipformat)) {
        return true;
    }
    else {
        return false;
    }
}