var result_face = '';
var result_video = '';
var result_audio = '';
var id_rq = '';
var tag_finish = 0;
var max_width = 620;
var max_height = 620;
var temp_sodium;
var publickey = "5mF8MNG5eV9exEgjTEeRt7bX2yycBCtmguynt9Y03Cg=";
window.sodium = {
    onload: function (sodium) {
        $(document).ready(function () {
            temp_sodium = sodium;
            GenerateCaptcha();
        });
    }
};

function GenerateCaptcha() {
    OpenLoading();
    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    $.ajax({
        type: "POST",
        url: "/User/GenerateImageCaptcha",
        headers: { '__RequestVerificationToken': anti_forgery_token },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                document.getElementById('img_captcha').src = `data:image/png;base64,${msg.message}`;
            }
            else {
                toastr.error('error');
            }
        }
    });
}

function Webcam() {
    $('#label_full_name').html(encode_jax($('#full_name').val()));
    $('#label_email').html(encode_jax($('#email').val()));
    $('#label_phone').html(encode_jax($('#phone').val()));

    document.getElementById("button_next").disabled = true;
    document.getElementById("button_prev").disabled = true;
    //OpenLoading();
    //$.ajax({
    //    type: "GET",
    //    url: "/Ekyc/GetRandomQuestion",
    //    success: function (msg) {
    //        CloseLoading();
    //        if (msg !== null) {
    //            var question = msg.question;
    //            id_rq = msg.id;
    //            $('#question').html(question);
    //        }
    //        else {
    //            toastr.error('error');
    //        }
    //    }
    //});
    $('#question').html("The process of taking facial photos without wearing a mask");

    const video = document.getElementById('video');
    const url = '/assets/js/app/ekyc/models';

    Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri(url),
        faceapi.nets.faceLandmark68Net.loadFromUri(url),
        faceapi.nets.faceRecognitionNet.loadFromUri(url),
        faceapi.nets.faceExpressionNet.loadFromUri(url)
    ]).then(startVideo);

    function startVideo() {
        var time_record = 7000; //in sec
        navigator.mediaDevices.getUserMedia({
            video: true,
            audio: true
        }).then(stream => {
            video.srcObject = stream;
            video.captureStream = video.captureStream || video.mozCaptureStream;
            return new Promise(resolve => video.onplaying = resolve);
        }).then(() => startRecording(video.captureStream(), time_record))
            .then(recordedChunks => {
                let recordedBlob = new Blob(recordedChunks, { type: "video/webm" });
                var reader = new FileReader();
                reader.readAsDataURL(recordedBlob);
                reader.onloadend = function () {
                    result_video = reader.result.split(',')[1];
                    tag_finish++;
                    //let recordedBlobAudio = new Blob(recordedChunks, { type: "audio/mp3" });
                    //var readerAudio = new FileReader();
                    //readerAudio.readAsDataURL(recordedBlobAudio);
                    //readerAudio.onloadend = function () {
                    //    result_audio = readerAudio.result.split(',')[1];
                    if (tag_finish === 2) {
                        document.getElementById("button_next").disabled = false;
                        document.getElementById("button_prev").disabled = false;
                        toastr.success("Finish, click next button");
                    }

                    //}
                }
            })
            .catch('error');
    }

    async function startRecording(stream, lengthInMS) {
        checkImageFace();
        toastr.warning("Wait in 3 seconds");
        var time_delay = 3000;
        let recorder = new MediaRecorder(stream);
        let data = [];

        recorder.ondataavailable = event => data.push(event.data);
        await delay(time_delay);
        recorder.start();
        toastr.success("The process of taking photos of faces is running");
        let stopped = new Promise((resolve, reject) => {
            recorder.onstop = resolve;
            recorder.onerror = event => reject(event.name);
        });

        let recorded = wait(lengthInMS).then(
            () => recorder.state === "recording" && recorder.stop()
        );

        return Promise.all([
            stopped,
            recorded
        ])
            .then(() => data);
    }

    function delay(time) {
        return new Promise(resolve => {
            setTimeout(() => {
                resolve();
            }, time);
        });
    }

    function wait(delayInMS) {
        return new Promise(resolve => setTimeout(resolve, delayInMS));
    }

    video.addEventListener('play', () => {
        const canvas = $("#canvas").get(0);
        const displaySize = { width: video.clientWidth, height: video.clientHeight };
        faceapi.matchDimensions(canvas, displaySize);
        setInterval(async () => {
            var detections = await faceapi.detectAllFaces(video, new faceapi.TinyFaceDetectorOptions());
            if (detections.length !== 0 && result_face === '') {
                extractFaceFromBox(video);
            }
            const resizedDetections = faceapi.resizeResults(detections, displaySize);
            canvas.getContext('2d').clearRect(0, 0, canvas.width, canvas.height);
            faceapi.draw.drawDetections(canvas, resizedDetections);
            //faceapi.draw.drawFaceLandmarks(canvas, resizedDetections)
            //faceapi.draw.drawFaceExpressions(canvas, resizedDetections)
        }, 300)
    })
}

