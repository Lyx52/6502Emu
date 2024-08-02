using Emu.Subscribers;
using Xunit;

namespace Emu.Tests;

public class AddressingModeTests
{
    [Fact]
    public void TestAbsoluteMode()
    {
        var cpu = new Cpu();
        var mem = new Memory64k();
        var pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        var data = (byte)Random.Shared.Next(0x01, 0xFF);
        mem.Memory[pc] = (byte)(targetAddr & 0xFF);
        mem.Memory[pc + 1] = (byte)(targetAddr >> 8);
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
        
        var readData = cpu.ReadByteFromMemory(AddressingMode.Absolute);
        Assert.Equal(data, readData);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
    }

    [Fact]
    public void TestAbsoluteModeShort()
    {
        var cpu = new Cpu();
        var mem = new Memory64k();
        var pc = (ushort)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (ushort)Random.Shared.Next(0x3F3F, 0x4F4F);
        mem.Memory[pc] = (byte)(targetAddr & 0xFF);
        mem.Memory[pc + 1] = (byte)(targetAddr >> 8);
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
        
        var readData = cpu.ReadShortFromMemory(AddressingMode.Absolute);
        Assert.Equal(targetAddr, readData);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
    }
    
    
    [Fact]
    public void TestIndirectModeShort()
    {
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
        
        var readData = cpu.ReadShortFromMemory(AddressingMode.Indirect);
        Assert.Equal(derefAddr, readData);
        Assert.Equal(pc + 2, cpu.ProgramCounter);
    }
    
    [Fact]
    public void TestZeroPageMode()
    {
        var cpu = new Cpu();
        var mem = new Memory64k();
        var pc = (byte)Random.Shared.Next(0x1F1F, 0x2F2F);
        var targetAddr = (byte)Random.Shared.Next(0x01, 0xFF);
        var data = (byte)Random.Shared.Next(0x01, 0xFF);
        mem.Memory[pc] = targetAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.Bus.AddSubscriber(mem);
        
        var readData = cpu.ReadByteFromMemory(AddressingMode.ZeroPage);
        Assert.Equal(data, readData);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
    }
    
    [Fact]
    public void TestZeroPageXMode()
    {
        var cpu = new Cpu();
        var mem = new Memory64k();
        var x = (byte)Random.Shared.Next(0x01, 0x20);
        var pc = (byte)Random.Shared.Next(0x1F1F, 0x2F2F);
        var baseAddr = (byte)Random.Shared.Next(0x01, 0x80);
        var data = (byte)Random.Shared.Next(0x01, 0xFF);
        byte targetAddr = (byte)(baseAddr + x);
        mem.Memory[pc] = baseAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.X = x;
        cpu.Bus.AddSubscriber(mem);
        
        var readData = cpu.ReadByteFromMemory(AddressingMode.ZeroPageX);
        Assert.Equal(data, readData);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
        
        // Overflow
        x = (byte)Random.Shared.Next(0x20, 0xFF);
        pc = (byte)Random.Shared.Next(0x1F1F, 0x2F2F);
        baseAddr = (byte)Random.Shared.Next(0xFE, 0xFF);
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        targetAddr = (byte)((baseAddr + x) % 0xFF);
        mem.Memory[pc] = baseAddr;
        mem.Memory[targetAddr] = data;
        cpu.ProgramCounter = pc;
        cpu.X = x;
        cpu.Bus.AddSubscriber(mem);
        
        readData = cpu.ReadByteFromMemory(AddressingMode.ZeroPageX);
        Assert.Equal(data, readData);
        Assert.Equal(pc + 1, cpu.ProgramCounter);
    }
}