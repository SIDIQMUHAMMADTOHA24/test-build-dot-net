function ChangeLabelFile(id_file, tipe) {
    var fp = $("#" + id_file);
    var items = fp[0].files;
    var name = items[0].name;

    var fileSize = 0;
    var fileTypes = ['jpg', 'jpeg', 'bmp', 'png'];
    if (tipe === 1) {
        fileTypes = ['pdf'];
    }
    fileSize += items[0].size; // get file size
    var extension = items[0].name.split('.').pop().toLowerCase(),  //file extension from input file
        isSuccess = fileTypes.indexOf(extension) > -1;

    if (fileSize > 5242880) {
        toastr.error("File size cannot be more than 5 Mb");
        fp.val('');
        return;
    }
    if (tipe === 0) {
        if (!isSuccess) {
            toastr.error("File not image");
            fp.val('');
            return;
        }
    }
    else {
        if (!isSuccess) {
            toastr.error("File not pdf");
            fp.val('');
            return;
        }
    }
    

    $("#label_" + id_file).html(encode_jax(name));
}