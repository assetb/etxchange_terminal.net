require(["angular", "alta-api"], function (ng) {
    var app = ng.module("alta.online.main", ["alta.service.api"]);

    app.controller("OnlineMainCrtl", ["$location", "$window", "$scope", "$interval", "altaApi", function ($location, $window, $scope, $interval, altaApi) {
        $scope.auctions = null;
        $scope.selectedLots = [];
        $scope.timer = null;
        $scope.timeout = 1;

        function generateColor() {
            var rgb = [];

            for (var i = 0; i < 3; i++)
                rgb.push(Math.floor(Math.random() * 255));
            return rgb
        }

        $scope.updatePriceOffers = function () {
            console.log($scope.selectedLots);

            var selectedLotsNumbers = [];

            ng.forEach($scope.selectedLots, function (lot) {
                selectedLotsNumbers.push(lot.Number);
            });

            altaApi.online.ets({ lots: selectedLotsNumbers },
                    function (responce) {
                        if (responce.code == 200) {
                            ng.forEach($scope.selectedLots, function (lot) {
                                lot.$priceOffers = responce.data.filter(function (priceOffer) {
                                    return priceOffer.lotCode == lot.Number;
                                });
                            });
                        } else {
                            console.error("Error: code", responce.code, "description", responce.description);
                        }
                    });
        }

        $scope.selectLot = function (lotId) {
            $scope.selectedLots = [];

            ng.forEach($scope.auctions, function (auction) {
                ng.forEach(auction.lots, function (lot) {
                    if (lotId && lotId == lot.Id) {
                        lot.$selected = true;
                    }

                    if (lot.$selected) {
                        lot.$color = generateColor();
                        $scope.selectedLots.push(lot);
                    } else {
                        lot.$color = null;
                    }
                });
            });
            if ($scope.selectedLots.length) {
                $scope.startTimer();
            } else {
                $scope.stopTimer();
            }
        };

        $scope.getAuctions = function () {
            var currentDate = new Date();
            currentDate.setHours(0, 0, 0, 0);

            var tsCurrentDay = Math.round(currentDate.getTime() / 1000);
            var tsTomorrowDay = tsCurrentDay + (24 * 3600 - 1);

            altaApi.auction.get({
                customerId: $window.globalSettings.custumerId,
                site: 4,
                page: 1,
                countItems: 100,
                fromDate: new Date(tsCurrentDay * 1000).toISOString(),
                toDate: new Date(tsTomorrowDay * 1000).toISOString()
            }, function (responce) {
                $scope.auctions = responce.rows;

                var params = $location.search();

                if (params.lotId) {
                    $scope.selectLot(params.lotId);
                }
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
            $scope.getAuctions();
        }
    }]);


    ng.bootstrap($("#application_online"), ['alta.online.main']);
});