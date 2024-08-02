namespace Emu;

public static class InstructionSet
{
    public static Dictionary<byte, Instruction> Instructions { get; set; } = new Dictionary<byte, Instruction>()
    
    {
        { 0x00, new Instruction(CmdBrk, "BRK", AddressingMode.Implied) },
        { 0xBA, new Instruction(CmdTsx, "TSX", AddressingMode.Implied) },
        
        // STA
        { 0x85, new Instruction(CmdSta, "STA", AddressingMode.ZeroPage)  },
        { 0x95, new Instruction(CmdSta, "STA", AddressingMode.ZeroPageX) },
        { 0x8D, new Instruction(CmdSta, "STA", AddressingMode.Absolute)  },
        { 0x9D, new Instruction(CmdSta, "STA", AddressingMode.AbsoluteX) },
        { 0x99, new Instruction(CmdSta, "STA", AddressingMode.AbsoluteY) },
        { 0x81, new Instruction(CmdSta, "STA", AddressingMode.IndirectX) },
        { 0x91, new Instruction(CmdSta, "STA", AddressingMode.IndirectY) },
        
        // INX
        { 0xEB, new Instruction(CmdInx, "INX", AddressingMode.Implied) },
        
        // INY
        { 0xC8, new Instruction(CmdIny, "INY", AddressingMode.Implied) },
        
        // JMP
        { 0x4C, new Instruction(CmdJmp, "JMP", AddressingMode.Absolute) },
        { 0x6C, new Instruction(CmdJmp, "JMP", AddressingMode.Indirect) },
    };

    public static FlagSet CmdNop(Cpu cpu, AddressingMode mode)
    {
        return cpu.Status;
    }
    
    /*
     *  BRK
     *  Modes: Implied
     */
    public static FlagSet CmdBrk(Cpu cpu, AddressingMode mode)
    {
        cpu.PushStack16(cpu.ProgramCounter);
        cpu.PushStack8(cpu.Status);
        cpu.ProgramCounter = cpu.Bus.Read16(0xFFFE); // IRQ Vector - 0xFFFE, 0xFFFF
        cpu.Status.SetFlag(Flag.BreakCommand, true);
        return cpu.Status;
    }
    
    /*
     *  TSX
     *  Modes: Implied
     */
    public static FlagSet CmdTsx(Cpu cpu, AddressingMode mode)
    {
        cpu.X = cpu.StackPointer;
        cpu.CheckAndSetZero(cpu.X);
        cpu.CheckAndSetNegative(cpu.X);
        return cpu.Status;
    }
    
    /*
     *  TXS
     *  Modes: Implied
     */
    public static FlagSet CmdTxs(Cpu cpu, AddressingMode mode)
    {
        cpu.StackPointer = cpu.X;
        return cpu.Status;
    }
    
    /*
     *  STA
     *  Modes: 
     */
    public static FlagSet CmdSta(Cpu cpu, AddressingMode mode)
    {
        cpu.Accumulator = cpu.ReadByteFromMemory(mode);
        return cpu.Status;
    }
    
    /*
     * INX
     * Modes: Implied
     */
    public static FlagSet CmdInx(Cpu cpu, AddressingMode mode)
    {
        cpu.X++;
        cpu.CheckAndSetZero(cpu.X);
        cpu.CheckAndSetNegative(cpu.X);
        return cpu.Status;
    }

    /*
     * INY
     * Modes: Implied
     */
    public static FlagSet CmdIny(Cpu cpu, AddressingMode mode)
    {
        cpu.Y++;
        cpu.CheckAndSetZero(cpu.Y);
        cpu.CheckAndSetNegative(cpu.Y);
        return cpu.Status;
    }
    
    /*
     * JMP
     * Modes: Absolute, Indirect
     */
    public static FlagSet CmdJmp(Cpu cpu, AddressingMode mode)
    {
        var targetAddr = cpu.ReadShortFromMemory(mode);
        cpu.ProgramCounter = targetAddr;
        return cpu.Status;
    }
}