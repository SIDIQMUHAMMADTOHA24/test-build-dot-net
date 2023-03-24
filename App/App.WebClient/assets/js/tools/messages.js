function SetMessage(tm, m) {
    DeleteCookie('_tm');
    SetCookie('_tm', tm, 1);

    DeleteCookie('_m');
    SetCookie('_m', m, 1);
}

function DeleteMessage() {
    DeleteCookie('_tm');
    DeleteCookie('_m');
}