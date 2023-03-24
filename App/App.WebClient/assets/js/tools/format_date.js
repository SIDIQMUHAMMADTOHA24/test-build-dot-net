function ConvertDate(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat);
    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}

function DateServer(a) {
    var b = a.split('/');
    return b[2] + '-' + b[1] + '-' + b[0];
}