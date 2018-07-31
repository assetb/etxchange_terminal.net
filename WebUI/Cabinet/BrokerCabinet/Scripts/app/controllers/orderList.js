require([
    "angular",
    "alta-search",
], function (angular) {
    var app = angular.module("app.controllers.OrderList", ["app.services.Search"]);
    app.controller("orderListCtrl", ["$scope", "ListSearch", "$http", function ($scope, ListSearch, $http) {

        $scope.Listform = ListSearch;
        //$scope.enum = enumFactory;

        $scope.init = function () {
            $scope.Listform.$isLoading = true;
            $scope.Listform.$url = baseUrl + "/api/broker/orderList";
            $scope.Listform.Search();
        }

        $scope.orderDetails = function (id) {            
            return baseUrl.replace("ServerApp", "BrokerCabinet") + "/Order/OrderInfo/" +id;
        }

        var data = new FormData();
        data.append("info", "23605"           
        );       

        $scope.createOrders = function () {
            $http.post(baseUrl + "/api/broker/confirmOrder", data, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            }).
                then(function success(response) {
                    $scope.question = response.data.question;
                }, function error(response) {
                    console.log("Возникла ошибка");
                }
                );
        }

     

        //$scope.data = {
        //    "param1": "param1",
        //    "param2": "2"
        //}


        //$scope.createAuction = function () {
        //    $.ajax({
        //        url: baseUrl + "/api/auction/createAuction",
        //        type: "POST",
        //        data: $scope.data,
        //        success: function () {
        //            alert(data)
        //        }
        //        //dataType: dataType
        //    });
        //    alert("Все стало хорошо!")
        //}




        //$scope.handleFileSelect = function (evt) {
        //    $scope.files = evt.target.files; // FileList object

        //    // files is a FileList of File objects. List some properties.
        //    var output = [];
        //    for (var i = 0, f; f = $scope.files[i]; i++) {
        //        output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
        //            f.size, ' bytes, last modified: ',
        //            f.lastModifiedDate.toLocaleDateString(), '</li>');
        //    }
        //    document.getElementById('list').innerHTML = '<ul>' + output.join('') + '</ul>';
        //}

        //document.getElementById('files').addEventListener('change', $scope.handleFileSelect, false);

    }]);
    angular.bootstrap($("#orderApp"), ["app.controllers.OrderList"]);
});