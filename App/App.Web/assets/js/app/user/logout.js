function Logout() {
    OpenLoading();
    $.ajax({
        type: "GET",
        url: "/User/Logout",
        headers: { '_ip_user': $('#_ip_user').val() },
        success: function (msg) {
            CloseLoading();
            if (msg.result) {
                window.location.href = '/';
            }
            else {
                Swal.fire({ type: "error", title: "Error", text: "Logout tidak berhasil, cek koneksi" });
            }
        }
    });
}