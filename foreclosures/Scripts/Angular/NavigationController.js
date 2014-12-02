myApp.controller('Navigation', ['$scope', '$http', '$rootScope','UserService', 'WebService', function ($scope, $http,$rootScope, UserService, WebService) {

    $scope.User = {};
    $scope.WebService = WebService.GetWebService();
    $scope.isShown = false;


    $scope.ShowWebServiceDiv = function () {
        $scope.isShown = $scope.isShown == true ? false : true;
    }

    $scope.$watch(UserService.GetUser, function () {
        $scope.User = UserService.GetUser();
       
    });

    $rootScope.$on("webservice", function () {
        console.log($scope.WebService);
        $scope.WebService = WebService.GetWebService();
        
    });

}]);