$(document).ready(function () {
    $("#myLoginForm").submit(redirect)
    $("#myRegisterForm").submit(registration)
    $("#Guest").click(function () {
        window.location.href = "./../Pages/index.html";
    });
})

function redirect() {
    let Email = $("#EmailLogin").val();
    let Password = $("#PasswordLogin").val();
    let LoginPair = {
        Email: Email,
        Password: Password
    }
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Users/Login";
    ajaxCall("POST", api, JSON.stringify(LoginPair), successredirect, errorredirect);
    return false;
}
function successredirect(data) {
    //console.log(data.userName);
    localStorage.setItem("UserName", data.userName);
    localStorage.setItem("ID", data.idNum);
    window.location.href = "./../Pages/index.html";
}
function errorredirect(err) {
    console.log("faileedtologin")
    console.log(err);
}
function registration() {
    let id = 0;
    let name = $("#NameReg").val();
    let email = $("#EmailReg").val();
    let password = $("#PassReg").val();
    let active = $("#active-user").prop("checked");
    let user = {
        Id: id,
        Name: name,
        Email: email,
        Password: password,
        Active: active
    }
    localStorage.setItem("UserName", name);
    let api = "https://proj.ruppin.ac.il/cgroup21/test2/tar1/api/Users";
    ajaxCall("POST", api, JSON.stringify(user), successregistration, errorrregistration);
    return false;
}
function successregistration(data) {
    localStorage.setItem("ID", data.useridtoreturn);
    window.location.href = "./../Pages/index.html";
}

function errorrregistration(err) {
    alert("Use Unique email");
    console.log(err);
}