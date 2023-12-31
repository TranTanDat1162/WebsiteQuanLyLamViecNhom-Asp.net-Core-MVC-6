"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    li.classList.add("line-height", "font-size-13","pb-2");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user}: ${message}`;
    updateScroll()
});

connection.on("ReceiveNotification", function (user, message, imgId, timestamp) {

    // Create the link element within the notification
    var notificationLink = document.createElement("a");
    notificationLink.href = "#"; // Adjust the link URL if needed
    notificationLink.classList.add("iq-sub-card");

    // Create the media card element
    var mediaCard = document.createElement("div");
    mediaCard.classList.add("media", "align-items-center", "cust-card", "py-3", "border-bottom", "pr-2", "pl-2");

    // Create the image container
    var imgContainer = document.createElement("div");

    // Create the image element
    var img = document.createElement("img");
    img.classList.add("avatar-50", "rounded-small");
    img.src = `https://drive.google.com/uc?id=${imgId}&export=download`;
    img.alt = "01";
    imgContainer.appendChild(img);

    // Create the media body container
    var mediaBody = document.createElement("div");
    mediaBody.classList.add("media-body", "ml-3");

    // Create the header with name and timestamp
    var headerDiv = document.createElement("div");
    headerDiv.classList.add("d-flex", "align-items-center", "justify-content-between");

    var headerName = document.createElement("h6");
    headerName.textContent = user;
    headerName.classList.add("mb-0");

    var headerTimestamp = document.createElement("small");
    headerTimestamp.textContent = timestamp;
    headerTimestamp.classList.add("text-dark", "mb-0");
    headerTimestamp.style.fontWeight = "bold";

    headerDiv.appendChild(headerName);
    headerDiv.appendChild(headerTimestamp);

    // Create the message element
    var messageElement = document.createElement("small");
    messageElement.textContent = message;
    messageElement.classList.add("mb-0");

    mediaBody.appendChild(headerDiv);
    mediaBody.appendChild(messageElement);

    // Append elements to their respective containers
    mediaCard.appendChild(imgContainer);
    mediaCard.appendChild(mediaBody);
    notificationLink.appendChild(mediaCard);

    // Append the notification to the messages list
    var notifyblock = document.getElementById("notification-block");

    notifyblock.appendChild(notificationLink);

    // Update the scroll
    updateScroll();
});


connection.start().then(function () {
    var room = document.getElementById("roomInput").value;

    connection.invoke("AddToGroup", room.toString()).catch(function (err) {
        return console.error(err.toString());
    });

    connection.invoke("GetChatHistory", room.toString()).catch(function (err) {
        return console.error(err.toString());
    });

    var studentId = document.getElementById("studentInput").value;

    if (studentId != null) {
        connection.invoke("GetClassNotification", studentId).catch(function (err) {
            return console.error(err.toString());
        });
    }

    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var room = document.getElementById("roomInput").value;
    connection.invoke("SendMessage", user, room, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function updateScroll() {
    var element = document.getElementById("messagesList");
    element.scrollTop = element.scrollHeight;
}