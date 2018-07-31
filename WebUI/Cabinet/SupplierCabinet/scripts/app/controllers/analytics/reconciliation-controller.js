require([
    'angular',
    'alta-api',
    'datepicker'
], function (angular) {
    var app = angular.module("app.profile.Reconciliation", ['alta.service.api', 'alta.directive.datepicker']);
    app.filter('split', function () {
        return function (input, splitChar, splitIndex) {
            // do some bounds checking here to ensure it has that index
            return input.split(splitChar)[splitIndex];
        }
    });

    var ReconciliationСtrl = function ($window, $scope, altaApi) {
        $scope.form = {
            brokerId: null,
            startDate: null,
            toDate: null,
        },
            $scope.model = {
                brokers: null,
                reports: null
            };

        $scope.checkCredit = function () {
            return $scope.getSum("credit") - $scope.getSum("debit");
        };

        $scope.getSum = function (columnName) {
            var sum = 0;
            angular.forEach($scope.model.reports, r => sum += r[columnName]);

            return sum;
        };     
        $scope.dateFilter = function () {
            var arr = [];
            angular.forEach($scope.reports, function (value, key) {
                var parts = value.docDate.split('.');
                var d = new Date(parts[2].split(' ')[0],parts[1]-1, parts[0], );
                if (d >= $scope.form.startDate && d <= $scope.form.toDate) {
                    arr.push(value);                  
                }
                $scope.model.reports = arr;
            });
        }

        $scope.getBrokers = function () {
            altaApi.company.getBrokers(function (brokers) {
                $scope.model.brokers = brokers;
            }, function (responce) {
                notification.showErrApplication(responce.status);
            });
        };

        $scope.getReconciliation = function ($event) {
            var button = null;

            if ($event != null && $event.target != null) {
                button = $("button[type=\"submit\"]", $event.target).prop("disabled", true);
            }

            $scope.reports = null;
            altaApi.company.getReconciliation($scope.form,
                function (reports) {
                    $scope.reports = reports;
                    $scope.dateFilter();
                    if (button)
                        button.prop("disabled", false);
                },
                function (responce) {
                    switch (responce.status) {
                        case (404): {
                            notification.showWarn("Не удалось найти поставщика или брокера.");
                        } break;
                        case (501): {
                            notification.showWarn("Взаимодействие с брокером не реализовано.");
                        } break;
                        default: {
                            notification.showErrApplication();
                        }
                    }
                    if (button)
                        button.prop("disabled", false);
                }
           );
        };

        $scope.init = function () {
            $scope.getBrokers();
        };

        $scope.init();
    };

    ReconciliationСtrl.$inject = ['$window', '$scope', 'altaApi'];

    app.controller("ReconciliationСtrl", ReconciliationСtrl);

    angular.bootstrap($('#controller_container'), ['app.profile.Reconciliation']);
});