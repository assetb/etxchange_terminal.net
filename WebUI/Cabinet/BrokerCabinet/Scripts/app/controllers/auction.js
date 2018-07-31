require([
    "angular",
    "pagination",
    "datepicker",
    "alta-search",
    "alta-enum",
], function (angular) {
    var app = angular.module("app.controllers.Auction", ["app.directives.Pagination", "app.directives.Datepicker", "app.services.Search","app.utils.AltaEnum"]);
    
    app.filter('cutName', function () {
        return function(input) {
          if(input.includes("Акционерное общество")){
        return input.replace(/Акционерное общество/g, 'АО');
       }else if(input.includes("Товарищество с ограниченной ответственностью")){
       return input.replace(/Товарищество с ограниченной ответственностью/g, 'ТОО');
     }
   }
    });
    app.controller("auctionCtrl", ["$scope", "TableSearch", "ListSearch", "enumFactory", function ($scope, TableSearch, ListSearch, enumFactory) {
          

     $scope.form = TableSearch;
     $scope.enum = enumFactory;
        $scope.Listform = ListSearch;  
        $scope.form.$url = baseUrl + "/api/auction";

        $("#content").attr("style", "visibility:visible");
        $("#orderRow").hide();

        $scope.Init = function () {
            $scope.form.$isLoading = true;
            $scope.form.params.countItems = 10;
            $scope.form.params.page = 1;
            $scope.form.params.isdesc = true;
            $scope.form.params.orderby = "date";            
            $scope.form.Search();
        };

        $scope.search = function (page) {  
            if (page) {
               $scope.form.params.page = page;
            } else {
              $scope.form.params.page = 1;
            }      
            $scope.form.params.isdesc = true;
            $scope.form.params.orderby = "date";
            $scope.form.$isLoading = true;        
            $scope.form.Search();     
        };

        $scope.update = function (page, items) {
            $scope.form.params.countItems = items;
            $scope.form.params.page = page;            
            $scope.search(page);    
        };

        $scope.checkOrder = function(){
            $scope.Listform.$isLoading = true;
            $scope.Listform.$url = baseUrl + "/api/order";
            $scope.Listform.params.statusId = 1;
            $scope.Listform.Search();         
                         
        }

        $scope.toggle_order = function (element) {
            $(element).slideToggle(function () {
                $('#submit>label').text(
                    $(this).is(':visible') ? "Скрыть" : "Новые заявки"
                );
            });
        };

    }]);
    angular.bootstrap($("#auctionApp"), ["app.controllers.Auction"]);
        });