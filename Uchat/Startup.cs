using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Uchat.Startup))]
namespace Uchat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
