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
using Xamarin.Auth;

namespace KeyStoreBug.DemoApp.Tests
{
    public class AccountStoreInstanceProvider
    {
        private static List<AccountStore> _instances = new List<AccountStore>();

        private static string _serviceId = "DemoApp";

        public static Context Context { get; set; }

        public static AccountStore GetNew()
        {
            var instance = AccountStore.Create(Context);
            _instances.Add(instance);

            return instance;
        }

        public static void ClearAllAccounts()
        {
            _instances.ForEach(i => i.FindAccountsForService(_serviceId).ToList().ForEach(a => i.Delete(a, _serviceId)));
        }
    }
}