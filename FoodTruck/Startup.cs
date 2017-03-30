using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FoodTruck.Startup))]
namespace FoodTruck
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
