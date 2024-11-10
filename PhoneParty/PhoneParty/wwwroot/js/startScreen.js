// Инициализация подключения к SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Запуск подключения
connection.start()
    .then(() => {
        console.log("Подключение установлено");
    })
    .catch(err => console.error("Ошибка соединения: " + err.toString()));

connection.onreconnected(() => {
    this.http.get("/lobbyHub")
        .subscribe(res => {
            console.log(res);
        })
})

// Метод для создания лобби
function createLobby() {
    const userName = document.getElementById("userNameInput").value;
    if (userName && connection.state === "Connected")   {
        connection.invoke("CreateLobby", userName)
            .catch(err => console.error("Ошибка при создании лобби: " + err.toString()));
    } else if (connection.state !== "Connected") {
        console.warn("Подключение к серверу не установлено");
    }
}

// Метод для подключения к лобби
function joinLobby() {
    const lobbyId = document.getElementById("lobbyIdInput").value;
    const userName = document.getElementById("userNameInput").value;
    if (lobbyId && connection.state === "Connected") { // Проверка подключения
        setCookie("userName", userName, 1);
        window.location.href = `/Lobby?lobbyId=${lobbyId}`;
    } else if (connection.state !== "Connected") {
        console.warn("Подключение к серверу не установлено");
    } else {
        alert("Введите номер лобби!");
    }
}

function setCookie(name, value, days) {
    const d = new Date();
    d.setTime(d.getTime() + (days*24*60*60*1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/";
}

// Переход в лобби после создания
connection.on("LobbyCreated", (lobbyId, userName) => {
    setCookie("userName", userName, 1);
    window.location.href = `/Lobby?lobbyId=${lobbyId}`;
});

// connection.on("JoinedToLobby", (lobbyId, userName) => {
//     setCookie("userName", userName, 1);
//     window.location.href = `/Lobby?lobbyId=${lobbyId}`;
// });

connection.on("UserJoined", (userId, users) => {
    const status = document.getElementById("status");
    status.innerText += `\nПользователь ${userId} присоединился к лобби.`;
});