using System;
using System.Linq;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Xamarin.Auth.Interfaces;
using Java.Security;
using Javax.Crypto;
using System.IO;

namespace Xamarin.Auth.Droid
{
    public class AccountStore : IAccountStore
    {
        Context context;
        KeyStore ks;
        KeyStore.PasswordProtection prot;

        static readonly object fileLock = new object();

        const string FileName = "Xamarin.Social.Accounts";
        static readonly char[] Password = "3295043EA18CA264B2C40E0B72051DEF2D07AD2B4593F43DDDE1515A7EC32617".ToCharArray();
        
        public IEnumerable<Account> FindAccountsForService(string serviceId)
        {
            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.FindAccountsForService() entering...");
            
            var r = new List<Account>();

            var postfix = "-" + serviceId;

            var aliases = ks.Aliases();
            while (aliases.HasMoreElements)
            {
                var alias = aliases.NextElement().ToString();
                if (alias.EndsWith(postfix))
                {
                    var e = ks.GetEntry(alias, prot) as KeyStore.SecretKeyEntry;
                    if (e != null)
                    {
                        var bytes = e.SecretKey.GetEncoded();
                        var serialized = System.Text.Encoding.UTF8.GetString(bytes);
                        var acct = Account.Deserialize(serialized);
                        r.Add(acct);
                    }
                }
            }

            r.Sort((a, b) => a.Username.CompareTo(b.Username));

            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.FindAccountsForService() leaving (return " + r.ToArray().ToString() + " ) ...");

            return r;
        }

        public void Save(Account account, string serviceId)
        {
            var alias = MakeAlias(account, serviceId);

            var secretKey = new SecretAccount(account);
            var entry = new KeyStore.SecretKeyEntry(secretKey);
            ks.SetEntry(alias, entry, prot);

            Save();
        }

        public void Delete(Account account, string serviceId)
        {
            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Delete() before var alias = MakeAlias(account, serviceId)");
            var alias = MakeAlias(account, serviceId);

            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Delete() before ks.DeleteEntry(alias)");
            ks.DeleteEntry(alias);
            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Delete() before Save()");
            Save();
        }

        public AccountStore(Context context)
        {
            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore..ctor() entering...");
            
            this.context = context;

            ks = KeyStore.GetInstance(KeyStore.DefaultType);

            prot = new KeyStore.PasswordProtection(Password);

            if (context.FileList().Any(fname => fname == FileName))
            {

                try
                {
                    lock (fileLock)
                    {
                        using (var s = context.OpenFileInput(FileName))
                        {
                            ks.Load(s, Password);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    ks.Load (null, Password);
                    //LoadEmptyKeyStore(Password);
                }
            }
            else
                ks.Load(null, Password);//LoadEmptyKeyStore(Password);

            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore..ctor() leaving...");
        }

        void Save()
        {
            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() enter (we're just befroe lock())...");

            lock (fileLock)
            {
                Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() locked");

                using (var s = context.OpenFileOutput(FileName, FileCreationMode.Private))
                {
                    Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() before ks.Store(s, Password)");

                    ks.Store(s, Password);

                    Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() after ks.Store(s, Password)");
                }

                Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() leaving lock...");
            }

            Cirrious.CrossCore.Platform.MvxTrace.Trace(Cirrious.CrossCore.Platform.MvxTraceLevel.Diagnostic, "AccountStore.Save() leaving...");
        }

        static string MakeAlias(Account account, string serviceId)
        {
            return account.Username + "-" + serviceId;
        }

        class SecretAccount : Java.Lang.Object, ISecretKey
        {
            byte[] bytes;
            public SecretAccount(Account account)
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(account.Serialize());
            }
            public byte[] GetEncoded()
            {
                return bytes;
            }
            public string Algorithm
            {
                get
                {
                    return "RAW";
                }
            }
            public string Format
            {
                get
                {
                    return "RAW";
                }
            }
        }

        //static IntPtr id_load_Ljava_io_InputStream_arrayC;

        /// <summary>
        /// Work around Bug https://bugzilla.xamarin.com/show_bug.cgi?id=6766
        /// </summary>
        //void LoadEmptyKeyStore(char[] password)
        //{
        //    if (id_load_Ljava_io_InputStream_arrayC == IntPtr.Zero)
        //    {
        //        id_load_Ljava_io_InputStream_arrayC = JNIEnv.GetMethodID(ks.Class.Handle, "load", "(Ljava/io/InputStream;[C)V");
        //    }
        //    IntPtr intPtr = IntPtr.Zero;
        //    IntPtr intPtr2 = JNIEnv.NewArray(password);
        //    JNIEnv.CallVoidMethod(ks.Handle, id_load_Ljava_io_InputStream_arrayC, new JValue[]
        //        {
        //            new JValue (intPtr),
        //            new JValue (intPtr2)
        //        });
        //    JNIEnv.DeleteLocalRef(intPtr);
        //    if (password != null)
        //    {
        //        JNIEnv.CopyArray(intPtr2, password);
        //        JNIEnv.DeleteLocalRef(intPtr2);
        //    }
        //}
    }
}