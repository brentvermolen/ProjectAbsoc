using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Absoc.Startup))]
namespace Absoc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
