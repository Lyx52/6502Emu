using Emu.Subscribers;
using Xunit;

namespace Emu.Tests;

public class EmuMemoryTests
{
    [Fact]
    public void TestReadWriteBytes()
    {
        var cpu = new Cpu();
        cpu.Bus.AddSubscriber(new Memory64k());

        // Random
        var address = (ushort)Random.Shared.Next(0x0000, 0xFFFF);
        var data = (byte)Random.Shared.Next(0x01, 0xFF);
        cpu.Bus.Write8(address, data);
        var readData = cpu.Bus.Read8(address);
        
        Assert.Equal(data, readData);
        
        // Start
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        cpu.Bus.Write8(0x0000, data);
        readData = cpu.Bus.Read8(0x0000);
        
        Assert.Equal(data, readData);
        
        // End
        data = (byte)Random.Shared.Next(0x01, 0xFF);
        cpu.Bus.Write8(0xFFFF, data);
        readData = cpu.Bus.Read8(0xFFFF);
        
        Assert.Equal(data, readData);
    }
    
    [Fact]
    public void TestReadWriteShort()
    {
        var cpu = new Cpu();
        cpu.Bus.AddSubscriber(new Memory64k());

        // Random
        var address = (ushort)Random.Shared.Next(0x0000, 0xFFFF);
        var data = (ushort)Random.Shared.Next(0x1f1f, 0xFFFF);
        cpu.Bus.Write16(address, data);
        var readData = cpu.Bus.Read16(address);
        
        Assert.Equal(data, readData);
        
        // Start
        data = (ushort)Random.Shared.Next(0x1f1f, 0xFFFF);
        cpu.Bus.Write16(0x0000, data);
        readData = cpu.Bus.Read16(0x0000);
        
        Assert.Equal(data, readData);
        
        // End
        data = (ushort)Random.Shared.Next(0x1f1f, 0xFFFF);
        cpu.Bus.Write16(0xFFFF, data);
        readData = cpu.Bus.Read16(0xFFFF);
        
        Assert.Equal(data, readData);
    }

    [Fact]
    public void TestReadWriteStackBytes()
    {
        var cpu = new Cpu();
        cpu.Bus.AddSubscriber(new Memory64k());
        
        var data = (byte)Random.Shared.Next(0x01, 0xFF);
        cpu.PushStack8(data);
        var readData = cpu.PopStack8();
        
        Assert.Equal(data, readData);
    }
    
    [Fact]
    public void TestReadWriteStackShort()
    {
        var cpu = new Cpu();
        cpu.Bus.AddSubscriber(new Memory64k());
        
        var data = (ushort)Random.Shared.Next(0x1f1f, 0xFFFF);
        cpu.PushStack16(data);
        var readData = cpu.PopStack16();
        
        Assert.Equal(data, readData);
    }
}