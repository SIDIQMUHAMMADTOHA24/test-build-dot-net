function FindDuplicate(arr) {
    var q = String.format('SELECT v.jumlah FROM (SELECT COUNT(field) AS jumlah FROM ? GROUP BY field) as v WHERE v.jumlah > 1');;
    var res = alasql(q, [arr]);
    if (res.length !== 0) {
        return true;
    }
    else {
        return false;
    }
}