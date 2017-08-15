using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OKB.Startup))]
namespace OKB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
