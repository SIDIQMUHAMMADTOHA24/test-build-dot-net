function RaVerification() {
    OpenLoading();
    var nik = document.getElementById('nik').value;
    var name = document.getElementById('name').value;
    var birthplace = document.getElementById('birthplace').value;
    var birthdate = document.getElementById('birthdate').value;
    var id_ekyc = document.getElementById('id_ekyc').value;
    var model = {
        nik: nik,
        name: name, 
        birthplace: birthplace, 
        birthdate: birthdate
    };
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/Ekyc/PostRaVerification?id_ekyc=" + id_ekyc,
        data: model,
        headers: { '__RequestVerificationToken': anti_forgery_token, '_ip_user': $('#_ip_user').val() },
        success: function (data) {
            CloseLoading();
            if (data.result) {
                toastr.success(data.message);
                var data_result = data.data;
                var html = '<tbody>';
                if (data_result === null) {
                    html += `<tr>
                                            <th>Validated Name</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;

                    html += `<tr>
                                            <th>Validated Birthdate</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;

                    html += `<tr>
                                            <th>Validated Birthplace</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                }
                else {
                    if (data_result.name === null) {
                        html += `<tr>
                                            <th>Validated Name</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                    else {
                        if (data_result.name) {
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

                    if (data_result.birthplace === null) {
                        html += `<tr>
                                            <th>Validated Birthplace</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                    else {
                        if (data_result.birthplace) {
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

                    if (data_result.birthdate === null) {
                        html += `<tr>
                                            <th>Validated Birthdate</th>
                                            <td>
                                                <i class="icon-1x ki ki-bold-close text-danger"></i>
                                            </td>
                                        </tr>`;
                    }
                    else {
                        if (data_result.birthdate) {
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
                }
                html += '</tbody>';
                $('#table_ra').html(html);
            }
            else {
                toastr.error(data.message);
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.status + ': ' + xhr.statusText
            CloseLoading();
            toastr.error(errorMessage);
        }
    });
}