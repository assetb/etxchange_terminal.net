require.config({
    //  псевдонимы и пути используемых библиотек и плагинов
    baseUrl: window.globalSettings.applicationPath,
    paths: {
        //foundation
        'foundation': 'scripts/foundation/foundation',
        'foundation-datepicker': 'scripts/foundation/foundation-datepicker',
        'foundation-dropdown': 'scripts/foundation/foundation.dropdown',
        //angular
        'angular': 'scripts/angular',
        'lang': 'scripts/i18n/angular-locale_ru-kz',
        'angular-route': 'scripts/angular-route',
        'angular-resource': 'scripts/angular-resource',
        "angular-filter": "scripts/lib/angular-filter/dist/angular-filter",
        //services
        'alta-api': 'scripts/app/services/alta.api',
        "altatender-api": "scripts/app/services/altatender.api",
        //directives
        'datepicker': 'scripts/app/directives/datepicker',
        "pagination": 'scripts/app/directives/pagination',
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