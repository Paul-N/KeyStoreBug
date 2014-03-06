using System;
using System.Linq;
using System.Windows;
using NUnit.Framework;
#if !UNMOD
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.CrossCore.Plugins;
using Xamarin.Auth.Interfaces;
using Cirrious.CrossCore.Platform;
using KeyStoreBug.DemoApp.Android; 
#endif
using System.Collections.Generic;
using KeyStoreBug.DemoApp.Tests;

namespace Xamarin.Auth.Common.Tests
{
    /// <summary>
    /// Tests with "emulation" of sequence of relaunches of application.
    /// </summary>
    [TestFixture]
    public class CredentialsTestsAppRestart
    {
        private string _serviceId = "DemoApp";

        #region CheckCreateAccount

        [Test]
        public void TestCheckCreateAccountMultipleTimes()
        {
            CheckCreateAccount("TestCheckCreateAccountMultipleTimes", 500);
        }
        
        [Test]
        public void TestCheckCreateAccountOnce()
        {
            CheckCreateAccount("TestCheckCreateAccountOnce", 1);
        }
        
        public void CheckCreateAccount(string testName, int secondaryLaunches)
        {
            //Arrange

            TestTrace.Trace(testName, TestStage.Arrange);

            //Avoid Mvx.Resolve<IAccountStore>(); using .ctor to "emulate" sequence of two launches of the app
            var serv1 = AccountStoreInstanceProvider.GetNew();//new Xamarin.Auth.Droid.AccountStore(Mvx.Resolve<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>().ApplicationContext);

            //Act

            TestTrace.Trace(testName, TestStage.Act);

            if (!serv1.FindAccountsForService(_serviceId).Any())
                serv1.Save(
                    new Account(
                        /*username:*/ "User1",
                        /*properties:*/ new Dictionary<string, string> { 
                            {"Birthday", (DateTime.UtcNow - TimeSpan.FromDays(365 * 20)).ToString()}, 
                            {"Token", "sdfert56rtylfkdpoti"} 
                        }),
                    _serviceId);
            else
                Assert.Fail("There's already one or more accounts in app");

            serv1 = null;

            GC.Collect();

            var serv2 = AccountStoreInstanceProvider.GetNew();//new Xamarin.Auth.Droid.AccountStore(Mvx.Resolve<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>().ApplicationContext);

            //TODO: Create IAccountStore AccountStoreProvider.GetNewInstance(), Fix TearDown method.

            //Assert
            TestTrace.Trace(testName, TestStage.Assert);

            if (serv2.FindAccountsForService(_serviceId).Any())
            {
                for (int i = 0; i < secondaryLaunches; i++)
                {
                    var acc = serv2.FindAccountsForService(_serviceId).First();
                    Assert.That(acc, Is.Not.Null);
                    Assert.That(acc.Username, Is.EqualTo("User1"));
                }
            }
            else
                Assert.Fail("One account expected but 0 was found");
        }

        #endregion

        public static Action PostSetupAction { get; set; }
        private static bool _initialized = false;

        [SetUp]
        public void SetUp()
        {
#if !UNMOD            
        if (!_initialized) 
            {
                Initialize();
                _initialized = true;

            }
#else
            if (PostSetupAction != null) PostSetupAction.Invoke();
#endif 
        }

        [TearDown]
        public void TearDown()
        {
            AccountStoreInstanceProvider.ClearAllAccounts();
        }

        #region Helpers

#if !UNMOD
        private void Initialize()
        {
            var iocProvider = MvxSimpleIoCContainer.Initialize();
            Mvx.RegisterSingleton<IMvxTrace>(new DebugTrace());
            MvxTrace.Initialize();
            Mvx.RegisterSingleton(iocProvider);
            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "CredentialsTests: SetUp() entered and IoC set up...");
            Mvx.RegisterSingleton<IMvxPluginManager>(new MvxFilePluginManager(Const.PluginPath));

            if (PostSetupAction != null) PostSetupAction.Invoke();

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "CredentialsTests: SetUp() ...leaving");
        }  
#endif

        #endregion
    }
}
