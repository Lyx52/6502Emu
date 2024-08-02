namespace Emu;

public class Instruction
{
    public Instruction(Func<Cpu, AddressingMode, FlagSet> command, string name, AddressingMode mode)
    {
        Command = command;
        Name = name;
        Mode = mode;
    }
    
    public Func<Cpu, AddressingMode, FlagSet> Command { get; set; }
    public string Name { get; set; }
    public AddressingMode Mode { get; set; }

    public FlagSet Execute(Cpu cpu)
    {
        return Command.Invoke(cpu, Mode);
    }
}