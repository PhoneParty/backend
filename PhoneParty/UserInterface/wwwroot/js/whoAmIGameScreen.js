const lobbyId = getUrlParams();
const userId = localStorage.getItem("userId");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/WhoIAmHub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();


connection.start()
    .then(() => {
        console.log("Подключено к SignalR");
        if (lobbyId) {
            connection.invoke("UpdateGroupConnection", userId, lobbyId)
                .catch(err => console.error("Ошибка при подключении пользователя: " + err.toString()));
            connection.invoke("ShowTurnInfo", userId, lobbyId)
                .catch(err => console.error("Ошибка при обновлении инфы о ходе: " + err.toString()));
        }
    })

connection.onreconnected(() => {
    this.http.get("/game")
        .subscribe(res => {
            console.log(res);
        })
})

connection.on("ShowTurn", (role, character) => {
    changeCharacter(character)
    changeState(role)
});

connection.on("ChangeTurn" , ()=> {
    connection.invoke("ShowTurnInfo", userId, lobbyId)
        .catch(err => console.error("Ошибка при обновлении инфы о ходе: " + err.toString()));
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

function getUrlParams() {
    const params = new URLSearchParams(window.location.search);
    return params.get("lobbyId");
}

const wrongGuessButton = document.getElementById("wrongGuessButton")
const rightGuessButton = document.getElementById("rightGuessButton")

wrongGuessButton.onclick = function () {
    connection.invoke("ChangeTurn", false)
        .catch(err => console.error("Ошибка при смене хода: " + err.toString()));
}
rightGuessButton.onclick = function () {
    connection.invoke("ChangeTurn", true)
        .catch(err => console.error("Ошибка при смене хода: " + err.toString()));
}
