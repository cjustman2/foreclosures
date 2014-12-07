var myApp = angular.module('myApp', ['ngRoute']);



myApp.config(['$routeProvider',function ($routeProvider) {
      $routeProvider.
        when('/', {
            templateUrl: 'Pages/GoogleMap.html',
            controller: 'GoogleMapController'
        }).
        when('/login', {
            templateUrl: 'Pages/login.html',
            controller: 'AccountController'
        }).
    when('/register', {
        templateUrl: 'Pages/register.html',
        controller: 'AccountController'
    }).
    when('/admin', {
        templateUrl: 'Pages/admin.html',
        controller: 'AdminController'
    }).
      otherwise({
          redirectTo: '/'
      });
  }]);





























