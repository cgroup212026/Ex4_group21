$(document).ready(function () {
    $("#addGameForm").submit(addition)
    renderTagsFromLocalStorage();

})

function addition() {
    let id = 0;
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
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Games";
    ajaxCall("POST", api, JSON.stringify(game), successadd, errorradd);
    return false;
}
function successadd() {

    alert("Game Added");
}

function errorradd(err) {
    alert("Failed to add the game");
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