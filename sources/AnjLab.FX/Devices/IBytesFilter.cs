namespace AnjLab.FX.Devices
{
    public interface IBytesFilter
    {
        byte[] Buffer { get; }
        byte[] PacketStart { get; set; }
        byte[] PacketEnd { get; set; }
        void Clear();
        byte[][] Proccess(byte[] bytes);
    }
}
