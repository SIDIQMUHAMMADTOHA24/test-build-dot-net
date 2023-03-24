$(document).ready(function () {
    ClickMenu('m_users');
    ClickMenu('m_list_revoke_cert');
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
        ajax: "/User/GetDataCertRevoke",
        columnDefs: [
            {
                targets: 5,
                render: function (data, type, row, meta) {
                    return UbahWaktu(data);
                }
            },
            {
                targets: 6,
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
        order: [[6, 'desc']]
    });
    CloseLoading();
}

function ModalPreviewCertificate(user_name) {
    OpenLoading();
    $.ajax({
        type: "GET",
        url: "/Certificate/GetDataByUserName?user_name=" + user_name,
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                var list_certificate = msg.objek.data;

                var html = '<tbody>';
                html += `<tr>
                                            <th>User Name</th>
                                            <th>Validity Start</th>
                                            <th>Validity End</th>
                                            <th>Status</th>
                                            <th>Actions</th>
                                        </tr>`;
                for (var lu in list_certificate) {
                    var action = ``;
                    if (list_certificate[lu].status.includes('ISSUE')) {
                        action = `<div class="dropdown dropdown-inline"><button type="button" class="btn btn-light-primary btn-icon btn-sm" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="ki ki-bold-more-ver"></i></button><div class="dropdown-menu"><a class="dropdown-item" <a href="#" onclick="ModalRevoke('${list_certificate[lu].username}', '${list_certificate[lu].serial_number}')">Revoke Certificate</a>`;
                    }
                    html += `<tr>
                                            <td>${list_certificate[lu].username}</td>
                                            <td>${UbahWaktu(list_certificate[lu].validity_start)}</td>
                                            <td>${UbahWaktu(list_certificate[lu].validity_end)}</td>
                                            <td>${list_certificate[lu].status}</td>
                                            <td>${action}</td>
                                        </tr>`;
                }
                html += '</tbody>';

                $('#table_certificate').html(html);
                $('#model_preview_certificate').modal('show');
            }
            else {
                toastr.error(msg.message);
            }
        }
    });
}

function ModalRevoke(user_name, serial_number) {
    $('#span_user_name').html(user_name);
    $('#user_name').val(user_name);
    $('#serial_number').val(serial_number);
    $('#modal_revoke').modal('show');
}

