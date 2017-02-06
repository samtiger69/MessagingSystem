using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MessagingSystem.Startup))]
namespace MessagingSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
