using Emu.Subscribers;
using Xunit;

namespace Emu.Tests;

public class InstructionSetTests
{
    [Fact]
    public void TestBrk()
    {
        var cpu = new Cpu();
        var mem = new Memory64k();
        cpu.Bus.AddSubscriber(mem);

        var pcPrev = (ushort)Random.Shared.Next(0x1f1f, 0x2f2f);
        var stackPrev = (byte)Random.Shared.Next(0x00, 0xff); 
        var irq = (ushort)Random.Shared.Next(0x3f3f, 0x4f4f);
        mem.Memory[0xFFFE] = (byte)(irq & 0xFF);
        mem.Memory[0xFFFF] = (byte)(irq >> 8);
        
        cpu.StackPointer = stackPrev;
        cpu.ProgramCounter = pcPrev;
        
        cpu.Status.SetFlag(Flag.Carry, true);
        cpu.Status.SetFlag(Flag.Decimal, true);
        cpu.Status.SetFlag(Flag.Overflow, true);
        cpu.Status.SetFlag(Flag.BreakCommand, false);
        var statusPrev = cpu.Status.Flags;
        
        InstructionSet.Instructions[0x00].Execute(cpu);
        Assert.Equal(statusPrev, cpu.PopStack8());
        Assert.Equal(pcPrev, cpu.PopStack16());
        Assert.True(cpu.Status.IsSet(Flag.BreakCommand));
        Assert.Equal(irq, cpu.ProgramCounter);
    }

    [Fact]
    public void TestInx()
    {
        var cpu = new Cpu();
        InstructionSet.Instructions[0xEB].Execute(cpu);
        Assert.Equal(1, cpu.X);
        
        // Check zero flag
        cpu.X = 0xFF;
        InstructionSet.Instructions[0xEB].Execute(cpu);
        Assert.True(cpu.Status.IsSet(Flag.Zero));
        
        // Check negative flag
        cpu.X = 0xFE;
        InstructionSet.Instructions[0xEB].Execute(cpu);
        Assert.True(cpu.Status.IsSet(Flag.Negative));
    }
    
    [Fact]
    public void TestIny()
    {
        var cpu = new Cpu();
        InstructionSet.Instructions[0xC8].Execute(cpu);
        Assert.Equal(1, cpu.Y);
        
        // Check zero flag
        cpu.Y = 0xFF;
        InstructionSet.Instructions[0xC8].Execute(cpu);
        Assert.True(cpu.Status.IsSet(Flag.Zero));
        
        // Check negative flag
        cpu.Y = 0xFE;
        InstructionSet.Instructions[0xC8].Execute(cpu);
        Assert.True(cpu.Status.IsSet(Flag.Negative));
    }
    
    [Fact]
    public void TestJmp()
    {
        // Indirect
        var cpu = new Cpu();
        var mem = new Memory64k();
        var pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        var derefAddr = (ushort)Random.Shared.Next(0x5F5F, 0x6F6F);
        mem.Memory[pc] = (byte)(targetAddr & 0xFF);
        mem.Memory[pc + 1] = (byte)(targetAddr >> 8);
        mem.Memory[targetAddr] = (byte)(derefAddr & 0xFF);
        mem.Memory[targetAddr + 1] = (byte)(derefAddr >> 8);
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
        
        InstructionSet.Instructions[0x6C].Execute(cpu);
        Assert.Equal(derefAddr, cpu.ProgramCounter);
        
        // Absolute
        cpu = new Cpu();
        mem = new Memory64k();
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        mem.Memory[pc] = (byte)(targetAddr & 0xFF);
        mem.Memory[pc + 1] = (byte)(targetAddr >> 8);
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
        
        InstructionSet.Instructions[0x4C].Execute(cpu);
        Assert.Equal(targetAddr, cpu.ProgramCounter);
    }
}