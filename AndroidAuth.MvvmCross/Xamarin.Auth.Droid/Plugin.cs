using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid;
using Cirrious.CrossCore.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Auth.Interfaces;

namespace Xamarin.Auth.Droid
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterSingleton<IAccountStore>(new AccountStore(Mvx.Resolve<IMvxAndroidGlobals>().ApplicationContext));
        }
    }
}