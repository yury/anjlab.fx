namespace AnjLab.FX.Devices
{
    public interface IBytesFilter
    {
        byte[] Buffer { get; }
        void Clear();
        byte[][] Proccess(byte[] bytes);
    }
}
