using System;
using System.Linq;
using System.Windows;
using NUnit.Framework;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.CrossCore.Plugins;
using Xamarin.Auth.Interfaces;
using Cirrious.CrossCore.Platform;
using KeyStoreBug.DemoApp.Android;
using System.Collections.Generic;

namespace Xamarin.Auth.Common.Tests
{
    [TestFixture]
    public class CredentialsTests
    {
        private string _serviceId = "DemoApp";

        [Test, Ignore]
        public void CreateRetriveCredentials()
        {
            var testName = "CreateRetriveCredentials";
            
            //Arrange

            Trace(testName, TestStage.Arrange);

            var user = new Account
            (
                username: "Foo Name",
                properties: new System.Collections.Generic.Dictionary<string, string>{ { "Password", "BarPassword" }, {"AvatarUri", "http://DemoApp.jp/static/user.png"} }
            );

            var store = Mvx.Resolve<IAccountStore>();
            store.Save(user, _serviceId);

            //Act

            Trace(testName, TestStage.Act);

            var loadedUsers = store.FindAccountsForService(_serviceId);

            //Assert
            //Assert.AreEqual(loadedUser.UserName, user.UserName);
            //Assert.AreEqual(loadedUser.Password, user.Password);

            Trace(testName, TestStage.Assert);

            Assert.That(loadedUsers.Any(), Is.True);
            Assert.That(loadedUsers.Count(), Is.EqualTo(1));
            Assert.That(loadedUsers.First().Username, Is.EqualTo("Foo Name"));
            Assert.That(loadedUsers.First().Properties["Password"], Is.EqualTo("BarPassword"));
        }

        #region CheckCreateAccount

        [Test, Ignore]
        public void TestCheckCreateAccountMultipleTimes()
        {
            CheckCreateAccount("TestCheckCreateAccountMultipleTimes", 500);
        }
        
        [Test, Ignore]
        public void TestCheckCreateAccountOnce()
        {
            CheckCreateAccount("TestCheckCreateAccountOnce", 1);
        }
        
        public void CheckCreateAccount(string testName, int secondaryLaunches)
        {
            //Arrange

            Trace(testName, TestStage.Arrange);

            //Avoid Mvx.Resolve<IAccountStore>(); using .ctor to "emulate" sequence of two launches of the app
            var serv1 = Mvx.Resolve<IAccountStore>();//new Xamarin.Auth.Droid.AccountStore(Mvx.Resolve<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>().ApplicationContext);

            //Act

            Trace(testName, TestStage.Act);

            if (!serv1.FindAccountsForService(_serviceId).Any())
                serv1.Save(
                    new Account(
                        username: "User1",
                        properties: new Dictionary<string, string> { 
                            {"Birthday", (DateTime.UtcNow - TimeSpan.FromDays(365 * 20)).ToString()}, 
                            {"Token", "sdfert56rtylfkdpoti"} 
                        }),
                    _serviceId);
            else
                Assert.Fail("There's already one or more accounts in app");

            var serv2 = Mvx.Resolve<IAccountStore>();//new Xamarin.Auth.Droid.AccountStore(Mvx.Resolve<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>().ApplicationContext);

            //Assert
            Trace(testName, TestStage.Assert);

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

        #region DeleteAccountWhichWasNeverCreated

        [Test, Ignore]
        public void TestDeleteAccountWhichWasNeverCreated()
        {
            DeleteAccountWhichWasNeverCreated("TestDeleteAccountWhichWasNeverCreated");
        }

        [Test, Ignore]
        public void TestDeleteAccountWhichWasNeverCreatedMultiple()
        {
            for (int i = 0; i < 1500; i++)
            {
                DeleteAccountWhichWasNeverCreated("TestDeleteAccountWhichWasNeverCreatedMultiple");
            }
        }

        public void DeleteAccountWhichWasNeverCreated(string testName)
        {
            //Arrange

            Trace(testName, TestStage.Arrange);

            var store = Mvx.Resolve<IAccountStore>();
            var user = new Account
            (
                username: "Foo Name",
                properties: new System.Collections.Generic.Dictionary<string, string> { { "Password", "BarPassword" }, { "AvatarUri", "http://DemoApp.jp/static/user.png" } }
            );

            Trace(testName, TestStage.Act);
            //Act
            store.Delete(user, _serviceId);

            Trace(testName, TestStage.Assert);

            Assert.That(store.FindAccountsForService(_serviceId).Any(), Is.False);
        } 

        #endregion

        #region GetAccountThatNeverExists

        [Test, Ignore]
        public void TestGetAccountThatNeverExistsMultipleTimes()
        {
            for (int i = 0; i < 5000; i++)
            {
                GetAccountThatNeverExists("TestGetAccountThatNeverExistsMultipleTimes");
            }
        }

        [Test, Ignore]
        public void TestGetAccountThatNeverExists()
        {
            GetAccountThatNeverExists("TestGetAccountThatNeverExists");
        }

        public void GetAccountThatNeverExists(string testName)
        {
            //Arrange

            Trace(testName, TestStage.Arrange);

            var store = Mvx.Resolve<IAccountStore>();

            //Act

            Trace(testName, TestStage.Act);

            var accs = store.FindAccountsForService("never_exists");

            //Assert

            Trace(testName, TestStage.Assert);

            Assert.That(accs.Any(), Is.False);
        } 

        #endregion

        [Test, Ignore]
        public void TestCreateThenDelete()
        {
            //Arrange

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "TestCreateThenDelete() arrange");

            var user = new Account
            (
                username: "Foo Name",
                properties: new System.Collections.Generic.Dictionary<string, string> { { "Password", "BarPassword" }, { "AvatarUri", "http://DemoApp.jp/static/user.png" } }
            );

            var store = Mvx.Resolve<IAccountStore>();

            //Act

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "TestCreateThenDelete() act");

