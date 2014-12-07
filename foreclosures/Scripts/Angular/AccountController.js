myApp.controller('AccountController', ['$scope', '$http', '$location', 'UserService', function ($scope, $http, $location, UserService) {

    $scope.UserName = '';
    $scope.password = '';
    $scope.formSubmitted = false;
    $scope.confirmPassword = '';



    $scope.Login = function () {

        $scope.formSubmitted = true;


        $http.post('/Account/Login', { UserName: $scope.UserName, Password: $scope.password }).success(function (data, status, headers, config) {

            UserService.SetUser(data.User);
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


    $scope.Register = function () {

        $http.post('/Account/Register', { UserName: $scope.UserName, Password: $scope.password, ConfirmPassword: $scope.confirmPassword }).success(function (data, status, headers, config) {

       
            $scope.formSubmitted = false;
          

        }).
error(function (data, status, headers, config) {

    console.log(status);
});
    }


}]);






