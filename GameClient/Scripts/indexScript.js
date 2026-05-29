let allGames = []; // שמירת הנתונים לסינון
let selectedTags = [];
let searchtags = [];
let mytags = [];
let isTableView = false;

$(document).ready(function () {
    // טעינת משחקים והעלמת הכפתור
    $("#removeTagBtn").prop("disabled", true);
    $("#SearchByTags").prop("disabled", true);
    $("#Recomendation").prop("disabled", true);
    $("#addTagBtn").prop("disabled", true);
    $("#popform").submit(Edit)

    let userName = localStorage.getItem("UserName");

    if (userName) {
        console.log("success")
        $("#user-greeting").html("Hello " + userName);
        $("#Edit").show()
        $(".tag-filter").show()
        console.log(".show")
    } else {
        console.log("fail")
        $("#user-greeting").html("Hello Guest");
        $("#Edit").hide()
        $(".tag-filter").hide()
    }
    $("#logout-btn").on("click", function () {
        localStorage.clear(); 
        window.location.href = "./../Pages/LoginPage.html";
    });
    $("#load-btn").on("click", function () {
        let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games";
        getGames(api);
        $("#removeTagBtn").prop("disabled", false);
        $("#SearchByTags").prop("disabled", false);
        $("#Recomendation").prop("disabled", false);
        $("#addTagBtn").prop("disabled", false);
        $(this).hide();
    });

    $("#my-games-btn").on('click', function () {
        if (!localStorage.getItem("UserName")) {
            window.location.href = "./../Pages/LoginPage.html";
        }
        else {
            window.location.href = "./../Pages/myGames.html"
        }
    })
    GetAllTags();

    $("#viewToggle").change(function () {
        toggleGamesView();
    });

    document.getElementById("Edit").addEventListener("click", function () {
        document.getElementById("myForm").showModal();
    });

});


function closeForm() {
    document.getElementById("myForm").close();
}


function GetAllTags() {
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games/GetAllTags";
    ajaxCall("GET", api, "", SuccessTags, FailTags);
}

function SuccessTags(data) {
    console.log("got the tags");
    selectedTags = data;
    localStorage.setItem("AllTags", JSON.stringify(selectedTags));
    $("#tagsList").empty();

    for (let i = 0; i < selectedTags.length; i++) {
        $("#tagsList").append(
            `<option value="${selectedTags[i]}"></option>`
        );
    }
}

function FailTags(err) {
    console.log(err);
}




function AddtoMyGames(element) {
    let userid = localStorage.getItem("ID");
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Users/InsertToUserGameTable/" + userid + "/" + element.id;
    console.log("toMyGames says:" + element.name)
    ajaxCall("POST", api, null, AddToMyGamesSuccess, AddToMyGamesErr);
}

function createGameObject(id, name, steamUrl, img, releaseDate, reviewSummary, price, tags, windows, mac, linux) {
    console.log(name)
    gameObject = {
        Id: id,
        Name: name,
        SteamURL: steamUrl,
        Img: img,
        ReleaseDate: releaseDate,
        ReviewSummary: reviewSummary + "%",
        Price: price,
        Tags: tags,
        Windows: windows,
        Mac: mac,
        Linux: linux
    }
    return gameObject;
}

function AddToMyGamesSuccess() {
    console.log("api has been called")

    
}
function AddToMyGamesErr(err) {
    console.log("STATUS:", err.status);
    console.log("RESPONSE:", err.responseText);
    console.log("FULL ERROR:", err);

    alert("Failed to add game. Check console.");
}


function addSelectedTag() {
    let tag = $("#tagSelect").val();

    // nothing selected
    if (tag === "") {
        return;
    }

    // already exists, do nothing
    if (searchtags.includes(tag)) {
        return;
    }

    // add one tag
    searchtags.push(tag);

    // render after add
    renderSelectedTagsList();
}

function removeSelectedTag() {
    let tag = $("#Removetags").val();

    // nothing selected
    if (tag === "") {
        return;
    }

    // remove one tag
    searchtags = searchtags.filter(t => t !== tag);

    // render after remove
    renderSelectedTagsList();
}

function renderSelectedTagsList() {
    $("#Removetags").empty();

    $("#Removetags").append(`<option value="">Selected tags...</option>`);

    for (let i = 0; i < searchtags.length; i++) {
        $("#Removetags").append(
            `<option value="${searchtags[i]}">${searchtags[i]}</option>`
        );
    }
}

//function searchByTags() {
//    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games/GetAllTags";
//    ajaxCall("GET", api, "", SuccessTags, FailTags);
//}

//function SuccessTags(data) {
//    console.log("got the tags");
//    selectedTags = data;
//    for (let i = 0; i < selectedTags.length; i++) {
//        $("#tagSelect").append(
//            `<option value="${selectedTags[i]}">${selectedTags[i]}</option>`
//        );
//    }
//}

