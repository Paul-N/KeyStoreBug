package com.xamarin.auth.droid;

import com.xamarin.auth.Account;
import com.xamarin.auth.interfaces.IAccountStore;
import com.xamarin.auth.utils.Uri;

import android.content.Context;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.security.KeyStore;
import java.security.KeyStoreException;
import java.security.NoSuchAlgorithmException;
import java.security.UnrecoverableEntryException;
import java.security.cert.CertificateException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.Enumeration;
import java.util.List;

import javax.crypto.SecretKey;

/**
 * Created by HomeUser on 25.02.14.
 */
public class AccountStore implements IAccountStore {

    Context context;
    KeyStore ks;
    KeyStore.PasswordProtection prot;

    private static final Object fileLock = new Object();

    private static final String FileName = "Xamarin.Social.Accounts";
    private static final char[] Password = "3295043EA18CA264B2C40E0B72051DEF2D07AD2B4593F43DDDE1515A7EC32617".toCharArray();

    public AccountStore(Context context)
    {
        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore..ctor() entering...");

        this.context = context;

        try {
            ks = KeyStore.getInstance(KeyStore.getDefaultType());
        } catch (KeyStoreException e) {
            e.printStackTrace();
        }

        prot = new KeyStore.PasswordProtection(Password);

        boolean isAlreadyExists = false;
        for(String curfileName : context.fileList())
        {
            if(curfileName.compareTo(FileName) == 0)
            {
                isAlreadyExists = true;
                break;
            }
        }

        if (isAlreadyExists/*context.fileList().Any(fname => fname == FileName)*/)
        {

            try
            {
                /*lock*/synchronized(fileLock)
                {
                    FileInputStream s = context.openFileInput(FileName);
                    //using (var s = context.OpenFileInput(FileName))
                    //{
                        ks.load(s, Password);
                    //}
                    s.close();
                }
            }
            catch (FileNotFoundException fnfe)
            {
                try {
                    ks.load(null, Password);
                } catch (IOException e) {
                    e.printStackTrace();
                } catch (NoSuchAlgorithmException e) {
                    e.printStackTrace();
                } catch (CertificateException e) {
                    e.printStackTrace();
                }
                //LoadEmptyKeyStore(Password);
            } catch (CertificateException e) {
                e.printStackTrace();
            } catch (NoSuchAlgorithmException e) {
                e.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        else
            try {
                ks.load(null, Password);//LoadEmptyKeyStore(Password);
            } catch (IOException e) {
                e.printStackTrace();
            } catch (NoSuchAlgorithmException e) {
                e.printStackTrace();
            } catch (CertificateException e) {
                e.printStackTrace();
            }

        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore..ctor() leaving...");
    }

    @Override
    public List<Account> FindAccountsForService(String serviceId) {

        List<Account> r = new ArrayList<Account>();

        String postfix = "-" + serviceId;

        try {

            Enumeration<String> aliases = ks.aliases();
            while (aliases.hasMoreElements()) {
                String alias = aliases.nextElement();
                if (alias.endsWith(postfix)) {
                    KeyStore.SecretKeyEntry e = (KeyStore.SecretKeyEntry) ks.getEntry(alias, prot);
                    if (e != null) {
                        byte[] bytes = e.getSecretKey().getEncoded();
                        String serialized = new String(bytes, Uri.Utf8Enc);//System.Text.Encoding.UTF8.GetString(bytes);
                        Account acct = Account.Deserialize(serialized);
                        r.add(acct);
                    }
                }
            }
        } catch (KeyStoreException kse) {
            kse.printStackTrace();
        } catch (NoSuchAlgorithmException nsae) {
            nsae.printStackTrace();
        } catch (UnrecoverableEntryException uee) {
            uee.printStackTrace();
        } catch (UnsupportedEncodingException uence) {
            uence.printStackTrace();
        }

        //r.Sort((a, b) => a.Username.CompareTo(b.Username));
        Collections.sort(r, new Comparator<Account>() {
            @Override
            public int compare(Account account, Account account2) {
                return account.getUsername().compareTo(account2.getUsername());
            }
        });

        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.FindAccountsForService() leaving (return " + r.ToArray().ToString() + " ) ...");

        return r;

    }

    @Override
    public void Save(Account account, String serviceId) {

        String alias = MakeAlias(account, serviceId);

        SecretAccount secretKey = new SecretAccount(account);
        KeyStore.SecretKeyEntry entry = new KeyStore.SecretKeyEntry(secretKey);
        try {
            ks.setEntry(alias, entry, prot);
        } catch (KeyStoreException e) {
            e.printStackTrace();
        }

        Save();
    }

    @Override
    public void Delete(Account account, String serviceId) {
        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Delete() before var alias = MakeAlias(account, serviceId)");
        String alias = MakeAlias(account, serviceId);

        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Delete() before ks.DeleteEntry(alias)");
        try {
            ks.deleteEntry(alias);
        } catch (KeyStoreException e) {
            e.printStackTrace();
        }
        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Delete() before Save()");
        Save();
    }

    private void Save() {
        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() enter (we're just befroe lock())...");

        //synchronized (fileLock){}

        synchronized/*lock*/ (fileLock) {
            //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() locked");

            FileOutputStream s = null;
            try {
                s = context.openFileOutput(FileName, Context.MODE_PRIVATE);

//            using (var s = context.OpenFileOutput(FileName, FileCreationMode.Private))
//            {
                //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() before ks.Store(s, Password)");

                ks.store(s, Password);
                s.close();
                //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() after ks.Store(s, Password)");
                //}
            } catch (FileNotFoundException e) {
                e.printStackTrace();
            } catch (CertificateException e) {
                e.printStackTrace();
            } catch (NoSuchAlgorithmException e) {
                e.printStackTrace();
            } catch (KeyStoreException e) {
                e.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            }
            //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() leaving lock...");
        }

        //Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() leaving...");
    }

    static String MakeAlias(Account account, String serviceId) {
        return account.getUsername() + "-" + serviceId;
    }

    private class SecretAccount implements SecretKey {
        byte[] bytes;

        public SecretAccount(Account account) {
            try {
                bytes = account.Serialize().getBytes(Uri.Utf8Enc);
            } catch (UnsupportedEncodingException e) {
                e.printStackTrace();
            }
        }

        public byte[] getEncoded() {
            return bytes;
        }

        public String getAlgorithm() {
            return "RAW";
        }

        public String getFormat() {
            return "RAW";
        }
    }
}
