"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, userId) {
    var messageContainer = document.createElement("div");

    var username = document.createElement("div");
    username.classList.add("chat-user", "line-height");
    username.textContent = user + ":";

    var messageText = document.createElement("span");
    messageText.classList.add("chat-content", "rounded", "bg-primary", "text-white", "p-2", "border-0", "mt-0");
    messageText.textContent = message;

    // Xác định hướng tin nhắn dựa trên người dùng
    var test = document.getElementById("userInput").value;
    var isOwnMessage = (userId === document.getElementById("userInput").value);

        // Thêm hoặc xóa class "float-right" tùy thuộc vào điều kiện
    messageContainer.classList.toggle("text-right", isOwnMessage);
    messageText.classList.toggle("bg-dark-light", !isOwnMessage);  // Nếu không phải tin nhắn của người gửi thì thêm màu nền
    messageContainer.appendChild(username);
    messageContainer.appendChild(messageText);

    document.getElementById("messagesList").appendChild(messageContainer);

    updateScroll();
});

connection.on("ReceiveNotification", function (user, message, imgId, timestamp) {
    // Tạo phần hiển thị thông báo
    var notificationLink = document.createElement("a");
    notificationLink.href = "#"; // Điều chỉnh URL nếu cần
    notificationLink.classList.add("iq-sub-card");

    var mediaCard = document.createElement("div");
    mediaCard.classList.add("media", "align-items-center", "cust-card", "py-3", "border-bottom", "pr-2", "pl-2");

    var imgContainer = document.createElement("div");
    var img = document.createElement("img");
    img.classList.add("avatar-50", "rounded-small");
    img.src = `https://drive.google.com/uc?id=${imgId}&export=download`;
    img.alt = "01";
    imgContainer.appendChild(img);

    var mediaBody = document.createElement("div");
    mediaBody.classList.add("media-body", "ml-3");

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

    var messageElement = document.createElement("small");
    messageElement.textContent = message;
    messageElement.classList.add("mb-0");

    // Giới hạn ký tự cho nội dung
    var maxMessageLength = 10;
    var displayedMessage = message.length > maxMessageLength
        ? message.substring(0, maxMessageLength) + "..."
        : message;

    var messageElement = document.createElement("small");
    messageElement.textContent = displayedMessage;
    messageElement.classList.add("mb-0");

    mediaBody.appendChild(headerDiv);
    mediaBody.appendChild(messageElement);

    mediaCard.appendChild(imgContainer);
    mediaCard.appendChild(mediaBody);
    notificationLink.appendChild(mediaCard);

    // Thêm sự kiện nhấp chuột để hiển thị nội dung đầy đủ khi cần
    notificationLink.addEventListener("click", function (event) {
        // Ngăn chặn sự kiện click mặc định để tránh chuyển hướng
        event.preventDefault();

        // Tạo phần tử popup bên trái
        var popup = document.createElement("div");
        popup.classList.add("popup", "position-absolute", "bg-white", "p-3", "shadow", "rounded", "mr-5");
        popup.textContent = message;
        popup.style.maxWidth = "500px";
        popup.style.wordWrap = "break-word";
        // Hiển thị toàn bộ nội dung

        // Đặt vị trí của popup bên trái của media card
        var rect = notificationLink.getBoundingClientRect();
        popup.style.top = rect.top + "px";
        popup.style.left = rect.left - popup.offsetWidth - 10 + "px";

        // Thêm popup vào body
        document.body.appendChild(popup);

        // Thêm sự kiện click để đóng popup khi click ra ngoài
        var closeButton = document.createElement("button");
        closeButton.textContent = "Đóng";
        closeButton.classList.add("btn", "btn-secondary", "position-absolute", "bottom-0", "right-0", "mb-2", "ml-2");
        closeButton.addEventListener("click", function () {
            popup.remove();
        });

        // Thêm nút đóng vào popup
        popup.appendChild(closeButton);
        popup.appendChild(document.createElement("br")); // Thêm dòng mới để tạo khoảng cách giữa nút đóng và nội dung

        // Thêm popup vào body
        document.body.appendChild(popup);

    });

    // Thêm thông báo vào block thông báo
    var notifyBlock = document.getElementById("notification-block");
    notifyBlock.appendChild(notificationLink);

    // Cập nhật thanh cuộn
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