//
// Translated by CS2J (http://www.cs2j.com): 02/03/2014 00:30:30
//

package com.xamarin.auth.testutils;


import android.util.Log;

public class TestTrace
{
    public static void trace(String testName, TestStage stage)  {
        Log.e("MvxTraceLevel.Diagnostic", testName.concat(": ").concat(stage.toString()));
        //MvxTrace.Trace(MvxTraceLevel.Diagnostic, String.Format("{0}: {1}", testName, stage.ToString()));
    }

}


