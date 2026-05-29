$(document).ready(function () {
    $("#EditProfileForm").submit(Edit)

})

function Edit() {
    let id = localStorage.getItem("ID");
    let name = $("#UserName").val();
    let email = "string";
    let password = $("#UserPass").val();
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
    alert("Deatails Updated ");
}

function errorrregistration(err) {
    alert("Failed to update the user");
}