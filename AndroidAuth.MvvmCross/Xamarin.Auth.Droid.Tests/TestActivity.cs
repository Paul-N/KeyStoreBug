using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
//using NUnitLite.MonoDroid;
using System.Reflection;
using System.Collections.Generic;
using Cirrious.CrossCore.Droid;
using Cirrious.CrossCore;
using Xamarin.Auth.Common.Tests;
using Xamarin.Android.NUnitLite;
using Xamarin.Auth.Interfaces;

namespace Xamarin.Auth.Droid.Tests
{
    [Activity(Label = "Xamarin.Auth.Droid.Tests", MainLauncher = true, Icon = "@drawable/icon")]
    public class TestActivity : TestSuiteActivity//TestRunnerActivity
    {
        //protected override IEnumerable<Assembly> GetAssembliesForTest()
        //{
        //    yield return GetType().Assembly;
        //}

        //protected override Type GetDetailsActivityType
        //{
        //    get { return typeof(TestActivity); }
        //}

        protected override void OnCreate(Bundle savedInstanceState)
        {
#if !FROMJAR
            CredentialsTests.PostSetupAction = SetGlobals;
#endif
            CredentialsTestsAppRestart.PostSetupAction = SetGlobals;

            this.AddTest(Assembly.GetExecutingAssembly());
            base.OnCreate(savedInstanceState);
            
        }

        private void SetGlobals()
        {
            if (!Mvx.CanResolve<IMvxAndroidGlobals>())
            {
                Mvx.RegisterSingleton<IMvxAndroidGlobals>(new Globals(this.ApplicationContext));
            }
        }
    }

    public class Globals : IMvxAndroidGlobals
    {
        private Context _ctx;

        public Context ApplicationContext
        {
            get { return _ctx; }
        }

        public Assembly ExecutableAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public string ExecutableNamespace
        {
            get { return "KeyStoreBug.MvvmCross.Plugins.StoredCredentials.Droid.Test"; }
        }

        public Globals(Context ctx)
        {
            _ctx = ctx;
        }
    }
}

