namespace AnjLab.FX.Tools.Testing
{
    public interface ITestCleaner
    {
        void OnSetup();
        void ClearCache();
        void OnTearDown();
    }
}
