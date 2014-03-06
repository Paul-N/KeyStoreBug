//
// Translated by CS2J (http://www.cs2j.com): 02/03/2014 00:38:03
//

package com.xamarin.auth.testutils;


import android.content.Context;
import com.xamarin.auth.Account;
import com.xamarin.auth.droid.AccountStore;
import com.xamarin.auth.interfaces.IAccountStore;

import java.util.ArrayList;
import java.util.List;

public class AccountStoreInstanceProvider
{
    private static List<IAccountStore> _instances = new ArrayList<IAccountStore>();
    private static String _serviceId = "kanjigrand";
    public static IAccountStore getNew(Context ctx) throws Exception {
        IAccountStore instance = new AccountStore(ctx/*Mvx.<Cirrious.CrossCore.Droid.IMvxAndroidGlobals>Resolve().ApplicationContext*/);
        _instances.add(instance);
        return instance;
    }

    public static void clearAllAccounts() /*throws Exception*/ {
        //_instances.ForEach(i => i.FindAccountsForService(_serviceId).ToList().ForEach(a => i.Delete(a, _serviceId)));
        for(IAccountStore store : _instances)
        {
            for (Account acc : store.FindAccountsForService(_serviceId))
            {
                store.Delete(acc, _serviceId);
            }
        }
    }

}


