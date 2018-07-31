require([
    "angular",
    "alta-search",
    "datepicker",
    "alta-enum",
    //"scroll",
], function (angular) {
    var app = angular.module("app.controllers.NewAuction", ["app.services.Search", "app.directives.Datepicker", "app.utils.AltaEnum"]);
    app.controller("newAucCtrl", ["$scope", "$filter", "ListSearch", "enumFactory", function ($scope, $filter, ListSearch, enumFactory) {

        $scope.Listform = ListSearch;
        $scope.enum = enumFactory;
        $scope.date =new Date();

        $scope.init = function () {
            $scope.Listform.$isLoading = true;
            $scope.Listform.params = [];
            $scope.Listform.$url = baseUrl + "/api/order/?id=" + 22;
            $scope.Listform.Search();
        }

        $scope.getDateReglament = function (date) {           
            $scope.Listform.$isLoading = true;
            $scope.Listform.params = {orderDate: date};
            $scope.Listform.$url = baseUrl + "/api/broker/dateRglament";
            $scope.Listform.Search();
        }
        
    

        $scope.createAuction = function () {
            $.ajax({
                url: baseUrl + "/api/auction/createAuction",
                type: "POST",
                data: $scope.data,
                //success: function () {
                //    alert(data)
                //}
                ////dataType: dataType
            });
            alert("Все стало хорошо!")
        }


        $scope.toggle_dom = function (element,button) {
            $(element).slideToggle(function () {
                $(button).text(
                    $(this).is(':visible') ? "Принять" : "Показать"
                );
            });
        };

        $scope.postParam = {}




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
    angular.bootstrap($("#auctionApp"), ["app.controllers.NewAuction"]);
});