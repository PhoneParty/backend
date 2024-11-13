const lobbyId = getUrlParams();
const userId = localStorage.getItem("userId");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start()
    .then(() => {
        console.log("Подключено к SignalR");
        if (lobbyId) {
            connection.invoke("UpdateGroupConnection", userId, lobbyId)
                .catch(err => console.error("Ошибка при подключении пользователя: " + err.toString()));
            connection.invoke("UpdateLobby", lobbyId)
                .catch(err => console.error("Ошибка при обновлении лобби: " + err.toString()));
        }
    })

connection.onreconnected(() => {
    this.http.get("/lobbyHub")
        .subscribe(res => {
            console.log(res);
        })
})

connection.on("UpdateLobbyUsers", users => {
    updateUserList(users);
});

connection.on("UserLeft", users => {
    updateUserList(users);
});

function getUrlParams() {
    const params = new URLSearchParams(window.location.search);
    return params.get("lobbyId");
}

const userListLine =  document.getElementById("guestLine");
const hostListLine =  document.getElementById("hostLine");

function updateUserList(users) {
    const userList = document.getElementById("userList");
    userList.innerHTML = "";
    users.forEach(user => {
        let newLine = userListLine.cloneNode(true);
        newLine.querySelector("#username").innerHTML = user;
        newLine.classList.remove("d-none");
        userList.appendChild(newLine);
    });
}

function leaveLobby() {
    connection.invoke("LeaveLobby", userId, lobbyId).then(() => {
        connection.stop();
        window.location.href = "/";
    }).catch(err => console.error("Ошибка при выходе из лобби: " + err.toString()));
}