function Process() {
    OpenLoading();
    var file = $("#file_data")[0].files;
    if (file.length === 0) {
        CloseLoading();
        toastr.error("Choose file");
        return;
    }

    var allowedExtensions = /(\.pdf)$/i;
    if (!allowedExtensions.exec(document.getElementById('file_data').value)) {
        CloseLoading();
        toastr.error("Check file extension. Only .pdf extensions are allowed");
        return;
    }

    var reader = new FileReader();
    var file_data = new Blob(file);
    reader.readAsDataURL(file_data);
    reader.onload = function (readerEvt) {
        var pdf = readerEvt.target.result.split(',')[1];
        var model = {
            base64data : pdf
        };
        var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
        $.ajax({
            type: "POST",
            url: "/Verify/ProcessData",
            data: model,
            headers: { '__RequestVerificationToken': anti_forgery_token },
            success: function (data) {
                CloseLoading();
                if (data.success) {
                    var summary = data.data.summary;
                    var html = `<tbody><tr><td><b>Summary</b></td><td>:</td><td>${summary}</td></tr>`;
                    var notes = data.data.notes;
                    html += `<tr><td><b>Notes</b></td><td>:</td><td>${notes}</td></tr>`;
                    var details = data.data.details;
                    for (var d in details) {
                        var signature_doc = details[d].signature_document;
                        var info_signer = details[d].info_signer;
                        var reason = signature_doc.reason;
                        var location = signature_doc.location;
                        var signed_in = signature_doc.signed_in;
                        var signer_name = info_signer.signer_name;
                        var signer_dn = info_signer.signer_dn;
                        var issuer_dn = info_signer.issuer_dn;
                        var serial = info_signer.serial;
                        var signer_cert_validity = info_signer.signer_cert_validity;
                        html += `<tr><td colspan="3">Signature ${parseInt(d) + 1}</td></tr>`;
                        html += `<tr><td><b>Reason</b></td><td>:</td><td>${reason}</td></tr>`;
                        html += `<tr><td><b>Location</b></td><td>:</td><td>${location}</td></tr>`;
                        html += `<tr><td><b>Signed In</b></td><td>:</td><td>${signed_in}</td></tr>`;
                        html += `<tr><td><b>Signer Name</b></td><td>:</td><td>${signer_name}</td></tr>`;
                        html += `<tr><td><b>Signer dn</b></td><td>:</td><td>${signer_dn}</td></tr>`;
                        html += `<tr><td><b>Issuer dn</b></td><td>:</td><td>${issuer_dn}</td></tr>`;
                        html += `<tr><td><b>Serial Number</b></td><td>:</td><td>${serial}</td></tr>`;
                        html += `<tr><td><b>Certificate Validity</b></td><td>:</td><td>${signer_cert_validity}</td></tr>`;

                        var info_ca = details[d].info_ca;
                        for (var ic in info_ca) {
                            var common_name_ca = info_ca[ic].common_name;
                            var cert_validity_ca = info_ca[ic].cert_validity;
                            var subject_dn_ca = info_ca[ic].subject_dn;
                            var issuer_dn_ca = info_ca[ic].issuer_dn;
                            var serial_ca = info_ca[ic].serial;
                            html += `<tr><td colspan="3">Info CA ${parseInt(ic) + 1}</td></tr>`;
                            html += `<tr><td><b>Common Name</b></td><td>:</td><td>${common_name_ca}</td></tr>`;
                            html += `<tr><td><b>Certificate Validity</b></td><td>:</td><td>${cert_validity_ca}</td></tr>`;
                            html += `<tr><td><b>Subject dn</b></td><td>:</td><td>${subject_dn_ca}</td></tr>`;
                            html += `<tr><td><b>Issuer dn</b></td><td>:</td><td>${issuer_dn_ca}</td></tr>`;
                            html += `<tr><td><b>Serial Number</b></td><td>:</td><td>${serial_ca}</td></tr>`;
                        }

                        var info_tsa = details[d].info_tsa;
                        var common_name_tsa = info_tsa.common_name;
                        var cert_validity_tsa = info_tsa.cert_validity;
                        var subject_dn_tsa = info_tsa.subject_dn;
                        var issuer_dn_tsa = info_tsa.issuer_dn;
                        var serial_tsa = info_tsa.serial;
                        html += `<tr><td colspan="3">Info TSA ${parseInt(d) + 1}</td></tr>`;
                        html += `<tr><td><b>Common Name</b></td><td>:</td><td>${common_name_tsa}</td></tr>`;
                        html += `<tr><td><b>Certificate Validity</b></td><td>:</td><td>${cert_validity_tsa}</td></tr>`;
                        html += `<tr><td><b>Subject dn</b></td><td>:</td><td>${subject_dn_tsa}</td></tr>`;
                        html += `<tr><td><b>Issuer dn</b></td><td>:</td><td>${issuer_dn_tsa}</td></tr>`;
                        html += `<tr><td><b>Serial Number</b></td><td>:</td><td>${serial_tsa}</td></tr>`;
                    }
                    
                    html += `</tbody>`;
                    var label_html = `<div class="row">
                                <div class="col-md-12 col-sm-5">
                                    <div class="card card-custom">
                                        <div class="card-header">
                                            <div class="card-title">
                                                <span class="card-icon">
                                                    <i class="flaticon2-browser text-primary"></i>
                                                </span>
                                                <h3 class="card-label">
                                                    Digital Signature Information
                                                </h3>
                                            </div>
                                        </div>
                                        <div class="card-body">
                                            <div class="row d-flex justify-content-center">
                                                <table class="table  table-hover table-header-fixed">
                                                     ${html}
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`;
                    $('#info_cert').html(label_html);
                }
                else {
                    Swal.fire({
                        text: data.message,
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Ok, got it!",
                        customClass: {
                            confirmButton: "btn font-weight-bold btn-light-primary"
                        }
                    }).then(function () {
                        KTUtil.scrollTop();
                    });
                    toastr.error(data.message);
                }
            },
            error: function (xhr, status, error) {
                var errorMessage = xhr.status + ': ' + xhr.statusText
                CloseLoading();
                toastr.error(errorMessage);
            }
        });
    };
}