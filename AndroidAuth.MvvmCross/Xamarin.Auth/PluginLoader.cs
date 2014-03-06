using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin.Auth
{
    public class PluginLoader : IMvxPluginLoader
    {
        public static readonly PluginLoader Instance = new PluginLoader();
        public void EnsureLoaded()
        {
            var manager = Mvx.Resolve<IMvxPluginManager>();
            manager.EnsurePlatformAdaptionLoaded<PluginLoader>();
        }
    }
}
