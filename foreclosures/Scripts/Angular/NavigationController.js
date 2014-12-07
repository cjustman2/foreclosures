myApp.controller('Navigation', ['$scope', '$http', '$rootScope','UserService', 'PageScrapingService', function ($scope, $http,$rootScope, UserService, PageScrapingService) {

    $scope.User = {};
    $scope.PageScraper = PageScrapingService.GetPageScrapingService();
    $scope.isShown = false;


    $scope.ShowPageScrapingDiv = function () {
        $scope.isShown = $scope.isShown == true ? false : true;
    }

    $scope.$watch(UserService.GetUser, function () {
        $scope.User = UserService.GetUser();
       
    });

    $rootScope.$on("PageScrapingPages", function () {
        console.log($scope.PageScraper);
        $scope.PageScraper = PageScrapingService.GetPageScrapingService();
        
    });

}]);