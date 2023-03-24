function ProcessRevokeAll() {
    OpenLoading();
    var model = {
        user: ''
    }
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/CertificateRevoke/RevokeAll",
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success Revoke All');
                window.location.href = '/CertificateRevoke/Get';
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

function ModalProcessRevokeAll() {
    $('#modal_certificate_revoke_all').modal('show');
}