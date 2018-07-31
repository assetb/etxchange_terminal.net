require(["angular", "alta-api"], function (ng) {
    var app = ng.module("alta.online.main", ["alta.service.api"]);

    app.controller("OnlineMainCrtl", ["$location", "$window", "$scope", "$interval", "$filter", "altaApi", function ($location, $window, $scope, $interval, $filter, altaApi) {
        $scope.auctions = null;
        $scope.lots = [];
        $scope.selectedLots = [];
        $scope.timer = null;
        $scope.timeout = 1;
        $scope.online = false;
        $scope.switchSelectAll = false;
        $scope.isLoading = false;

        function generateColor() {
            var rgb = [];

            for (var i = 0; i < 3; i++)
                rgb.push(Math.floor(Math.random() * 255));
            return rgb
        }

        $scope.updatePriceOffers = function () {
            var selectedLotsNumbers = [];

            ng.forEach($scope.lots, function (lot) {
                if (lot.$selected == true) {
                    selectedLotsNumbers.push(lot.Number);
                }
            });

            if (selectedLotsNumbers.length > 0) {
                altaApi.online.ets({ lots: selectedLotsNumbers },
                        function (responce) {
                            if (responce.code == 200) {
                                ng.forEach($scope.lots, function (lot) {
                                    lot.$priceOffers = responce.data.filter(function (priceOffer) {
                                        return priceOffer.lotCode == lot.Number;
                                    });
                                });
                            } else {
                                console.error("Error: code", responce.code, "description", responce.description);
                            }
                        });
            }
        }

        $scope.onSwitchSelectAll = function () {
            ng.forEach($scope.lots, function (lot) {
                lot.$selected = $scope.switchSelectAll;
            });
        }

        $scope.getAuctions = function () {
            $scope.lots = [];
            $scope.isLoading = true;

            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);
            var tsCurrentDay = Math.round(currentDate.getTime() / 1000);
            var tsTomorrowDay = tsCurrentDay + (24 * 3600 - 1);
            altaApi.auction.get({
                customerId: $window.globalSettings.customerId,
                site: 4,
                page: 1,
                countItems: 100,
                fromDate: new Date(tsCurrentDay * 1000).toISOString(),
                toDate: new Date(tsTomorrowDay * 1000).toISOString()
            }, function (responce) {
                var auctions = $filter('orderBy')(responce.rows, "number");
                $scope.isLoading = false;

                ng.forEach(auctions, function (auction) {
                    ng.forEach(auction.lots, function (lot) {
                        lot.$auctionNumber = auction.number;
                    });
                    $scope.lots = [].concat.apply($scope.lots, auction.lots);
                });
            });
        };

        $scope.stopTimer = function () {
            $scope.timer && $interval.cancel($scope.timer);
            $scope.timer = null;
        };

        $scope.startTimer = function () {
            if (!$scope.timer) {
                $scope.timer = $interval($scope.updatePriceOffers, ($scope.timeout * 1000));
                $scope.$on('$destroy', function () {
                    $scope.stopTimer();
                });
            }
        };

        $scope.init = function () {
            $("#tableAuctionOnline").css("display", "");
            $scope.getAuctions();
            $scope.startTimer();
        }
    }]);


    ng.bootstrap($("#application_online"), ['alta.online.main']);
});