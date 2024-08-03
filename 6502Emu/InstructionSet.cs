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
        { 0x81, new Instruction(CmdSta, "STA", AddressingMode.IndexedIndirect) },
        { 0x91, new Instruction(CmdSta, "STA", AddressingMode.IndirectIndexed) },
        
        // STX
        { 0x86, new Instruction(CmdStx, "STX", AddressingMode.ZeroPage) },
        { 0x96, new Instruction(CmdStx, "STX", AddressingMode.ZeroPageX) },
        { 0x8E, new Instruction(CmdStx, "STX", AddressingMode.Absolute) },
        
        // STY
        { 0x84, new Instruction(CmdSty, "STY", AddressingMode.ZeroPage) },
        { 0x94, new Instruction(CmdSty, "STY", AddressingMode.ZeroPageX) },
        { 0x8C, new Instruction(CmdSty, "STY", AddressingMode.Absolute) },
        
        // SEI
        { 0x78, new Instruction(CmdSei, "SEI", AddressingMode.Implied) },
        
        // SED
        { 0xF8, new Instruction(CmdSed, "SED", AddressingMode.Implied) },
        
        // SEC
        { 0x38, new Instruction(CmdSec, "SEC", AddressingMode.Implied) },
        
        // INX
        { 0xEB, new Instruction(CmdInx, "INX", AddressingMode.Implied) },
        
        // INY
        { 0xC8, new Instruction(CmdIny, "INY", AddressingMode.Implied) },
        
        // JMP
        { 0x4C, new Instruction(CmdJmp, "JMP", AddressingMode.Absolute) },
        { 0x6C, new Instruction(CmdJmp, "JMP", AddressingMode.Indirect) },
        
        // TYA
        { 0x98, new Instruction(CmdTya, "TYA", AddressingMode.Implied) },
        
        // TXA
        { 0x8A, new Instruction(CmdTxa, "TXA", AddressingMode.Implied) },
        
        // TXS
        { 0x9A, new Instruction(CmdTxs, "TXS", AddressingMode.Implied) },
        
        // TAY
        { 0xA8, new Instruction(CmdTay, "TAY", AddressingMode.Implied) },
        
        // TAX
        { 0xAA, new Instruction(CmdTax, "TAX", AddressingMode.Implied) },
        
        // NOP
        { 0xEA, new Instruction(CmdNop, "NOP", AddressingMode.Implied) },
    };

    /*
     *  NOP
     *  Modes: Implied
     */
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
     *  SEI
     *  Modes: Implied
     */
    public static FlagSet CmdSei(Cpu cpu, AddressingMode mode)
    {
        cpu.Status.SetFlag(Flag.InterruptDisable, true);
        return cpu.Status;
    }
    
    /*
     *  SED
     *  Modes: Implied
     */
    public static FlagSet CmdSed(Cpu cpu, AddressingMode mode)
    {
        cpu.Status.SetFlag(Flag.Decimal, true);
        return cpu.Status;
    }
    
    /*
     *  SEC
     *  Modes: Implied
     */
    public static FlagSet CmdSec(Cpu cpu, AddressingMode mode)
    {
        cpu.Status.SetFlag(Flag.Carry, true);
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
     *  STX
     *  Modes: ZeroPage, ZeroPageY, Absolute
     */
    public static FlagSet CmdStx(Cpu cpu, AddressingMode mode)
    {
        cpu.X = cpu.ReadByteFromMemory(mode);
        return cpu.Status;
    }

    /*
     *  STY
     *  Modes: ZeroPage, ZeroPageY, Absolute
     */
    public static FlagSet CmdSty(Cpu cpu, AddressingMode mode)
    {
        cpu.Y = cpu.ReadByteFromMemory(mode);
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
        var targetAddr = cpu.ReadAddrFromMemory(mode);
        cpu.ProgramCounter = targetAddr;
        return cpu.Status;
    }
    
    /*
     * TYA
     * Modes: Implied
     */
    public static FlagSet CmdTya(Cpu cpu, AddressingMode mode)
    {
        cpu.Accumulator = cpu.Y;
        cpu.CheckAndSetZero(cpu.Y);
        cpu.CheckAndSetNegative(cpu.Y);
        return cpu.Status;
    }
    
    /*
     * TXA
     * Modes: Implied
     */
    public static FlagSet CmdTxa(Cpu cpu, AddressingMode mode)
    {
        cpu.Accumulator = cpu.X;
        cpu.CheckAndSetZero(cpu.X);
        cpu.CheckAndSetNegative(cpu.X);
        return cpu.Status;
    }
    
    /*
     * TAY
     * Modes: Implied
     */
    public static FlagSet CmdTay(Cpu cpu, AddressingMode mode)
    {
        cpu.Y = cpu.Accumulator;
        cpu.CheckAndSetZero(cpu.Accumulator);
        cpu.CheckAndSetNegative(cpu.Accumulator);
        return cpu.Status;
    }
    
    /*
     * TAX
     * Modes: Implied
     */
    public static FlagSet CmdTax(Cpu cpu, AddressingMode mode)
    {
        cpu.X = cpu.Accumulator;
        cpu.CheckAndSetZero(cpu.Accumulator);
        cpu.CheckAndSetNegative(cpu.Accumulator);
        return cpu.Status;
    }
    
    /*
     * SBC
     * Modes: Implied
     */
    public static FlagSet CmdSbc(Cpu cpu, AddressingMode mode)
    {
        var data = cpu.ReadByteFromMemory(mode);
        cpu.CheckAndSetZero(cpu.Accumulator);
        cpu.CheckAndSetNegative(cpu.Accumulator);
        return cpu.Status;
    }
}