
myApp.controller('AdminController', ['$scope', '$http','$rootScope','PageScrapingService', function ($scope, $http,$rootScope,PageScrapingService) {
    $scope.locationHierarchy = [];
    $scope.PageScraperErrors = PageScrapingService.GetPageScrapingErrors();


    $rootScope.$on("PageScrapingErrors", function () {
        console.log($scope.PageScraperErrors);
        $scope.PageScraperErrors = PageScrapingService.GetPageScrapingErrors();

    });

    $http.get('/Admin/LocationHierarchy').
success(function (data, status, headers, config) {

    $scope.locationHierarchy = data.Data;

}).
error(function (data, status, headers, config) {

    console.log(status, data);
});





    $scope.Scrape = function (key,value) {
   
        PageScrapingService.AddPageScrapingService({ ID: key });
      
        PageScrape(key,value);
      
    }


    var meter = '<div id="" class="meterHolder"><h4></h4><div class="meter nostripes"><span class="orange" style=""></span></div></div>';
    function PageScrape(id,name) {
        // var target = document.getElementById('inDIV');
        //var spinner = new Spinner().spin(target);

        // var id = $('#Counties').val();
        var county = $('#Counties :selected').text();
        $.ajax({
            url: '/Home/PageScrape',
            type: 'Post',
            data: {
                ID: id
            },
            success: function (result) {

                if (result.IsStarted) {
                    $(meter).attr('id', id).appendTo('#meterHolder');
                    $('#' + id + ' h4').html(name);

                    GetProgress(id);
                }


            },
            complete: function () {

            },
            beforeSend: function () {
                $('#errors').empty();
            },
            error: function () {

                alert('Error!');
            }


        });
    }



    function GetProgress(id) {

        $.ajax({
            url: '/Home/Progress',
            data: { countyId: id },
            type: 'Post',
            success: function (result) {
                $('#' + id + ' span').css('width', result.Complete + '%');
                if (result.Complete < 100) {

                    setTimeout(function () {
                        GetProgress(id);
                    }, 500);

                } else {
                    setTimeout(function () {
                       
                        $('#' + id).remove();
                        PageScrapingService.RemovePageScrapingService(id);
                        $scope.$apply();
                    }, 1000);
                }

                if (result.Errors.length > 0) {
                
                   PageScrapingService.AddPageScrapingErrors(result.Errors);
                    $scope.$apply();
                }
               

               // console.log($scope.PageScraperErrors);
            }
        });
    }

}]);

