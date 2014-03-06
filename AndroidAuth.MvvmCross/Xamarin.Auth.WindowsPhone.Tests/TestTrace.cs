#if !UNMOD
using Cirrious.CrossCore.Platform;
#endif


namespace KeyStoreBug.DemoApp.Tests
{
    public enum TestStage { Arrange, Act, Assert }
    
    public static class TestTrace
    {
        public static void Trace(string testName, TestStage stage)
        {
#if UNMOD
            Android.Util.Log.Debug("MvxTraceLevel.Diagnostic", string.Format("{0}: {1}", testName, stage.ToString()));
#else
            MvxTrace.Trace(MvxTraceLevel.Diagnostic, string.Format("{0}: {1}", testName, stage.ToString()));
#endif
        } 
    }
}
