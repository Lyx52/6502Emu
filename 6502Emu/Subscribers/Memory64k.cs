namespace Emu.Subscribers;

public class Memory64k : IBusSubscriber
{
    public byte[] Memory = new byte[0xFFFF + 1];

    public ushort StartAddress { get; set; } = 0x0000;
    public ushort EndAddress { get; set; } = 0xFFFF;
    public void Write(ushort address, byte data)
    {
        Memory[address] = data;
    }

    public byte Read(ushort address)
    {
        return Memory[address];
    }
}