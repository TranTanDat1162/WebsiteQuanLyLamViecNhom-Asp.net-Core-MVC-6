//This file is for creating a custom javascript for a really specific usecase

//Read the name and you can get the idea
//By Minh Trièu
//(function generateBreadcrumbs(breadcrumbItems) {
//    // Get the current page URL
//    //var currentPageUrl = window.location.href;

//    //// Split the URL by '/' to extract the necessary information
//    //var urlParts = currentPageUrl.split('/');

//    //// Remove the first 3 empty items from the array
//    //urlParts.shift();
//    //urlParts.shift();
//    //urlParts.shift();

//    // Initialize an empty array to store the breadcrumb items
//    //var breadcrumbItems = [];

//    // Add "Home" as the first item
//    breadcrumbItems.push("Home");

//    // Iterate through the URL parts to generate the breadcrumb items
//    //for (var i = 0; i < urlParts.length; i++) {
//    //    var breadcrumbItem = urlParts[i].replace(/-/g, ' '); // Replace '-' with space
//    //    breadcrumbItems.push(breadcrumbItem);
//    //}

//    var breadcrumbContainer = document.createElement("nav");
//    breadcrumbContainer.setAttribute("aria-label", "breadcrumb");

//    var breadcrumbList = document.createElement("ol");
//    breadcrumbList.classList.add("breadcrumb");

//    for (var i = 0; i < breadcrumbItems.length; i++) {
//        var breadcrumbItem = document.createElement("li");
//        breadcrumbItem.classList.add("breadcrumb-item");

//        if (i === breadcrumbItems.length - 1) {
//            breadcrumbItem.classList.add("active");
//            breadcrumbItem.setAttribute("aria-current", "page");
//            breadcrumbItem.textContent = breadcrumbItems[i][1];
//            breadcrumbList.appendChild(breadcrumbItem);
//        }
//        else {
//            var breadcrumbLink = document.createElement("a");
//            breadcrumbLink.setAttribute("href", "/" + breadcrumbItems[i][0]);
//            breadcrumbLink.textContent = breadcrumbItems[i][1];

//            breadcrumbItem.appendChild(breadcrumbLink);
//            breadcrumbList.appendChild(breadcrumbItem);
//        }
//    }

//    breadcrumbContainer.appendChild(breadcrumbList);
//    document.getElementById("breadcrumbs").appendChild(breadcrumbContainer);
//})();
function generateBreadcrumbs(breadcrumbData) {

    const breadcrumbItems = JSON.parse(breadcrumbData);
    // Ensure breadcrumbItems is a valid 2D array
    if (!Array.isArray(breadcrumbItems) || breadcrumbItems.some(item => !Array.isArray(item) || item.length !== 2)) {
        throw new Error("Invalid breadcrumb data provided.");  // Handle invalid input gracefully
    }

    // Create the breadcrumb container and list elements
    const breadcrumbContainer = document.createElement("nav");
    breadcrumbContainer.setAttribute("aria-label", "breadcrumb");

    const breadcrumbList = document.createElement("ol");
    breadcrumbList.classList.add("breadcrumb");

    // Generate breadcrumb items from the provided array
    breadcrumbItems.forEach((item, index) => {
        const breadcrumbItem = document.createElement("li");
        breadcrumbItem.classList.add("breadcrumb-item");

        if (index === breadcrumbItems.length - 1) {
            // Last item is the current page
            breadcrumbItem.classList.add("active");
            breadcrumbItem.setAttribute("aria-current", "page");
            breadcrumbItem.textContent = item[1];
        } else {
            // Create a link for non-active items
            const breadcrumbLink = document.createElement("a");
            breadcrumbLink.setAttribute("href", item[0]); // Use the provided link directly
            breadcrumbLink.textContent = item[1];
            breadcrumbItem.appendChild(breadcrumbLink);
        }

        breadcrumbList.appendChild(breadcrumbItem);
    });

    // Append the breadcrumb list to the container and container to the page
    breadcrumbContainer.appendChild(breadcrumbList);
    document.getElementById("breadcrumbs").appendChild(breadcrumbContainer);
};
