﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Rituals.Startup))]

namespace Rituals
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
