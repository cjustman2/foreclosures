myApp.service('UserService', function () {
    var user = {
        Role: '',
        LoggedIn: false,
        UserName: ''
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


myApp.service('PageScrapingService', ['$rootScope',function ($rootScope) {

    var PageScraperPages = {PagesBeingScraped: []};

    var PageScraperErrors = {PageErrors:[]};

    return {
        
        AddPageScrapingService: function (value) {
            PageScraperPages.PagesBeingScraped.push(value);
            $rootScope.$broadcast("PageScrapingPages");
        },
        RemovePageScrapingService: function (value) {

            for (var i = 0; i < PageScraperPages.PagesBeingScraped.length; i++) {
                if (PageScraperPages.PagesBeingScraped[i].ID === value) {
                    PageScraperPages.PagesBeingScraped.splice(i, 1);
                    break;
                }
            }

            $rootScope.$broadcast("PageScrapingPages")
          
        },
        GetPageScrapingService: function () {
            return PageScraperPages;
        },
        AddPageScrapingErrors: function (value) {
            PageScraperErrors.PageErrors.push(value);
            $rootScope.$broadcast("PageScrapingErrors");
        },
        GetPageScrapingErrors: function () {
            return PageScraperErrors;
        }
    }
}]);