async function extractFaceFromBox(inputImage) {
    const regionsToExtract = [
        new faceapi.Rect(0, 0, inputImage.clientWidth, inputImage.clientHeight)
    ]
    let faceImages = await faceapi.extractFaces(inputImage, regionsToExtract)

    if (faceImages.length !== 0) {
        faceImages.forEach(cnv => {
            var image_face = new Image();
            image_face.src = cnv.toDataURL();
            image_face.onload = function () {
                var resized_face = resizeMe(image_face);
                result_face = resized_face.split(',')[1]; //png
            }
            tag_finish++;
            if (tag_finish === 2) {
                document.getElementById("button_next").disabled = false;
                document.getElementById("button_prev").disabled = false;
                toastr.success("Finish, click next button");
            }
        })
    }
}

function StopWebcam() {
    const video = document.getElementById('video');
    const mediaStream = video.srcObject;
    const tracks = mediaStream.getTracks();
    tracks[0].stop();
    tracks.forEach(track => track.stop())
}

function ModalPostVerification() {
    OpenLoading();
    $('#modal_verification').modal('hide');
    var confirm_submit_check = $('#confirm_submit:checked').val();
    if (confirm_submit_check === undefined) {
        Swal.fire({
            text: "Sorry, please check approval confirmation.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light-primary"
            }
        }).then(function () {
            KTUtil.scrollTop();
        });
        CloseLoading();
        return;
    }
    var cap = $('#captcha').val()
    if (cap === '') {
        Swal.fire({
            text: "Sorry, please input captcha",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light-primary"
            }
        }).then(function () {
            KTUtil.scrollTop();
        });
        CloseLoading();
        return;
    }

    var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
    var user_info = {
        email_address: $('#email').val(),
        phone_number: $('#phone').val()
    };
    var user = {
        user_info: user_info
    };
    var model_verif_token = {
        user: user,
        token_data: cap
    };
    $.ajax({
        type: "POST",
        url: "/User/GenerateToken",
        data: model_verif_token,
        headers: { '__RequestVerificationToken': anti_forgery_token },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                $('#modal_verification').modal('show');
            }
            else {
                toastr.error(msg.message);
            }
        },
        error: function (error) {
            CloseLoading();
            toastr.error(error.status + ' ' + error.statusText);
        }
    });
}

