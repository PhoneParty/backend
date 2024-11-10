const lobbyId = getUrlParams();
const userName = getCookie("userName");
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start()
    .then(() => {
        console.log("Подключено к SignalR");
        if (lobbyId && userName != null) {
            connection.invoke("JoinLobby", lobbyId, userName)
                .catch(err => console.error("Ошибка при подключении к лобби: " + err.toString()));
        }
    })

connection.onreconnected(() => {
    this.http.get("/lobbyHub")
        .subscribe(res => {
            console.log(res);
        })
})

connection.on("UserJoined", (userId, users) => {
    updateUserList(users);
});

connection.on("UserLeft", (userId, users) => {
    updateUserList(users);
});

function getUrlParams() {
    const params = new URLSearchParams(window.location.search);
    return params.get("lobbyId");
}

function updateUserList(users) {
    const userList = document.getElementById("userList");
    userList.innerHTML = "";
    users.forEach(user => {
        const li = document.createElement("li");
        li.textContent = `Пользователь ${user}`;
        userList.appendChild(li);
    });
}

function leaveLobby() {
    connection.invoke("LeaveLobby", lobbyId).then(() => {
        connection.stop();
        window.location.href = "/";
    }).catch(err => console.error("Ошибка при выходе из лобби: " + err.toString()));
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}