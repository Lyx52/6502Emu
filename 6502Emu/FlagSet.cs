namespace Emu;

public class FlagSet
{
    public byte Flags { get; private set; } = 0x1 << 5;
    
    public FlagSet SetFlag(Flag flag, bool state)
    {
        Flags = (byte)(state ? Flags | (byte)flag : Flags & ~(byte)flag);
        return this;
    }

    public bool IsSet(Flag flag)
    {
        return (Flags & (byte)flag) == (byte)flag;
    }
    
    public static implicit operator byte(FlagSet set) => set.Flags;
}