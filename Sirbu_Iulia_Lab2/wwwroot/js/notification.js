
const messages = document.getElementById('messages');
if (!messages) {
    console.error('Messages element not found!');
}


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Notification")
    .configureLogging(signalR.LogLevel.Information)
    .build();


connection.on('newMessage', (sender, messageText) => {
    console.log(`${sender}:${messageText}`);

    
    const newMessage = document.createElement('li');
    newMessage.textContent = `${sender}: ${messageText}`;
    messages.appendChild(newMessage);
});


connection.start()
    .then(() => {
        console.log('SignalR connection established.');
    })
    .catch(err => {
        console.error('SignalR connection failed:', err);
    });