            store.Save(user, _serviceId);
            store.Delete(user, _serviceId);

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "TestCreateThenDelete() assert");
            
            //Assert
            //Assert.IsFalse(store.IsUserStored());
            Assert.That(store.FindAccountsForService(_serviceId).Any(), Is.False);
        }

        [Test, Ignore]
        public void TestIsUserStoreReturnsFalseIfNeverCreated()
        {
            //Arrange

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "TestIsUserStoreReturnsFalseIfNeverCreated() arrange");

            var store = Mvx.Resolve<IAccountStore>();

            //Act

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "TestIsUserStoreReturnsFalseIfNeverCreated() act (do nothing)");

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "TestIsUserStoreReturnsFalseIfNeverCreated() assert");
            
            //Assert
            //Assert.IsFalse(store.IsUserStored());
            Assert.That(store.FindAccountsForService(_serviceId).Any(), Is.False);
        }

        public static Action PostSetupAction { get; set; }
        private static bool _initialized = false;

        [SetUp]
        public void SetUp()
        {
            if (!_initialized) 
            { 
                Initialize(); 
                _initialized = true; 
            }
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    IAccountStore srv = null;

        //    if (Mvx.TryResolve<IAccountStore>(out srv))
        //    {
        //        srv.FindAccountsForService(_serviceId).ToList().ForEach(a => srv.Delete(a, _serviceId));
        //    }
        //}

        #region Helpers

        private void Initialize()
        {
            var iocProvider = MvxSimpleIoCContainer.Initialize();
            Mvx.RegisterSingleton<IMvxTrace>(new DebugTrace());
            MvxTrace.Initialize();
            Mvx.RegisterSingleton(iocProvider);
            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "CredentialsTests: SetUp() entered and IoC set up...");
            Mvx.RegisterSingleton<IMvxPluginManager>(new MvxFilePluginManager(Const.PluginPath));

            if (PostSetupAction != null) PostSetupAction.Invoke();

            var bstrapper = new XamarinAuthPluginBootstrap();
            bstrapper.Run();
            PluginLoader.Instance.EnsureLoaded();

            MvxTrace.Trace(MvxTraceLevel.Diagnostic, "CredentialsTests: SetUp() ...leaving");
        }

        public enum TestStage { Arrange, Act, Assert };

        public void Trace(string testName, TestStage stage)
        {
            MvxTrace.Trace(MvxTraceLevel.Diagnostic, string.Format("{0: 1}", testName, stage.ToString()));
        } 

        #endregion
    }
}
