(function () {
    'use strict';

    angular
        .module('app')
        .controller('auction_details_controller', auction_details_controller);

    auction_details_controller.$inject = ['$scope', '$routeParams', 'excel_factory', 'http_factory', '$window'];

    function auction_details_controller($scope, $routeParams, excel_factory, http, $window) {
        $scope.$$back = $routeParams.referer;
        $scope.auctionId = $routeParams.auctionId;
        $scope.currentDate = $window.currentDate;
        $scope.URL_ARCHIVE = baseUrl + '/api/archive/file';
        $scope.excel = excel_factory;
        $scope.applicant = {
            search: {
                $selected: null,
                text: null,
                items: null,
            }
        };
        $scope.model = {};

        $scope.files = null;

        $scope.orders = null;
        $scope.auction = null;
        $scope.applicants = null;

        $scope.GetAuction = function () {
            http.get(baseUrl + "/api/auction/" + $scope.auctionId, null,
                function (data, status, header, config) {
                    $scope.auction = data;
                    $scope.GetReportFile($scope.auction['<FilesListId>k__BackingField']);
                    $scope.GetApplicantsFile($scope.auction['<FilesListId>k__BackingField']);
                    $scope.GetReports();
                },
                function (data, status, headers, config) {
                    console.error("GetAuction. Error responce:", status, data);
                    $scope.auction = null;
                });
        };

        $scope.GetOrder = function (id) {
            http.get(baseUrl + "/api/auction/" + $scope.auctionId + "/customer-order", null,
                function (data, status, header, config) {
                    $scope.orders = data;
                },
                function (data, status, headers, config) {
                    console.error("GetOrder. Error responce:", status, data);
                    $scope.order = null;
                });
        }

        $scope.GetApplicants = function () {
            http.get(baseUrl + "/api/auction/" + $scope.auctionId + "/orders_supplier", null,
            function (data, status, headers, config) {
                $scope.applicants = data;
            },
            function (data, status, headers, config) {
                $scope.applicants = null;
            });
        };

        $scope.GetApplicantsFile = function (fileListId) {
            $scope.model.reports = null;
            http.get(baseUrl + "/api/archive/list/" + fileListId, { params: { types: [44] } },
                function (data) {
                    $scope.model.applicants = data.length > 0 ? data[0] : null;
                },
                function (data, status, headers, config) {
                    console.error("GetReportFile. Error responce:", status, data);
                    $scope.model.applicants = null;
                });
        };

        $scope.GetReportFile = function (fileListId) {
            $scope.model.reports = null;
            http.get(baseUrl + "/api/archive/list/" + fileListId, { params: { types: [16] } },
                function (data) {
                    $scope.model.reports = data.length > 0 ? data : null;
                },
                function (data, status, headers, config) {
                    console.error("GetReportFile. Error responce:", status, data);
                    $scope.model.reports = null;
                });
        };

        $scope.GetReports = function () {
            $scope.reports = null;
            http.get(baseUrl + "/api/customer/auction/" + $scope.auctionId + "/reports", null,
                function (reports) {
                    $scope.reports = reports;
                });
        };

        $scope.GetFiles = function (fileListId, types) {
            $scope.files = null;
            http.get(baseUrl + "/api/archive/list/" + fileListId, null,
                function (data) {
                    $scope.files = data;
                    $('#filesModal').foundation('reveal', 'open');
                },
                function (data, status, headers, config) {
                    console.error("GetFiles. Error responce:", status, data);
                    $scope.files = null;
                });
        };

        /// Список участников
        $scope.rejectApplicant = function (applicant) {
            console.log("reject applicant:", applicant);
            http.post(baseUrl + "/api/auction/reject_supplier",
                {
                    auctionId: $scope.auction.id,
                    supplierId: applicant.id,
                }, function (data, status, headers, config) {
                    $scope.applicant.search.text = null;
                    $scope.applicant.search.$selected = null;
                    $scope.SearchSuppliers();
                });
        };

        $scope.selectApplicant = function (applicant) {
            if (applicant) {
                $scope.applicant.search.$selected = applicant;
                $scope.applicant.search.text = applicant.Name;
                $scope.SearchSuppliers();
            }
        }

        $scope.addApplicant = function (applicant) {
            console.log("add applicant:", applicant);
            http.post(baseUrl + "/api/auction/add_supplier",
                {
                    auctionId: $scope.auction.id,
                    supplierId: applicant.id,
                }, function (data, status, headers, config) {
                    $scope.applicant.search.text = null;
                    $scope.applicant.search.$selected = null;
                    $scope.SearchSuppliers();
                });
        };

        $scope.SearchSuppliers = function () {
            $scope.applicant.search.items = [];
            if ($scope.applicant.search.text != null && $scope.applicant.search.text.length > 2) {
                http.Get(
                    baseUrl + "/api/supplier",
                    { params: { page: 1, countItems: 5, searchsupplier: $scope.applicant.search.text } },
                    function (data, status, headers, config) {
                        $scope.applicant.search.items = data.rows;
                    });
            }
        };

        // Общий фукнционал
        $scope.Init = function () {
            $scope.GetAuction();
            $scope.GetOrder();
            $scope.GetApplicants();
        };
    };
})();
