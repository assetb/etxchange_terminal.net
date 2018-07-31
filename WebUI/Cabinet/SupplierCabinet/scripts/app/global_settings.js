var currentDate = new Date();

var globalSettings = {
    application: $("base").attr("href"),
    urlToApp: baseUrl,
    urlToArhive: baseUrl + '/api/archive',
};


require.config({
    //  псевдонимы и пути используемых библиотек и плагинов
    baseUrl: window.globalSettings.application,
    paths: {
        //libs
        "jquery": "scripts/lib/jquery/dist/jquery.min",
        "foundation": "scripts/lib/foundation/js/foundation.min",
        "foundation-datepicker": "scripts/lib/foundation-datepicker/js/foundation-datepicker.min",
        "angular": "scripts/lib/angular/angular.min",
        "lang": "scripts/lib/angular-i18n/angular-locale_ru-kz",
        "angular-route": "scripts/lib/angular-route/angular-route.min",
        "angular-messages": "scripts/lib/angular-messages/angular-messages.min",
        "angular-resource": "scripts/lib/angular-resource/angular-resource.min",
        //alta libs

        //services
        "alta-http": "scripts/app/services/http",
        "alta-search": "scripts/app/services/search_factory",
        "notification": "scripts/app/services/notification",

        "app.services.DocumentFormation": "scripts/app/services/document-formation",
        "app.services.Lot": "scripts/app/services/lot",
        "app.services.Company": "scripts/app/services/company",
        "app.services.Archive": "scripts/app/services/archive",
        //api
        "alta-api": "scripts/app/api/alta-api",
        "altatender-api": "scripts/app/api/altatender-api",
        "auction-api": "scripts/app/api/auction",
        "supplier-api": "scripts/app/api/supplier-api",
        "company-api": "scripts/app/api/company",

        //directives
        "pagination": "scripts/app/directives/pagination_directive",
        "datepicker": "scripts/app/directives/datepiker_directive",
        "file-input": "scripts/app/directives/file_input_directive",
        "multi-select": "scripts/app/directives/alta-multi-select",
        "app.directives.Table": "scripts/app/directives/table_directive",
    },
    shim: {
        "jquery": {
            exports: "$"
        },
        "foundation": {
            deps: ["jquery"],
            exports: "fountation"
        },
        "foundation-datepicker": {
            deps: ["foundation"]
        },
        "foundation-select": {
            deps: ["foundation"]
        },
        "angular": {
            deps: ["jquery"],
            exports: "angular"
        },
        "lang": {
            deps: ["angular"],
            exports: "lang"
        },
        "angular-route": {
            deps: ["angular"]
        },
        "angular-resource": {
            deps: ["angular"]
        },
        "angular-messages": {
            deps: ["angular"]
        },
        "notification": {
            exports: "notification"
        }
    },

    //  указываем зависимотси по умолчанию
    deps: [
    "angular",
    "lang",
    "foundation",
    "notification"
    ]
});