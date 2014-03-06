KeyStoreBug
===========

!! Fastest way to check the bug is to run AndroidAuth.MvvmCross/Xamarin.Auth.Droid.Tests(unmodified) application

Demo repo for Xamarin bug

AndroiAuthNative - java implementation of Xamarin.Auth
-/AndroidAuthNative - Xamarin.Auth java lib
-/AndroidAuthNativeApp - empty app. (IDEA not allows us to create test application of library, so we need this app)
-/AndroidAuthNativeTests - JUnit app with tests

AndroidAuth.MvvmCross - .net Xamarin app
-/KeyStoreBug.DemoApp.Android - helper files for linking
-/packages-local - local library of https://github.com/xamarin/Xamarin.Auth for Android
-/Xamarin.Auth - extracted interfaces of Xamarin.Auth for using it as Mvvmcross plugin
-/Xamarin.Auth.Droid - extracted implementation of Xamarin.Auth for using it as Mvvmcross plugin
-/Xamarin.Auth.Droid(Jar exported) - wrapper of java implementation Xamarin.Auth (of AndroiAuthNative/AndroidAuthNative project)
-/Xamarin.Auth.Droid.Tests - NUnit tests for AndroidAuth.MvvmCross/Xamarin.Auth.Droid
-/Xamarin.Auth.Droid.Tests(jar tests) - NUnit tests for AndroidAuth.MvvmCross/Xamarin.Auth.Droid(Jar exported)
-/Xamarin.Auth.Droid.Tests(unmodified) - NUnit tests for original Xamarin.Auth libs (AndroidAuth.MvvmCross/packages-local)
-/Xamarin.Auth.WindowsPhone.Tests - helper files for linking