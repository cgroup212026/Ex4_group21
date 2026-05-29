let allGames = [];

$(document).ready(function () {
    let userName = localStorage.getItem("UserName");
    let id = localStorage.getItem("ID");

    if (userName) {
        $("#user-greeting").html("Hello " + userName);
    } else {
        $("#user-greeting").html("Hello Guest");
    }
    //let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games";
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Users/GetUserGames?id=" + id;
    
    $("#back-btn").on('click', function(){ 
        window.location.href = "./../Pages/index.html";
    });
    $("#add-game-btn").on('click', function () {
        window.location.href = "./../Pages/AddGame.html";
    });
    $("#update-game-btn").on('click', function () {
        window.location.href = "./../Pages/UpdateGame.html";
    });
    $("#logout-btn").on("click", function () {
        localStorage.clear();
        window.location.href = "./../Pages/LoginPage.html";
    });
    getGames(api);
});

function RemoveFromMyGames(element) {
    let idtoremove = localStorage.getItem("ID");
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Users/DeleteUserGame/" + idtoremove + "/" + element.id;
    ajaxCall("DELETE", api, JSON.stringify(element.id), function() {
        RemoveFromMyGamesSuccess(element.id);
    }, RemoveFromMyGamesErr);
}

function RemoveFromMyGamesSuccess(deletedId){
    allGames = allGames.filter(game => game.id !== deletedId);
    applyFilters();
}

function RemoveFromMyGamesErr(err) {
    console.log(err);
}