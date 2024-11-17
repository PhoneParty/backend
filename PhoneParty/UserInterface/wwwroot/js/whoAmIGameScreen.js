const lobbyId = getUrlParams();
const userId = localStorage.getItem("userId");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
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
    this.http.get("/game")
        .subscribe(res => {
            console.log(res);
        })
})

connection.on("ChangeTurn", (role, character) => {
    changeCharacter(character)
    changeState(role)
});

connection.on("GameEnd", _ => {
    handleGameEnd()
});

const guessScreen = document.getElementById("guess-screen")
const viewScreen = document.getElementById("view-screen")
const judgeScreen = document.getElementById("judge-screen")

function changeState(playerRole) {
    switch (playerRole) {
        case playerRole.Observer:
            viewScreen.classList.add("visually-hidden")
            judgeScreen.classList.add("visually-hidden")
            guessScreen.classList.remove("visually-hidden")
            break;
        case playerRole.Player:
            guessScreen.classList.add("visually-hidden")
            judgeScreen.classList.add("visually-hidden")
            viewScreen.classList.remove("visually-hidden")
            break;
        case playerRole.Guesser:
            guessScreen.classList.add("visually-hidden")
            viewScreen.classList.remove("visually-hidden")
            judgeScreen.classList.remove("visually-hidden")
            break;
        default:
            alert("Wrong state received!!!");
    }
}

const characterNameElement = document.getElementById("characterName")
const characterImageElement = document.getElementById("characterImage")

function changeCharacter(character) {
    characterNameElement.innerHTML = character.Name
    characterImageElement.src = character.Picture.Fullname
}

function handleGameEnd() {
    window.location.assign("/gameEnd");
}

const wrongGuessButton = document.getElementById("wrongGuessButton")
const rightGuessButton = document.getElementById("rightGuessButton")

wrongGuessButton.onclick = function () {
    socket.send({"judgeTurn": false})
}
rightGuessButton.onclick = function () {
    socket.send({"judgeTurn": true})
}
