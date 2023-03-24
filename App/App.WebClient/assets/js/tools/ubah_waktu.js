function UbahWaktu(data) {
    var local_time = moment.utc(data).toDate();
    var waktu = moment(local_time).format('DD/MM/YYYY HH:mm');
    if (data === parseInt(data, 10)) {
        var _w = moment(local_time).format('YYYY-MM-DD HH:mm:ss');
        local_time = moment.utc(_w).toDate();
        waktu = moment(local_time).format('DD/MM/YYYY HH:mm');
    }
    return waktu;
}

function UbahTanggal(data) {
    var local_time = moment.utc(data).toDate();
    var waktu = moment(local_time).format('DD/MM/YYYY');
    if (data === parseInt(data, 10)) {
        var _w = moment(local_time).format('YYYY-MM-DD HH:mm:ss');
        local_time = moment.utc(_w).toDate();
        waktu = moment(local_time).format('DD/MM/YYYY');
    }
    return waktu;
}

function UbahWaktuAgo(data) {
    var local_time = moment.utc(data).toDate();
    var waktu = moment(local_time).fromNow();
    if (data === parseInt(data, 10)) {
        var _w = moment(local_time).format('YYYY-MM-DD HH:mm:ss');
        local_time = moment.utc(_w).toDate();
        waktu = moment(local_time).fromNow();
    }
    return waktu;
}

function convertDate(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat);
    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}

function DateServer(a) {
    var b = a.split('/');
    return b[2] + '-' + b[1] + '-' + b[0];
}