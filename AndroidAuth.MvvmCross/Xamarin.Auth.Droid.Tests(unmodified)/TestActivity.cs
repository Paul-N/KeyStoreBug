using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Android.NUnitLite;
using KeyStoreBug.DemoApp.Tests;
using System.Reflection;

namespace Xamarin.Auth.Droid.Tests
{
    [Activity(Label = "Xamarin.Auth.Tests (unmod)", MainLauncher = true, Icon = "@drawable/icon")]
    public class TestActivity : TestSuiteActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            AccountStoreInstanceProvider.Context = this.ApplicationContext;

            this.AddTest(Assembly.GetExecutingAssembly());
            base.OnCreate(bundle);
        }
    }
}

