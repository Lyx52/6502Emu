namespace Emu;

public class Bus
{
    private List<IBusSubscriber> Subscribers { get; set; } = new List<IBusSubscriber>();


    public void AddSubscriber(IBusSubscriber subscriber)
    {
        // TODO: Add range check
        
        Subscribers.Add(subscriber);
    }
    
    public void Write8(ushort address, byte data)
    {
        var subscriber = GetSubscriber(address);
        
        subscriber?.Write(address, data);
    }
    
    public byte Read8(ushort address)
    {
        var subscriber = GetSubscriber(address);
        
        return subscriber?.Read(address) ?? 0x00;
    }
    
    public void Write16(ushort address, ushort data)
    {
        var subscriber = GetSubscriber(address);
        if (subscriber is null) return;
        subscriber.Write(address, (byte)(data & 0xFF));
        subscriber.Write(++address, (byte)(data >> 8));
    }

    public ushort Read16(ushort address)
    {
        var subscriber = GetSubscriber(address);
        if (subscriber is null) return 0x0000;
        return (ushort)(subscriber.Read(address) | subscriber.Read(++address) << 8);
    }
    
    private IBusSubscriber? GetSubscriber(ushort address)
    {
        return Subscribers.FirstOrDefault(s => s.StartAddress <= address && s.EndAddress >= address);
    }
}