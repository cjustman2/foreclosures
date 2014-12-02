myApp.service('UserService', function () {
    var user = {
        Role: '',
        LoggedIn: false
    };


    return {

        IsLoggedIn:function(){
            return user.LoggedIn;
        },
        GetUser: function () {
            return user;
        },
        SetUser: function (value) {
           user = value;
        }
    }
    
});


myApp.service('WebService', ['$rootScope',function ($rootScope) {

    var WebServiceObserver = {PagesBeingScraped: []};


    return {
        
        AddWebService: function (value) {
            WebServiceObserver.PagesBeingScraped.push(value);
            $rootScope.$broadcast("webservice");
        },
        RemoveWebService: function (value) {

            for (var i = 0; i < WebServiceObserver.PagesBeingScraped.length; i++) {
                if (WebServiceObserver.PagesBeingScraped[i].ID === value) {
                    WebServiceObserver.PagesBeingScraped.splice(i, 1);
                    break;
                }
            }

            $rootScope.$broadcast("webservice")
          
        },
        GetWebService: function () {
            return WebServiceObserver;
        }
    }
}]);