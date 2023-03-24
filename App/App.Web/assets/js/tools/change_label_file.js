function ChangeLabelFile(id_file) {
    var fp = $("#" + id_file);
    var items = fp[0].files;
    var name = items[0].name;

    var fileSize = 0;
    var fileTypes = ['jpg', 'jpeg', 'bmp', 'png'];
    fileSize += items[0].size; // get file size
    var extension = items[0].name.split('.').pop().toLowerCase(),  //file extension from input file
        isSuccess = fileTypes.indexOf(extension) > -1;

    //if (fileSize > 25000000) {
    //    toastr.error("Ukuran file tidak boleh lebih dari 25 Mb");
    //    fp.val('');
    //    return;
    //}
    if (!isSuccess) {
        toastr.error("File not image");
        fp.val('');
        return;
    }

    $("#label_" + id_file).html(name);
}

function ChangeLabelFileZip(id_file) {
    var fp = $("#" + id_file);
    var items = fp[0].files;
    var name = items[0].name;

    var fileSize = 0;
    var fileTypes = ['zip'];
    fileSize += items[0].size; // get file size
    var extension = items[0].name.split('.').pop().toLowerCase(),  //file extension from input file
        isSuccess = fileTypes.indexOf(extension) > -1;

    //if (fileSize > 25000000) {
    //    toastr.error("Ukuran file tidak boleh lebih dari 25 Mb");
    //    fp.val('');
    //    return;
    //}
    if (!isSuccess) {
        toastr.error("File not zip");
        fp.val('');
        return;
    }

    $("#label_" + id_file).html(name);
}