function RevokeCertificate() {
    $('#modal_revoke').modal('hide');
    OpenLoading();
    var post_revoke = {
        serial_number: $('#serial_number').val(),
        username: $('#user_name').val(),
        revocation_reason: $('#revocation_reason').val()
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/Certificate/Revoke",
        data: post_revoke,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                SetMessage('1', 'Success revoke certificate');
                window.location.href = '/User/Get';
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

function ModalPreviewEKYC(id) {
    OpenLoading();
    $('#id_ekyc_ra').val(id);
    $.ajax({
        type: "GET",
        url: "/Ekyc/GetData?id_ekyc=" + id,
        success: function (msg1) {
            CloseLoading();
            var msg = JSON.parse(msg1);
            if (msg.result) {
                var base64_image_face_target = msg.objek.base64_image_face_target;
                var base64_image_id_card = msg.objek.base64_image_id_card;
                var base64_surat_pelengkap = msg.objek.base64_surat_pelengkap;
                var base64_verif_manual = msg.objek.base64_verif_manual;
                var question = msg.objek.question;
                var answer = msg.objek.answer;
                var confidence = JSON.parse(msg.objek.confidence);

                var html = '<tbody>';
                var face_compare = confidence.find(o => o.message === 'face_compare');
                if (face_compare === undefined) {
                    html += `<tr>
                                            <th>Face And ID Card Compare</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                }
                else {
                    if (face_compare.result) {
                        var persen = (parseFloat(face_compare.face_compare) * 100).toFixed(2);
                        html += `<tr>
                                            <th>Face And ID Card Compare</th>
                                            <td>
                                                ${persen}%
                                            </td>
                                        </tr>`;
                    }
                    else {
                        html += `<tr>
                                            <th>Face And ID Card Compare</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                }

                var anti_spoof = confidence.find(o => o.message === 'anti_spoof');
                if (anti_spoof === undefined) {
                    html += `<tr>
                                            <th>Anti Spoofing</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                }
                else {
                    if (anti_spoof.result) {
                        html += `<tr>
                                            <th>Anti Spoofing</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-check text-success"></i>
                                            </td>
                                        </tr>`;
                    }
                    else {
                        html += `<tr>
                                            <th>Anti Spoofing</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                }

                var validated_name = confidence.find(o => o.message === 'id_name');
                if (validated_name === undefined) {
                    html += `<tr>
                                            <th>Validated Name</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                }
                else {
                    if (validated_name.result) {
                        html += `<tr>
                                            <th>Validated Name</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-check text-success"></i>
                                            </td>
                                        </tr>`;
                    }
                    else {
                        html += `<tr>
                                            <th>Validated Name</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                }

                var validated_birthdate = confidence.find(o => o.message === 'id_birthdate');
                if (validated_birthdate === undefined) {
                    html += `<tr>
                                            <th>Validated Birthdate</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                }
                else {
                    if (validated_birthdate.result) {
                        html += `<tr>
                                            <th>Validated Birthdate</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-check text-success"></i>
                                            </td>
                                        </tr>`;
                    }
                    else {
                        html += `<tr>
                                            <th>Validated Birthdate</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                }

                var validated_birthplace = confidence.find(o => o.message === 'id_birthplace');
                if (validated_birthplace === undefined) {
                    html += `<tr>
                                            <th>Validated Birthplace</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                }
                else {
                    if (validated_birthplace.result) {
                        html += `<tr>
                                            <th>Validated Birthplace</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-check text-success"></i>
                                            </td>
                                        </tr>`;
                    }
                    else {
                        html += `<tr>
                                            <th>Validated Birthplace</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                }

                //var live_detection = confidence.filter(o => o.message !== 'face_compare').filter(o => o.message !== 'anti_spoof')
                //if (live_detection.length !== 0) {
                //    html += `<tr>
                //                        <th>Liveness Detection</th>
                //                        <td></td>
                //                    </tr>`;
                //}
                //for (var cf in live_detection) {
                //    if (live_detection[cf].result) {
                //        html += `<tr>
                //                            <td>${live_detection[cf].message}</td>
                //                            <td>
                //                                <i class="icon-1x ki ki-bold-check text-success"></i>
                //                            </td>
                //                        </tr>`;
                //    }
                //    else {
                //        html += `<tr>
                //                            <td>${live_detection[cf].message}</td>
                //                            <td>
                //                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                //                            </td>
                //                        </tr>`;
                //    }
                //}
                if (base64_surat_pelengkap !== null) {
                    html += `<tr>
                                            <th>Surat Pelengkap</th>
                                            <td>
                                                <input type="hidden" value="${base64_surat_pelengkap}" id="surat_pelengkap">
                                                <a href="javascript:;" onclick="UnduhFilePerusahaan('surat_pelengkap')">Unduh</a>
                                            </td>
                                        </tr>`;
                }

                if (base64_verif_manual !== null) {
                    html += `<tr>
                                            <th>Data Verifikasi Manual</th>
                                            <td>
                                                <input type="hidden" value="${base64_verif_manual}" id="verif_manual">
                                                <a href="javascript:;" onclick="UnduhFileVerifManual('verif_manual')">Unduh</a>
                                            </td>
                                        </tr>`;
                }

                html += '</tbody>';

                $('#table_ekyc').html(html);

                //$('#l_question').html(question);
                //$('#l_answer').html(answer);

                $("#face_target").attr('src', 'data:image/png;base64,' + base64_image_face_target);
                $("#id_card").attr('src', 'data:image/png;base64,' + base64_image_id_card);

                $('#model_preview_video').modal('show');
            }
            else {
                toastr.error(msg.message);
            }
        }
    });
}

function UnduhFilePerusahaan(id) {
    OpenLoading();
    var file = $(`#${id}`).val();
    var stream_b = base64ToArrayBuffer(file);
    const b = new Blob([stream_b]);
    saveAs(b, id + '.pdf');
    CloseLoading();
}

function UnduhFileVerifManual(id) {
    OpenLoading();
    var file = $(`#${id}`).val();
    var stream_b = base64ToArrayBuffer(file);
    const b = new Blob([stream_b]);
    saveAs(b, id + '.zip');
    CloseLoading();
}