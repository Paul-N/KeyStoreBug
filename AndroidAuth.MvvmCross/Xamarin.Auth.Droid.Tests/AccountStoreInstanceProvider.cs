using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Auth.Interfaces;
using Cirrious.CrossCore;
using Xamarin.Auth.Droid;

namespace KeyStoreBug.DemoApp.Tests
{
    public class AccountStoreInstanceProvider
    {
        private static List<IAccountStore> _instances = new List<IAccountStore>();

        private static string _serviceId = "DemoApp";
        
        public static IAccountStore GetNew()
        {
            var instance = new AccountStore(Mvx.Resolve<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>().ApplicationContext);
            _instances.Add(instance);

            return instance;
        }

        public static void ClearAllAccounts()
        {
            _instances.ForEach(i => i.FindAccountsForService(_serviceId).ToList().ForEach(a => i.Delete(a, _serviceId)));
        }
    }
}