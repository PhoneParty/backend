localStorage.clear()
let userId = localStorage.getItem("userId");
navigator.storage.persist()

// Инициализация подключения к SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/lobbyHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Запуск подключения
connection.start()
    .then(() => {
        if (!userId) {
            connection.invoke("RegisterUser")
                .catch(err => console.error("Ошибка при регистрации пользователя: " + err.toString()));
        }
        else {
            connection.invoke("UpdateUserConnection", userId)
                .catch(err => console.error("Ошибка при подключении пользователя: " + err.toString()));
        }
        console.log("Подключение установлено");
    })
    .catch(err => console.error("Ошибка соединения: " + err.toString()));

connection.onreconnected(() => {
    this.http.get("/lobbyHub")
        .subscribe(res => {
            console.log(res);
        })
})

function createLobby() {
    const userName = document.getElementById("userNameInput").value;
    if (userName && connection.state === "Connected")   {
        connection.invoke("UpdateUserName", userId, userName)
            .catch(err => console.error("Ошибка при добавлении имени пользователя: " + err.toString()));
        connection.invoke("CreateLobby", userId)
            .catch(err => console.error("Ошибка при создании лобби: " + err.toString()));
    } else if (connection.state !== "Connected") {
        console.warn("Подключение к серверу не установлено");
    }
}


connection.onclose = () => {
    console.log("WebSocket is closed now.");
};


function joinLobby() {
    const lobbyId = document.getElementById("lobbyIdInput").value.toUpperCase();
    const userName = document.getElementById("userNameInput").value;
    if (lobbyId && userName && connection.state === "Connected") { // Проверка подключения
        connection.invoke("UpdateUserName", userId, userName)
            .catch(err => console.error("Ошибка при добавлении имени пользователя: " + err.toString()));
        connection.invoke("JoinLobby", lobbyId, userId)
            .catch(err => console.error("Ошибка при добавлении в лобби: " + err.toString()));
    } else if (connection.state !== "Connected") {
        console.warn("Подключение к серверу не установлено");
    } else {
        alert("Введите номер лобби!");
    }
}

connection.on("LobbyJoinAccept", (lobbyId, userName) => {
    setCookie("userName", userName, 1);
    window.location.href = `/Lobby?lobbyId=${lobbyId}`;
});

function setCookie(name, value, days) {
    const d = new Date();
    d.setTime(d.getTime() + (days*24*60*60*1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/";
}

connection.on("LobbyCreated", (lobbyId, userName) => {
    setCookie("userName", userName, 1);
    window.location.href = `/Lobby?lobbyId=${lobbyId}`;
});


connection.on("UserCreated", id => {
    userId = id
    localStorage.setItem("userId", userId);
});
