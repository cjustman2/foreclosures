myApp.controller('Dashboard', ['$scope', '$http',  function ($scope, $http) {
 
    $scope.counties = [];
    $scope.countyCities = [];
    $scope.attributes = [];
    $scope.city = '';
    $scope.county = '';
    $scope.attribute = '';
  

    $scope.GetCounties = function () {

        $http.post('/Dashboard/GetCounties', {}).
        success(function (data, status, headers, config) {

            $scope.county = 1;
            $scope.counties = data.Counties;

            $scope.GetCities($scope.county);
            GetAddressesByCounty($scope.county);
            $scope.GetAttributes($scope.county);
        }).
        error(function (data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            console.log(status);
        });
    }



    $scope.GetCities = function (countyId) {

        $http.post('/Dashboard/GetCities', { countyId: countyId }).
        success(function (data, status, headers, config) {

            $scope.city = data.Cities.length > 0 ? data.Cities[0].cityId : '';
            $scope.countyCities = data.Cities;

        }).
        error(function (data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            console.log(status);
        });
    }




    $scope.GetAttributes = function (id) {

        $http.post('/Dashboard/GetAttributes', { ID: id }).
        success(function (data, status, headers, config) {

            $scope.attribute = data.Attributes.length == 1 ? data.Attributes[0].AttributeID : '';
            $scope.attributes = data.Attributes;


        }).
        error(function (data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            console.log(status);
        });

    }




    $scope.GetAddressesByCounty = function (id) {
        GetAddressesByCounty(id);
    }

    $scope.GetAddressesByCity = function (id) {
        GetAddressesByCity(id);
    }
    $scope.GetAddressesByAttribute = function (id) {
        GetAddressesByAttribute(id);
    }
}]);