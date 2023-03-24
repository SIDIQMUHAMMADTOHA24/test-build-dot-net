function VerifManual() {
    OpenLoading();
    $('#modal_verif_manual').modal('hide');
    var surat_pelengkap = $("#file_datamanual")[0].files;
    var reader_surat_pelengkap = new FileReader();
    var file_data_surat_pelengkap = new Blob(surat_pelengkap);
    reader_surat_pelengkap.readAsDataURL(file_data_surat_pelengkap);
    reader_surat_pelengkap.onload = function (readerEvtsurat_pelengkap) {
        var id_ekyc = document.getElementById('id_ekyc_ra').value;
        var model = {
            id_ekyc: id_ekyc
        };

        var file_surat_pelengkap = readerEvtsurat_pelengkap.target.result.split(',')[1];
        model.name_file_manual = "file_manual.zip";
        model.base64_file_manual = file_surat_pelengkap;

        var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
        $.ajax({
            type: "POST",
            url: "/Ekyc/PostVerifManual",
            data: model,
            headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
            success: function (data) {
                CloseLoading();
                var msg = JSON.parse(data);
                if (msg.result) {
                    toastr.success(msg.message);
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
}

function ModalVerifManual() {
    $('#modal_verif_manual').modal('show');
}