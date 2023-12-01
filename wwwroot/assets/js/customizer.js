//This file is for creating a custom javascript for a really specific usecase

//Read the name and you can get the idea
//By Minh Trièu
(function generateBreadcrumbs() {
    // Get the current page URL
    var currentPageUrl = window.location.href;

    // Split the URL by '/' to extract the necessary information
    var urlParts = currentPageUrl.split('/');

    // Remove the first 3 empty items from the array
    urlParts.shift();
    urlParts.shift();
    urlParts.shift();

    // Initialize an empty array to store the breadcrumb items
    var breadcrumbItems = [];

    // Add "Home" as the first item
    breadcrumbItems.push("Home");

    // Iterate through the URL parts to generate the breadcrumb items
    for (var i = 0; i < urlParts.length; i++) {
        var breadcrumbItem = urlParts[i].replace(/-/g, ' '); // Replace '-' with space
        breadcrumbItems.push(breadcrumbItem);
    }

    var breadcrumbContainer = document.createElement("nav");
    breadcrumbContainer.setAttribute("aria-label", "breadcrumb");

    var breadcrumbList = document.createElement("ol");
    breadcrumbList.classList.add("breadcrumb");

    for (var i = 0; i < breadcrumbItems.length; i++) {
        var breadcrumbItem = document.createElement("li");
        breadcrumbItem.classList.add("breadcrumb-item");

        if (i === breadcrumbItems.length - 1) {
            breadcrumbItem.classList.add("active");
            breadcrumbItem.setAttribute("aria-current", "page");
            breadcrumbItem.textContent = breadcrumbItems[i];
            breadcrumbList.appendChild(breadcrumbItem);
        }
        else {
            var breadcrumbLink = document.createElement("a");
            breadcrumbLink.setAttribute("href", "#");
            breadcrumbLink.textContent = breadcrumbItems[i];

            breadcrumbItem.appendChild(breadcrumbLink);
            breadcrumbList.appendChild(breadcrumbItem);
        }
    }

    breadcrumbContainer.appendChild(breadcrumbList);
    document.getElementById("breadcrumbs").appendChild(breadcrumbContainer);
})();