//function FailTags(err) {
//    console.log(err);
//}


function searchByTags() {
    // if no tags selected, do nothing for now
    if (searchtags.length === 0) {
        console.log("No tags selected");
        let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games";
        getGames(api);
        return;
    }

    // turns ["Action", "RPG", "Open World"] into "Action,RPG,Open World"
    let tagsString = searchtags.join(",");

    // build the API URL safely
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games/GetByTags?tags=" + encodeURIComponent(tagsString);

    ajaxCall("GET", api, "", SearchByTagsSuccess, SearchByTagsError);
}

function SearchByTagsSuccess(games) {
    console.log("games by tags:", games);
    console.log("games by tags:", games[0]);

    // use your existing render function here
    renderGames(games);
}

function SearchByTagsError(xhr) {
    console.log("status:", xhr.status);
    console.log("response:", xhr.responseText);
}


function RecomendationByTags() {
    let userId = localStorage.getItem("ID");

    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games/recommendations/" + userId;

    ajaxCall("GET", api, "", GetRecommendationsSuccess, GetRecommendationsError);
}

function GetRecommendationsSuccess(games) {
    console.log("recommended games:", games);

    renderGames(games);
}

function GetRecommendationsError(xhr) {
    console.log("status:", xhr.status);
    console.log("response:", xhr.responseText);
}



function toggleGamesView() {
    isTableView = !isTableView;

    if (isTableView) {
        $(".card-wrapper").hide();
        $("#tableView").show();

        $("#viewText").text("Table View");

        // important if tableView contains DataTable
        if ($.fn.DataTable.isDataTable("#gamesTable")) {
            $("#gamesTable").DataTable().columns.adjust();
        }

    } else {
        $("#tableView").hide();
        $(".card-wrapper").show();

        $("#viewText").text("Cards View");
    }
}

// mark the selected row
function markSelected(btn) {
    $("#gamesTable tbody tr").removeClass("selected-row");

    let row = btn.parentNode.parentNode;

    $(row).addClass("selected-row");
}


function editPrice(btn, gameId) {
    markSelected(btn);

    let gameupdate = allGames.find(g => g.id === gameId);

    if (gameupdate == null) {
        swal("Error", "Game not found", "error");
        return;
    }

    let row = btn.parentNode.parentNode;
    let currentPrice = row.cells[2].innerText;

    swal({
        title: "Update Price",
        text: "Enter the new price:",
        content: {
            element: "input",
            attributes: {
                type: "number",
                value: currentPrice,
                min: 0,
                step: 0.01
            }
        },
        buttons: {
            cancel: "Cancel",
            confirm: "Update"
        }
    }).then(function (value) {

        // remove selected mark after approve OR cancel
        $("#gamesTable tbody tr").removeClass("selected-row");

        // user clicked cancel
        if (value === null) {
            return;
        }

        // validation
        if (value === "") {
            swal("Error", "Please enter a price", "error");
            return;
        }

        if (Number(value) < 0) {
            swal("Error", "Price cannot be negative", "error");
            return;
        }

        let newPrice = Number(value);

        // change the object
        gameupdate.price = newPrice;

        // change the table visually
        row.cells[2].innerText = newPrice;

        console.log("Updated game:", gameupdate);

        let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games/" + gameId;

        ajaxCall("PUT",api,JSON.stringify(gameupdate),succesupdate,errorrupdate);
    });
}
    function succesupdate() {
        alert("Price Updated");
    }

    function errorrupdate(err) {
        alert("Failed to update the Price");
}


// Creates color from red = low score to green = high score
function getReviewColor(reviewSummary) {
    let percent = parseFloat(reviewSummary); // "85%" -> 85

    if (isNaN(percent)) {
        return "white";
    }

    percent = Math.max(0, Math.min(100, percent));

    let red = { r: 220, g: 38, b: 38 };     // low score color
    let green = { r: 22, g: 163, b: 74 };   // high score color

    let ratio = percent / 100;

    let r = Math.round(red.r + (green.r - red.r) * ratio);
    let g = Math.round(red.g + (green.g - red.g) * ratio);
    let b = Math.round(red.b + (green.b - red.b) * ratio);

    return `rgb(${r}, ${g}, ${b})`;
}

function Edit() {
    let id = localStorage.getItem("ID");
    let name = $("#popname").val();
    let email = "string";
    let password = $("#poppass").val();
    let active = true;
    let user = {
        Id: id,
        Name: name,
        Email: email,
        Password: password,
        Active: active
    }
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Users/" + id;
    ajaxCall("PUT", api, JSON.stringify(user), successregistration, errorrregistration);
    return false;
}
function successregistration(name) {
    localStorage.setItem("UserName", name.userName);
    location.reload();
    alert("Deatails Updated ");
}

function errorrregistration(err) {
    alert("Failed to update the user");
}
































