$(document).ready(function () {
    $("#formUpload").submit(insert)
    $("#backtolgin-btn").on('click', function () {
        window.location.href = "./../Pages/LoginPage.html";
    });
})
function insert() {
    var data = new FormData()
    var files = $("#files").get(0).files;
    if (files.length > 0) {
        data.append("file", files[0]);
    }
    api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games/UploadGames";

    // Ajax upload  
    $.ajax({
        type: "POST",
        url: api,
        contentType: false,
        processData: false,
        data: data,
        success: GamesAdded,
        error: error
    });

    return false;
}
function GamesAdded(data) {
    console.log("success:", data);
    alert("Success Make sure all games are loaded in DB befor leaving the page")
}

function error(xhr) {
    console.log("status:", xhr.status);
    console.log("response:", xhr.responseText);
    alert("fail")
}

