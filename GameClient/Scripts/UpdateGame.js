$(document).ready(function () {
    $("#UpdateGameForm").submit(Updatation)
    renderTagsFromLocalStorage();

})

function Updatation() {
    let id = parseInt($("#gameId").val());
    let name = $("#gameName").val();
    let steamURL = $("#steamUrl").val();
    let img = $("#imageUrl").val();
    let releaseDate = $("#releaseDate").val();
    let reviewSummary = $("#reviewSummary").val();
    let price = parseFloat($("#price").val());
    let tags = [];
    $("input[name='tags']:checked").each(function () {
        tags.push($(this).val());
    });
    let windows = $("#windows").prop("checked");
    let mac = $("#mac").prop("checked");
    let linux = $("#linux").prop("checked");
    let game = {
        Id: id,
        Name: name,
        SteamURL: steamURL,
        capsuleImage: img,
        ReleaseDate: releaseDate,
        ReviewSummary: reviewSummary + "%",
        Price: price,
        Tags: tags,
        Windows: windows,
        Mac: mac,
        Linux: linux
    }
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games/" + id;
    ajaxCall("PUT", api, JSON.stringify(game), successupdate, errorrupdate);
    return false;
}
function successupdate(data) {
    if (data == 0) {
        alert("No game with this id exists");
    }
    else {
        alert("Game Updated");

    }
}

function errorrupdate(err) {
    alert("Failed to Update the game");
}

function renderTagsFromLocalStorage() {
    let tagsDiv = $(".tags-checkbox-group");
    tagsDiv.empty();

    let tags = JSON.parse(localStorage.getItem("AllTags"));

    if (tags == null || tags.length === 0) {
        return;
    }

    for (let i = 0; i < tags.length; i++) {
        let tag = tags[i];

        let checkbox = `
            <label class="tag-checkbox">
                <input type="checkbox" name="tags" value="${tag}">
                ${tag}
            </label>
        `;

        tagsDiv.append(checkbox);
    }
}

$(document).ready(function () {
    renderTagsFromLocalStorage();
});