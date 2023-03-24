function OpenLoading() {
    document.getElementById("fade_loading").style.display = 'block';
    document.getElementById("modal_loading").style.display = 'block';
}

function CloseLoading() {
    document.getElementById("fade_loading").style.display = 'none';
    document.getElementById("modal_loading").style.display = 'none';
}


function OpenBarLoading() {
    document.getElementById("bar_loading").style.display = 'block';
}

function CloseBarLoading() {
    document.getElementById("bar_loading").style.display = 'none';
}

function ChangeValueBar(newprogress) {
    $('#bar_proses').html(newprogress + "%");
    $('#bar_proses').attr('aria-valuenow', newprogress).css('width', newprogress + "%");
}