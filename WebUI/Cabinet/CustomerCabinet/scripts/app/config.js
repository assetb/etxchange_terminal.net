require.config({
    //  псевдонимы и пути используемых библиотек и плагинов
    baseUrl: window.globalSettings.applicationPath,
    paths: {
        //foundation
        'foundation': 'scripts/lib/foundation/js/foundation.min',
        'foundation-datepicker': 'scripts/lib/foundation-datepicker/js/foundation-datepicker.min',
        'foundation-dropdown': 'scripts/lib/foundation/js/foundation/foundation.dropdown',
        //angular
        'angular': 'scripts/lib/angular/angular.min',
        'lang': 'scripts/lib/angular-i18n/angular-locale_ru-kz',
        'angular-route': 'scripts/lib/angular-route/angular-route.min',
        'angular-resource': 'scripts/lib/angular-resource/angular-resource.min',
        "angular-filter": "scripts/lib/angular-filter/dist/angular-filter.min",
        //services
        'alta-api': 'scripts/app/services/alta.api',
        'DocumentFormationApi': 'scripts/app/services/document-formation-api',
        "altatender-api": "scripts/app/services/altatender.api",
        "app.services.CustomerApi": "scripts/app/services/customer_api",
        "app.services.Archive": "scripts/app/services/archive",
        "search": "scripts/app/services/search",
        //directives
        "pagination": "scripts/app/directives/pagination",
        'datepicker': 'scripts/app/directives/datepicker',
    },
    //  экспортируем перменные в глобальную область
    shim: {
        'foundation': {
            exports: 'fountation'
        },
        'foundation-datepicker': {
            deps: ['foundation']
        },
        "foundation-dropdown": {
            deps: ['foundation']
        },
        'angular': {
            exports: 'angular'
        },
        'lang': {
            deps: ['angular']
        },
        'angular-route': {
            deps: ['angular']
        },
        'angular-resource': {
            deps: ['angular']
        },
        "angular-filter": {
            deps: ["angular"]
        },
        'alta-api': {
            deps: ['angular', 'angular-resource']
        },
        'datepicker': {
            deps: ['angular', 'foundation-datepicker']
        }
    },
    //  указываем зависимотси по умолчанию
    deps: [
    "angular",
    "lang",
    "foundation-dropdown"
    ]
});