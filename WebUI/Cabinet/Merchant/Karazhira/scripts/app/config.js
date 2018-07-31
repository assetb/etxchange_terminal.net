require.config({
    //  псевдонимы и пути используемых библиотек и плагинов
    paths: {
        //foundation
        'foundation': '/scripts/foundation/foundation',
        'foundation-datepicker': '/scripts/foundation/foundation-datepicker',
        //angular
        'angular': '/scripts/angular',
        'lang': '/scripts/i18n/angular-locale_ru-kz',
        'angular-route': '/scripts/angular-route',
        'angular-resource': '/scripts/angular-resource',
        //services
        'alta-api': '/scripts/app/services/alta.api',
        //directives
        'datepicker' : '/scripts/app/directives/datepicker',
    },
    //  экспортируем перменные в глобальную область
    shim: {
        'foundation': {
            exports: 'fountation'
        },
        'foundation-datepicker': {
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
    "lang"
    ]
});