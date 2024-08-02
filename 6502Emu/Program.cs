using Emu.Subscribers;

namespace Emu;

public class Program
{
    public static void Main()
    {
        var cpu = new Cpu();
        cpu.Bus.AddSubscriber(new Memory64k());
        cpu.Bus.Write16(0xFFFE, 0x2f2f);
        cpu.Status.SetFlag(Flag.Carry, true);
        cpu.ProgramCounter = 0x01f;
        InstructionSet.Instructions[0x00].Execute(cpu);
        
    }
}