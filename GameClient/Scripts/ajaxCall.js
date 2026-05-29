function ajaxCall(method, api, data, successCB, errorCB) {
    $.ajax({
        type: method,
        url: api,
        data: data,
        cache: false,
        contentType: "application/json",
        dataType: "json",
        success: successCB,
        error: errorCB
    });
}

function getGames(api) {
    ajaxCall("GET", api, "", successGetGamesCB, errorGetGamesCB);
}

function successGetGamesCB(games) {
    allGames = games;
    renderGames(allGames);
}

function errorGetGamesCB(err) {
    console.log(err);
}

function renderGames(games) {
    let tableBody = $("#gamesTableBody");
    if ($("#gamesTable").length > 0) {
        if ($.fn.DataTable.isDataTable("#gamesTable")) {
            $("#gamesTable").DataTable().clear().destroy();
        }
    }
    tableBody.empty();
    $(".card-wrapper").empty();

    if (!games || games.length === 0) {
        $(".card-wrapper").append("<h3>No games found</h3>");
        return;
    }
    
    for (let i = 0; i < games.length; i++) {
        const element = games[i];
        const card = $("<div>").addClass("card-style").attr('id', element.id);
        const title = $("<h3>").addClass("game-title").text(element.name);

        // שורות נתונים
        const priceRow = $("<div>").addClass("info-row").append(
            $("<span>").addClass("label").text("Price: "),
            $("<span>").addClass("value price").text(element.price === 0 ? "Free" : "$" + element.price)
        );

        const reviewRow = $("<div>").addClass("info-row").append(
            $("<span>").addClass("label").text("Reviews: "),
            $("<span>").addClass("value score").text(element.reviewSummary)
        );

        const dateRow = $("<div>").addClass("info-row").append(
            $("<span>").addClass("label").text("Release Date: "),
            $("<span>").addClass("value").text(element.releaseDate)
        );

        const idRow = $("<div>").addClass("info-row").append(
            $("<span>").addClass("label").text("App ID: "),
            $("<span>").addClass("value").text(element.id)
        );

        // OS
        const osContainer = $("<div>").addClass("os-container");
        if (element.windows) osContainer.append($("<span>").addClass("os-badge").text("Windows"));
        if (element.mac) osContainer.append($("<span>").addClass("os-badge").text("Mac"));
        if (element.linux) osContainer.append($("<span>").addClass("os-badge").text("Linux"));

        // Tags
        const tagsContainer = $("<div>").addClass("tags-container");
        for (let j = 0; j < element.tags.length; j++) {
            tagsContainer.append($("<span>").addClass("tag").text(element.tags[j]));
        }

        let img;
        let addToCartBtn;
        let deleteFromCartBtn
        if (window.location.pathname.includes("index.html")) {
            img = $("<img>").addClass("game-img").attr("src", element.capsuleImage);
            addToCartBtn = $("<button>").addClass("add-btn").text("Add To My Games").attr('id', element.id).on('click', function () {
                if (!localStorage.getItem("UserName")) {
                    window.location.href = "./../Pages/LoginPage.html";
                }
                else {
                    AddtoMyGames(element)
                }
            });
        } else if (window.location.pathname.includes("myGames.html")) {
            img = $("<img>").addClass("game-img").attr("src", element.capsuleImage);
            addToCartBtn = $("<button>").addClass("add-btn").addClass("remove-btn").text("Remove From My Games").attr('id', element.id).on('click', function () { RemoveFromMyGames(element) });
        }


        const steamBtn = $("<a>").addClass("steam-btn")
            .attr("href", element.steamUrl).attr("target", "_blank").text("Open on Steam");

        card.append(img, title, priceRow, reviewRow, dateRow, idRow, osContainer, tagsContainer, addToCartBtn, steamBtn);
        $(".card-wrapper").append(card);

        let row = `
            <tr>
                <td><img src="${element.capsuleImage}" alt="${element.name}"></td>
                <td>${element.name}</td>
                <td>${element.price}</td>
                <td>${element.releaseDate}</td>
                <td>${element.windows}</td>
                <td>${element.mac}</td>
                <td>${element.linux}</td>
                <td><button type="button" class="EditPrice" onclick="editPrice(this, ${element.id})">Edit</button></td>
                <td>${element.reviewSummary}</td>
            </tr>
        `

        tableBody.append(row);
    }

    if ($("#gamesTable").length > 0) {
        $('#gamesTable').DataTable({
            rowCallback: function (row) {
                let reviewText = $('td:eq(8)', row).text();
                let color = getReviewColor(reviewText);

                $('td:eq(8)', row).css({
                    "background-color": color,
                    "color": "white",
                    "font-weight": "bold",
                    "text-align": "center"
                });
            }
        });
    }

}
$(document).ready(function () {

    $("#toolbar").on("mouseenter", ".os-btn, .special-btn", function () {
        $(this).addClass("btn-hover");
    });

    $("#toolbar").on("mouseleave", ".os-btn, .special-btn", function () {
        $(this).removeClass("btn-hover");
    });

    // לחיצה (נשאר צבוע) וסינון
    $("#toolbar").on("click", ".os-btn, #free-only-btn", function () {
        $(this).toggleClass("active");
        applyFilters();
    });

    // חיפוש ומיון
    $("#search-input").on("input", applyFilters);
    $("#sort-select").on("change", applyFilters);

    // איפוס
    $("#reset-filters").on("click", function () {
        $(".active").removeClass("active");
        $("#search-input").val("");
        $("#sort-select").val("none");
        renderGames(allGames);
    });
});



function applyFilters() {
    let filtered = allGames.slice();

    let searchVal = $("#search-input").val().toLowerCase();
    if (searchVal) {
        filtered = filtered.filter(function (game) {
            return game.name.toLowerCase().includes(searchVal);
        });
    }

    let activeOSButtons = $(".os-btn.active");
    if (activeOSButtons.length > 0) {
        filtered = filtered.filter(function (game) {
            let hasMatch = false;
            activeOSButtons.each(function () {
                let osType = $(this).data("os");
                if (game[osType] === true) {
                    hasMatch = true;
                }
            });
            return hasMatch;
        });
    }

    if ($("#free-only-btn").hasClass("active")) {
        filtered = filtered.filter(function (game) {
            return game.price === 0;
        });
    }

    let sortType = $("#sort-select").val();
    if (sortType === "name") {
        filtered.sort(function (a, b) { return a.name.localeCompare(b.name); });
    } else if (sortType === "price-low") {
        filtered.sort(function (a, b) { return a.price - b.price; });
    } else if (sortType === "price-high") {
        filtered.sort(function (a, b) { return b.price - a.price; });
    }

    renderGames(filtered);
}

//// 3. סינון חינמיים
//if ($("#free-only-btn").hasClass("active")) {
//    filtered = filtered.filter(function (game) {
//        return game.price === 0;
//    });
//}

//// 4. מיון
//let sortType = $("#sort-select").val();
//if (sortType === "name") {
//    filtered.sort(function (a, b) { return a.name.localeCompare(b.name); });
//} else if (sortType === "price-low") {
//    filtered.sort(function (a, b) { return a.price - b.price; });
//} else if (sortType === "price-high") {
//    filtered.sort(function (a, b) { return b.price - a.price; });
//}

//renderGames(filtered);