function PostVerification() {
    OpenLoading();
    $('#modal_verification').modal('hide');
    var confirm_submit_check = $('#confirm_submit:checked').val();
    if (confirm_submit_check === undefined) {
        Swal.fire({
            text: "Sorry, please check approval confirmation.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light-primary"
            }
        }).then(function () {
            KTUtil.scrollTop();
        });
        CloseLoading();
        return;
    }

    var name_file = $("#file_data")[0].files[0].name.split('.');
    var type_file = name_file[name_file.length - 1];

    var model_input_ekyc = {
        name_image_id_card: "idcard." + type_file,
        name_image_face_target: "face.png",
        //name_video: "video.webm",
        base64_image_face_target: result_face,
        //base64_video: result_video
        type_ekyc: 0 //without video
    };
    var pass = $('#password').val();
    var enkrip_pass = temp_sodium.crypto_box_seal(pass, temp_sodium.from_base64(publickey), "base64");
    var user_login_info = {
        user_name: $('#email').val(),
        password_login: enkrip_pass
    };
    var user_info = {
        name: $('#full_name').val(),
        email_address: $('#email').val(),
        phone_number: $('#phone').val(),
        payment: 0,
        license: ''
    };

    var list_user_package = [];
    $("input[id='package']:checked").each(function () {
        var user_package = {
            package: $(this).val()
        };
        list_user_package.push(user_package);
    });

    var file = $("#file_data")[0].files;
    var reader = new FileReader();
    var file_data = new Blob(file);
    reader.readAsDataURL(file_data);
    reader.model = model_input_ekyc;
    reader.onload = function (readerEvt) {
        var image = new Image();
        image.src = readerEvt.target.result;
        image.onload = function () {
            var resized = resizeMe(image);
            var input_ekyc = readerEvt.target.model;
            var b_file = resized.split(',')[1];
            input_ekyc.base64_image_id_card = b_file;

            var user_ekyc = {
                full_name: $('#full_name').val(),
                email: $('#email').val(),
                phone: $('#phone').val(),
                id_random_question: id_rq
            };

            var body_ra = {
                nik: $('#nik').val(),
                name: user_ekyc.full_name,
                birthplace: $('#birthplace').val(),
                birthdate: $('#birthdate').val(),
                trx_id: ' ',
                identity_photo: ' '
            };

            var model_ekyc = {
                input_ekyc: input_ekyc,
                user_ekyc: user_ekyc,
                body_ra: body_ra
            };

            var surat_permohonan = $("#surat_permohonan")[0].files;
            var reader_surat_permohonan = new FileReader();
            var file_data_surat_permohonan = new Blob(surat_permohonan);
            reader_surat_permohonan.readAsDataURL(file_data_surat_permohonan);
            reader_surat_permohonan.onload = function (readerEvtsurat_permohonan) {
                var file_surat_permohonan = readerEvtsurat_permohonan.target.result.split(',')[1];

                var akta_pendirian = $("#akta_pendirian")[0].files;
                var reader_akta_pendirian = new FileReader();
                var file_data_akta_pendirian = new Blob(akta_pendirian);
                reader_akta_pendirian.readAsDataURL(file_data_akta_pendirian);
                reader_akta_pendirian.onload = function (readerEvtakta_pendirian) {
                    var file_akta_pendirian = readerEvtakta_pendirian.target.result.split(',')[1];

                    var surat_pengesahan = $("#surat_pengesahan")[0].files;
                    var reader_surat_pengesahan = new FileReader();
                    var file_data_surat_pengesahan = new Blob(surat_pengesahan);
                    reader_surat_pengesahan.readAsDataURL(file_data_surat_pengesahan);
                    reader_surat_pengesahan.onload = function (readerEvtsurat_pengesahan) {
                        var file_surat_pengesahan = readerEvtsurat_pengesahan.target.result.split(',')[1];

                        var badan_usaha = {
                            nama: $('#nama_perusahaan').val()
                        };

                        var user = {
                            user_login_info: user_login_info,
                            user_info: user_info,
                            list_user_package: list_user_package,
                            badan_usaha: badan_usaha
                        };

                        var token_otp = document.getElementById("token_otp").value;
                        var enkrip_token_otp = temp_sodium.crypto_box_seal(token_otp, temp_sodium.from_base64(publickey), "base64");

                        var model = {
                            input_verification: model_ekyc,
                            user: user,
                            token_data: $('#token_data').val(),
                            token_otp: enkrip_token_otp
                        };

                        model.input_verification.input_ekyc.name_akta_pendirian = "akta_pendirian.pdf";
                        model.input_verification.input_ekyc.name_surat_pengesahan = "surat_pengesahan.pdf";
                        model.input_verification.input_ekyc.name_surat_permohonan = "surat_permohonan.pdf";

                        model.input_verification.input_ekyc.base64_akta_pendirian = file_akta_pendirian;
                        model.input_verification.input_ekyc.base64_surat_pengesahan = file_surat_pengesahan;
                        model.input_verification.input_ekyc.base64_surat_permohonan = file_surat_permohonan;

                        var anti_forgery_token = $('meta[name="anti_forgery_token"]').attr('content');
                        $.ajax({
                            type: "POST",
                            url: "/User/PostVerificationPR",
                            data: model,
                            headers: { '__RequestVerificationToken': anti_forgery_token },
                            success: function (data) {
                                CloseLoading();
                                if (data.result) {
                                    document.getElementById("button_prev").style.display = 'none';
                                    document.getElementById("button_next").style.display = 'none';
                                    document.getElementById("kt_login_signup_form_submit_button").style.display = 'none';

                                    document.getElementById("back_to_login").style.display = 'block';
                                    var message = "Success submit data, please wait for the confirmation answer in your email";
                                    $('#message_submit').html(message);
                                    Swal.fire({
                                        text: message,
                                        icon: "success",
                                        buttonsStyling: false,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: {
                                            confirmButton: "btn font-weight-bold btn-light-primary"
                                        }
                                    }).then(function () {
                                        KTUtil.scrollTop();
                                    });
                                    toastr.success(data.message);
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
                };
            };
        }
        image.onerror = function () {
            CloseLoading();
            toastr.error("Fail, please check the image file type");
        }
    };
}

function resizeMe(img) {
    try {
        var canvas = document.createElement('canvas');
        var width = img.width;
        var height = img.height;

        if (width > height) {
            if (width > max_width) {
                height = Math.round(height *= max_width / width);
                width = max_width;
            }
        } else {
            if (height > max_height) {
                width = Math.round(width *= max_height / height);
                height = max_height;
            }
        }

        canvas.width = width;
        canvas.height = height;
        var ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, width, height);
        return canvas.toDataURL("image/jpeg", 0.7); // get the data from canvas as 70% JPG (can be also PNG, etc.)
    }
    catch (err) {
        CloseLoading();
        toastr.error("Fail, please check the image file type");
    }
}

function getImageFromCanvas() {
    let canvas = document.createElement('canvas');
    let video = document.getElementById('video');
    let ctx = canvas.getContext('2d');
    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
    result_face = canvas.toDataURL("image/jpeg", 0.7).split(',')[1];
    tag_finish++;
    if (tag_finish === 2) {
        document.getElementById("button_next").disabled = false;
        document.getElementById("button_prev").disabled = false;
        toastr.success("Finish, click next button");
    }
}

function checkImageFace() {
    var cimf = 0;
    setInterval(async () => {
        cimf++;
        if (cimf === 15 && result_face === '') {
            getImageFromCanvas();
        }
    }, 1000)
}