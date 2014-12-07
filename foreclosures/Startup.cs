using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(foreclosures.Startup))]
namespace foreclosures
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
           ConfigureAuth(app);

        
        }
    }


}

