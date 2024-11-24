const lobbyId = getUrlParams();
const userId = localStorage.getItem("userId");
navigator.storage.persist()

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

connection.on("ShowTurn", (role, isDecisionMaker, character) => {
    changeCharacter(character)
    changeState(role, isDecisionMaker)
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

function changeState(playerRole, isDesisionMaker) {
    console.log(playerRole, isDesisionMaker)
    if (isDesisionMaker) {
        guessScreen.classList.add("visually-hidden")
        viewScreen.classList.remove("visually-hidden")
        judgeScreen.classList.remove("visually-hidden")
    }
    else{
        switch (playerRole) {
            case 0:
                viewScreen.classList.remove("visually-hidden")
                judgeScreen.classList.add("visually-hidden")
                guessScreen.classList.add("visually-hidden")
                break;
            case 1:
                viewScreen.classList.remove("visually-hidden")
                judgeScreen.classList.add("visually-hidden")
                guessScreen.classList.add("visually-hidden")
                break;
            case 2:
                guessScreen.classList.remove("visually-hidden")
                viewScreen.classList.add("visually-hidden")
                judgeScreen.classList.add("visually-hidden")
                break;
            default:
                alert("Wrong state received!!!");
        }
    }
}

const characterNameElement = document.getElementById("characterName")
const characterImageElement = document.getElementById("characterImage")


function changeCharacter(character) {
    console.log(character)
    characterNameElement.innerHTML = character.name
    characterImageElement.src = "Characters\\" + character.picture
}

function handleGameEnd() {
    window.location.assign("/FinalPage?lobbyId=" + lobbyId);
}

function getUrlParams() {
    const params = new URLSearchParams(window.location.search);
    return params.get("lobbyId");
}

function wrongGuessButton() {
    connection.invoke("ChangeTurn", userId ,lobbyId,false)
        .catch(err => console.error("Ошибка при смене хода: " + err.toString()));
}
function rightGuessButton() {
    connection.invoke("ChangeTurn", userId ,lobbyId, true)
        .catch(err => console.error("Ошибка при смене хода: " + err.toString()));
}
