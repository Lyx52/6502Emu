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
    
    [Fact]
    public void TestSec()
    {
        var cpu = new Cpu();
        InstructionSet.Instructions[0x38].Execute(cpu);
        Assert.True(cpu.Status.IsSet(Flag.Carry));
    }
    
    [Fact]
    public void TestSed()
    {
        var cpu = new Cpu();
        InstructionSet.Instructions[0xF8].Execute(cpu);
        Assert.True(cpu.Status.IsSet(Flag.Decimal));
    }
    
    [Fact]
    public void TestSei()
    {
        var cpu = new Cpu();
        InstructionSet.Instructions[0x78].Execute(cpu);
        Assert.True(cpu.Status.IsSet(Flag.InterruptDisable));
    }
    
    [Fact]
    public void TestTya()
    {
        var cpu = new Cpu();
        var yValue = (byte)Random.Shared.Next(0x00, 0xFF);
        cpu.Y = yValue;
        InstructionSet.Instructions[0x98].Execute(cpu);
        Assert.Equal(yValue, cpu.Accumulator);

        // Zero flag
        yValue = 0;
        cpu.Y = yValue;
        InstructionSet.Instructions[0x98].Execute(cpu);
        Assert.Equal(yValue, cpu.Accumulator);
        Assert.True(cpu.Status.IsSet(Flag.Zero));
        
        // Negative flag
        yValue = 0xFF;
        cpu.Y = yValue;
        InstructionSet.Instructions[0x98].Execute(cpu);
        Assert.Equal(yValue, cpu.Accumulator);
        Assert.True(cpu.Status.IsSet(Flag.Negative));
    }
    
    [Fact]
    public void TestTxa()
    {
        var cpu = new Cpu();
        var xValue = (byte)Random.Shared.Next(0x00, 0xFF);
        cpu.X = xValue;
        InstructionSet.Instructions[0x8A].Execute(cpu);
        Assert.Equal(xValue, cpu.Accumulator);

        // Zero flag
        xValue = 0;
        cpu.X = xValue;
        InstructionSet.Instructions[0x8A].Execute(cpu);
        Assert.Equal(xValue, cpu.Accumulator);
        Assert.True(cpu.Status.IsSet(Flag.Zero));
        
        // Negative flag
        xValue = 0xFF;
        cpu.X = xValue;
        InstructionSet.Instructions[0x8A].Execute(cpu);
        Assert.Equal(xValue, cpu.Accumulator);
        Assert.True(cpu.Status.IsSet(Flag.Negative));
    }
    
    [Fact]
    public void TestTxs()
    {
        var cpu = new Cpu();
        var xValue = (byte)Random.Shared.Next(0x00, 0xFF);
        cpu.X = xValue;
        InstructionSet.Instructions[0x9A].Execute(cpu);
        Assert.Equal(xValue, cpu.StackPointer);
    }
    
    [Fact]
    public void TestTsx()
    {
        var cpu = new Cpu();
        var spValue = (byte)Random.Shared.Next(0x00, 0xFF);
        cpu.StackPointer = spValue;
        InstructionSet.Instructions[0xBA].Execute(cpu);
        Assert.Equal(spValue, cpu.X);
    }
    
    [Fact]
    public void TestTay()
    {
        var cpu = new Cpu();
        var aValue = (byte)Random.Shared.Next(0x00, 0xFF);
        cpu.Accumulator = aValue;
        InstructionSet.Instructions[0xA8].Execute(cpu);
        Assert.Equal(aValue, cpu.Y);
    }
    
    [Fact]
    public void TestTax()
    {
        var cpu = new Cpu();
        var aValue = (byte)Random.Shared.Next(0x00, 0xFF);
        cpu.Accumulator = aValue;
        InstructionSet.Instructions[0xAA].Execute(cpu);
        Assert.Equal(aValue, cpu.X);
    }
    
    [Fact]
    public void TestSty()
    {
        // ZeroPage
        SetupZeroPage(out var cpu, out var pc, out var data);
        InstructionSet.Instructions[0x84].Execute(cpu);
        Assert.Equal(data, cpu.Y);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // ZeroPageX
        SetupZeroPageX(out cpu, out pc, out data);
        InstructionSet.Instructions[0x94].Execute(cpu);
        Assert.Equal(data, cpu.Y);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // ZeroPageX Overflow
        SetupZeroPageXOverflow(out cpu, out pc, out data);
        InstructionSet.Instructions[0x94].Execute(cpu);
        Assert.Equal(data, cpu.Y);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // Absolute
        SetupAbsolute(out cpu, out pc, out data);
        InstructionSet.Instructions[0x8C].Execute(cpu);
        Assert.Equal(data, cpu.Y);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
    }
    
    [Fact]
    public void TestStx()
    {
        // ZeroPage
        SetupZeroPage(out var cpu, out var pc, out var data);
        InstructionSet.Instructions[0x86].Execute(cpu);
        Assert.Equal(data, cpu.X);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // ZeroPageY
        SetupZeroPageY(out cpu, out pc, out data);
        InstructionSet.Instructions[0x96].Execute(cpu);
        Assert.Equal(data, cpu.X);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // ZeroPageY Overflow
        SetupZeroPageYOverflow(out cpu, out pc, out data);
        InstructionSet.Instructions[0x96].Execute(cpu);
        Assert.Equal(data, cpu.X);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // Absolute
        SetupAbsolute(out cpu, out pc, out data);
        InstructionSet.Instructions[0x8E].Execute(cpu);
        Assert.Equal(data, cpu.X);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
    }
    
     [Fact]
    public void TestSta()
    {
        // ZeroPage
        SetupZeroPage(out var cpu, out var pc, out var data);
        InstructionSet.Instructions[0x85].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // ZeroPageX
        SetupZeroPageX(out cpu, out pc, out data);
        InstructionSet.Instructions[0x95].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // ZeroPageX Overflow
        SetupZeroPageXOverflow(out cpu, out pc, out data);
        InstructionSet.Instructions[0x95].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // Absolute
        SetupAbsolute(out cpu, out pc, out data);
        InstructionSet.Instructions[0x8D].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
        
        // AbsoluteX
        SetupAbsoluteX(out cpu, out pc, out data);
        InstructionSet.Instructions[0x9D].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
        
        // AbsoluteY
        SetupAbsoluteY(out cpu, out pc, out data);
        InstructionSet.Instructions[0x99].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
        
        // IndirectIndexed
        SetupIndirectIndexed(out cpu, out pc, out data);
        InstructionSet.Instructions[0x91].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // IndexedIndirect
        SetupIndexedIndirect(out cpu, out pc, out data);
        InstructionSet.Instructions[0x81].Execute(cpu);
        Assert.Equal(data, cpu.Accumulator);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
    }

    private void SetupZeroPage(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (byte)Random.Shared.Next(0x01, 0xFF);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        mem.Memory[pc] = targetAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
    }

    private void SetupZeroPageY(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        var y = (byte)Random.Shared.Next(0x01, 0x20);
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var baseAddr = (byte)Random.Shared.Next(0x01, 0x80);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        var targetAddr = (byte)(baseAddr + y);
        mem.Memory[pc] = baseAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.X = y;
        cpu.Bus.AddSubscriber(mem);
    }
    private void SetupZeroPageYOverflow(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        var y = (byte)Random.Shared.Next(0x20, 0xFF);
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var baseAddr = (byte)Random.Shared.Next(0xFE, 0xFF);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        var targetAddr = (byte)((baseAddr + y) % 0xFF);
        mem.Memory[pc] = baseAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.X = y;
        cpu.Bus.AddSubscriber(mem);
    }
    private void SetupZeroPageX(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        var x = (byte)Random.Shared.Next(0x01, 0x20);
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var baseAddr = (byte)Random.Shared.Next(0x01, 0x80);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        var targetAddr = (byte)(baseAddr + x);
        mem.Memory[pc] = baseAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.X = x;
        cpu.Bus.AddSubscriber(mem);
    }
    private void SetupZeroPageXOverflow(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        var x = (byte)Random.Shared.Next(0x20, 0xFF);
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var baseAddr = (byte)Random.Shared.Next(0xFE, 0xFF);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        var targetAddr = (byte)((baseAddr + x) % 0xFF);
        mem.Memory[pc] = baseAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.X = x;
        cpu.Bus.AddSubscriber(mem);
    }
    private void SetupAbsolute(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        mem.Memory[pc] = (byte)(targetAddr & 0xFF);
        mem.Memory[pc + 1] = (byte)(targetAddr >> 8);
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
    }
    private void SetupAbsoluteX(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        var x = (byte)Random.Shared.Next(0x01, 0x20);
        mem.Memory[pc] = (byte)(targetAddr & 0xFF);
        mem.Memory[pc + 1] = (byte)(targetAddr >> 8);
        mem.Memory[targetAddr + x] = data;
        cpu.X = x;
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
    }
    
    private void SetupAbsoluteY(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        var y = (byte)Random.Shared.Next(0x01, 0x20);
        mem.Memory[pc] = (byte)(targetAddr & 0xFF);
        mem.Memory[pc + 1] = (byte)(targetAddr >> 8);
        mem.Memory[targetAddr + y] = data;
        cpu.Y = y;
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
    }
    
    private void SetupIndexedIndirect(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        var baseAddr = (byte)Random.Shared.Next(0x01, 0xFF);
        var x = (byte)Random.Shared.Next(0x01, 0xFF);
        var addrSum = (byte)((baseAddr + x) % 0xFF);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        mem.Memory[pc] = baseAddr;
        mem.Memory[addrSum] = (byte)(targetAddr & 0xFF);
        mem.Memory[addrSum + 1] = (byte)(targetAddr >> 8);
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.X = x;
        cpu.Bus.AddSubscriber(mem);
    }
    
    private void SetupIndirectIndexed(out Cpu cpu, out ushort pc, out byte data)
    {
        cpu = new Cpu();
        var mem = new Memory64k();
        pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        var baseAddr = (byte)Random.Shared.Next(0x01, 0xFF);
        var y = (byte)Random.Shared.Next(0x01, 0xFF);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        mem.Memory[pc] = baseAddr;
        mem.Memory[baseAddr] = (byte)(targetAddr & 0xFF);
        mem.Memory[baseAddr + 1] = (byte)(targetAddr >> 8);
        mem.Memory[targetAddr + y] = data;
        cpu.ProgramCounter = pc;
        cpu.Y = y;
        cpu.Bus.AddSubscriber(mem);
    }
}