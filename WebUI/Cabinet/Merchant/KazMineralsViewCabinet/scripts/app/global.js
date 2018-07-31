var globalSettings = {
    urlToApp: baseUrl,
    applicationPath: $("base").attr("href"),
    customerId: CUSTOMER_ID,
    $title: $("#title_page")
}

$(window).on('load', function () {
    $(document).foundation({
        Dropdown: {
            active_class: 'open'
        }
    });
    $("a#profile_logout").attr("href", baseUrl + "/api/profile/logout");
});