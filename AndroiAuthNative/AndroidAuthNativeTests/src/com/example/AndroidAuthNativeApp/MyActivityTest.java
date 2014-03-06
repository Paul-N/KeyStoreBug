package com.example.AndroidAuthNativeApp;

import android.content.Context;
import android.test.ActivityInstrumentationTestCase2;
import android.util.Log;
import com.xamarin.auth.Account;
import com.xamarin.auth.interfaces.IAccountStore;
import com.xamarin.auth.testutils.AccountStoreInstanceProvider;
import com.xamarin.auth.testutils.TestStage;
import com.xamarin.auth.testutils.TestTrace;
import junit.framework.Assert;

import java.lang.ref.SoftReference;
import java.util.*;

/**
 * This is a simple framework for a test of an Application.  See
 * {@link android.test.ApplicationTestCase ApplicationTestCase} for more information on
 * how to write and extend Application tests.
 * <p/>
 * To run this test, you can type:
 * adb shell am instrument -w \
 * -e class com.example.AndroidAuthNativeApp.MyActivityTest \
 * com.example.AndroidAuthNativeApp.tests/android.test.InstrumentationTestRunner
 */
public class MyActivityTest extends ActivityInstrumentationTestCase2<MyActivity> {

    public MyActivityTest() {
        super("com.example.AndroidAuthNativeApp", MyActivity.class);
    }

    private String _serviceId = "kanjigrand";

    public void testCheckCreateAccountMultipleTimes() throws Exception {
        checkCreateAccount("TestCheckCreateAccountMultipleTimes",500);
    }

    public void testCheckCreateAccountOnce() throws Exception {
        checkCreateAccount("TestCheckCreateAccountOnce",1);
    }

    private void checkCreateAccount(String testName, int secondaryLaunches) throws Exception {
        //Arrange
        TestTrace.trace(testName, TestStage.Arrange);
        Context ctx = getActivity().getApplicationContext();
        //Avoid Mvx.Resolve<IAccountStore>(); using .ctor to "emulate" sequence of two launches of the app
        /* [UNSUPPORTED] 'var' as type is unsupported "var" */IAccountStore serv1 = AccountStoreInstanceProvider.getNew(ctx);
        //new Xamarin.Auth.Droid.AccountStore(Mvx.Resolve<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>().ApplicationContext);
        //Act
        TestTrace.trace(testName,TestStage.Act);

        List<Account> accsEverCreated = serv1.FindAccountsForService(_serviceId);

        Log.d("MyActivityTest: ", "accsEverCreated == null: " + Boolean.toString(accsEverCreated == null));
        Log.d("MyActivityTest: ", "accsEverCreated.size()" + Integer.toString(accsEverCreated.size()));

        String userName = "User1";

        if (accsEverCreated == null || accsEverCreated.size() == 0)
        {
            HashMap<String, String> accProps = new HashMap<String, String>();

            accProps.put("Birthday", Long.toString(Calendar.getInstance(TimeZone.getDefault()).getTimeInMillis() - (20 * 365 * 60 * 60 * 60 * 1000)));

            Account accToSave = new Account(userName, accProps);

            serv1.Save(accToSave, _serviceId);
        }
        else
            fail("There's already one or more accounts in app");
        serv1 = null;
        System.gc();//GC.Collect();
        /* [UNSUPPORTED] 'var' as type is unsupported "var" */IAccountStore serv2 = AccountStoreInstanceProvider.getNew(ctx);
        //new Xamarin.Auth.Droid.AccountStore(Mvx.Resolve<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>().ApplicationContext);
        //TODO: Create IAccountStore AccountStoreProvider.GetNewInstance(), Fix TearDown method.
        //Assert
        TestTrace.trace(testName,TestStage.Assert);
        List<Account> accs = serv2.FindAccountsForService(_serviceId);
        if (accs != null && accs.size() > 0)
        {
            for (int i = 0; i < secondaryLaunches; i++)
            {
                /* [UNSUPPORTED] 'var' as type is unsupported "var" */
                Account acc = serv2.FindAccountsForService(_serviceId).get(0);// First();
                assertNotNull(acc);
                assertEquals(acc.getUsername().compareTo("User1"), 0);
//                Assert.That(acc, Is.Not.Null);
//                Assert.That(acc.Username, Is.EqualTo("User1"));
            }
        }
        else
            fail("One account expected but 0 was found");
    }

//    private static Action __PostSetupAction = new Action();
//    public static Action getPostSetupAction() {
//        return __PostSetupAction;
//    }

//    public static void setPostSetupAction(Action value) {
//        __PostSetupAction = value;
//    }

    private static boolean _initialized = false;
    public void setUp() throws Exception {

        super.setUp();

//        if (!_initialized)
//        {
//            initialize();
//            _initialized = true;
//        }

    }

    @Override
    public void tearDown() throws Exception {

        //IAccountStore srv = null;
        //if (Mvx.TryResolve<IAccountStore>(out srv))
        //{
        //    srv.FindAccountsForService(_serviceId).ToList().ForEach(a => srv.Delete(a, _serviceId));
        //}
        AccountStoreInstanceProvider.clearAllAccounts();
        super.tearDown();
    }

//    private void initialize() throws Exception {
//        /* [UNSUPPORTED] 'var' as type is unsupported "var" */ iocProvider = MvxSimpleIoCContainer.Initialize();
//        Mvx.<IMvxTrace>RegisterSingleton(new DebugTrace());
//        MvxTrace.Initialize();
//        Mvx.RegisterSingleton(iocProvider);
//        MvxTrace.Trace(MvxTraceLevel.Diagnostic, "CredentialsTests: SetUp() entered and IoC set up...");
//        Mvx.<IMvxPluginManager>RegisterSingleton(new MvxFilePluginManager(Const.PluginPath));
//        if (getPostSetupAction() != null)
//            getPostSetupAction().Invoke();
//
//        MvxTrace.Trace(MvxTraceLevel.Diagnostic, "CredentialsTests: SetUp() ...leaving");
//    }

}
