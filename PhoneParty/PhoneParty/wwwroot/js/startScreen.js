// Создание подключения к SignalR хабу
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat") // Укажите правильный путь к вашему хабу на сервере, например "/chatHub"
    .build();

// Подключение к хабу
connection.start()
    .then(() => console.log("SignalR is connected!!!"))
    .catch(err => console.error("Error establishing SignalR connection:", err));

// Обработка входящего сообщения
connection.on("ReceiveMessage", (data) => {
    const parsedData = typeof data === 'string' ? JSON.parse(data) : data; // Проверка на JSON строку
    if ("joinRequestAccept" in parsedData) {
        handleJoinRequestAccept(parsedData);
    }
});

// Обработка нажатия кнопки
let button = document.getElementById('play_button');
button.onclick = function () {
    // Отправка сообщения на сервер
    connection.invoke("SendMessage", { "joinRequest": 1234 }) // Укажите правильное название метода, например "SendMessage"
        .catch(err => console.error("Error sending message:", err));
};

// Функция для обработки принятия запроса на присоединение
function handleJoinRequestAccept(parsedData) {
    if (parsedData["joinRequestAccept"]) {
        window.location.assign(parsedData["redirect"]);
    }
}
