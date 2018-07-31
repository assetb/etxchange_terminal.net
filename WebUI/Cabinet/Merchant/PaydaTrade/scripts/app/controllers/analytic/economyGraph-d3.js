require([
], function (angular) {
    var app = angular.module("controllers.analytic.EconomyGraph-d3", []);
    app.controller("EconomyCrtl", ["$scope", function ($scope) {

        var phones = [{ name: 'iPhone 6', price: 64, company: 'Apple' },
        { name: 'Samsung Galaxy Tab 4', price: 28, company: 'Samsung' },
        { name: 'iPhone 5s', price: 49, company: 'Apple' },
        { name: 'Samsung Galaxy S5', price: 48, company: 'Samsung' },
        { name: 'iPad Air', price: 37, company: 'Apple' },
        { name: 'Samsung Galaxy Note', price: 36, company: 'Samsung' }];

        function showGraph(phones, company) {

            d3.select('div.diagram').selectAll('div.item').data(phones).enter().append('div')
                .attr('class', 'item').append('span');

            d3.select('div.diagram').selectAll('div.item').data(phones)
                .attr("class", "item").style('width', function (d) { return (d.price * 6) + 'px'; })
                .select('span').text(function (d) { return d.name; });

            d3.select('div.diagram').selectAll('div.item').data(phones).exit().remove();

            d3.select("div.diagram").selectAll("div.item")
                .filter(function (d, i) {
                    if (company && company !== 'All')
                        return !(d.company == company);
                    else
                        return false;
                })
                .classed("unselected", true);
        }

        function select() {
            var company = document.getElementById("select").value;
            showGraph(phones, company);
        }

        showGraph(phones);


    }]);

    angular.bootstrap($("#economyGraph2"), ["controllers.analytic.EconomyGraph-d3"]);
    });


