namespace Emu;

public static class AddressingModeSet
{

    public static ushort GetCurrentModeAddress(this Cpu cpu, AddressingMode mode)
    {
        ushort addr;
        switch (mode)
        {
            case AddressingMode.ZeroPage:
            {
                return cpu.Bus.Read8(cpu.ProgramCounter++);
            }
            case AddressingMode.Absolute:
            {
                addr = cpu.Bus.Read16(cpu.ProgramCounter);
                cpu.ProgramCounter += 2;
                return addr;
            }
            case AddressingMode.Relative:
            {
                var offset = cpu.Bus.Read8(cpu.ProgramCounter++);
                cpu.Status.SetFlag(Flag.Negative, (offset & 0x1 << 7) == 0x1 << 7);
                return (ushort)(cpu.ProgramCounter + offset);
            }
            default:
                throw new ArgumentException("Unexpected addressing mode for addresses");
        }
    }
    
    public static ushort AccumulatorMode(this Cpu cpu)
    {
        return cpu.Accumulator;
    }

    public static ushort ImmediateMode(this Cpu cpu)
    {
        return cpu.Bus.Read8(cpu.ProgramCounter++);
    }


    public static ushort AbsoluteMode(this Cpu cpu)
    {
        var data = cpu.Bus.Read16(cpu.ProgramCounter);
        cpu.ProgramCounter += 2;
        return data;
    }
}