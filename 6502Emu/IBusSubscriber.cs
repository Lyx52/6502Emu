namespace Emu;

public interface IBusSubscriber
{
    public ushort StartAddress { get; set; }
    public ushort EndAddress { get; set; }
    public void Write(ushort address, byte data);
    public byte Read(ushort address);
}