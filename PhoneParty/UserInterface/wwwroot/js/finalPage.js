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
            connection.invoke("CheckHost", userId, lobbyId)
                .catch(err => console.error("Ошибка при проверке пользователя на хоста: " + err.toString()));
        }
    })

connection.onreconnected(() => {
    this.http.get("/lobbyHub")
        .subscribe(res => {
            console.log(res);
        })
})

connection.on("GameStarted", () =>{
    window.location.href = `/Game?lobbyId=${lobbyId}`;
})

function toLobby() {
    window.location.assign("/Lobby?lobbyId=" + lobbyId);
}

function getUrlParams() {
    const params = new URLSearchParams(window.location.search);
    return params.get("lobbyId");
}