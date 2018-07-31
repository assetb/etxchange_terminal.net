require([
    "angular",
    "alta-search",
    "datepicker",
    "alta-enum",
], function (angular) {
    var app = angular.module("app.controllers.Order", ["app.services.Search", "app.directives.Datepicker", "app.utils.AltaEnum"]);
    app.controller("orderCtrl", ["$scope", "ListSearch", "enumFactory", function ($scope, ListSearch, enumFactory) {

        $scope.Listform = ListSearch;
        $scope.enum = enumFactory;

        $scope.init = function (orderID) {
            $scope.Listform.$isLoading = true;
            $scope.Listform.$url = baseUrl + "/api/order/?id=" + orderID;     
            $scope.Listform.Search();
        }

        $scope.data = {
            "param1": "param1",
            "param2": "2"
        }


        $scope.createAuction = function () {
            $.ajax({
                url: baseUrl + "/api/auction/createAuction",
                type: "POST",
                data: $scope.data,
                success: function () {
                    alert(data)
                }
                //dataType: dataType
            });
            alert("Все стало хорошо!")
        }




        $scope.handleFileSelect = function (evt) {
            $scope.files = evt.target.files; // FileList object

            // files is a FileList of File objects. List some properties.
            var output = [];
            for (var i = 0, f; f = $scope.files[i]; i++) {
                output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
                    f.size, ' bytes, last modified: ',
                    f.lastModifiedDate.toLocaleDateString(), '</li>');
            }
            document.getElementById('list').innerHTML = '<ul>' + output.join('') + '</ul>';
        }

        document.getElementById('files').addEventListener('change', $scope.handleFileSelect, false);

    }]);
    angular.bootstrap($("#auctionApp"), ["app.controllers.Order"]);
});