namespace Emu;

public enum Flag : byte
{
    Carry = 0x1,
    Zero = 0x1 << 1,
    InterruptDisable = 0x1 << 2,
    Decimal = 0x1 << 3,
    BreakCommand = 0x1 << 4,
    Overflow = 0x1 << 6,
    Negative = 0x1 << 7
}