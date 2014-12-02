myApp.controller('AccountController', ['$scope', '$http', '$location', 'UserService', function ($scope, $http, $location, UserService) {

    $scope.email = '';
    $scope.password = '';
    $scope.formSubmitted = false;




    $scope.Submit = function () {

        $scope.formSubmitted = true;


        $http.post('/Account/Login', { Email: $scope.email, Password: $scope.password }).success(function (data, status, headers, config) {

            UserService.SetUser(data);
            $scope.formSubmitted = false;
            $location.path('/admin');

        }).
     error(function (data, status, headers, config) {

         console.log(status);
     });

    }




    $scope.LogOut = function () {
        UserService.SetUser({ Role: '', LoggedIn: false });
        $location.path('/');
    }

}]);






myApp.controller('RegisterController', ['$scope', '$http', function ($scope, $http) {


}]);