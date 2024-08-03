using Exception = System.Exception;

namespace Emu;

public class Cpu
{
    public FlagSet Status { get; set; } = new FlagSet();
    public Bus Bus { get; set; } = new Bus();
    public ushort ProgramCounter { get; set; }
    public byte Accumulator { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public byte StackPointer { get; set; } = 0x00; // Range 0x0100 - 0x01ff, holds lower 8 bits

    public void PushStack16(ushort data)
    {
        Bus.Write16((ushort)(0x01 << 8 | StackPointer), data);
        StackPointer += 2;
    }

    public ushort PopStack16()
    {
        StackPointer -= 2;
        var data = Bus.Read16((ushort)(0x01 << 8 | StackPointer));
        return data;
    }
    
    public void PushStack8(byte data)
    {
        Bus.Write8((ushort)(0x01 << 8 | StackPointer), data);
        StackPointer++;
    }

    public byte PopStack8()
    {
        StackPointer--;
        var data = Bus.Read8((ushort)(0x01 << 8 | StackPointer));
        return data;
    }

    public ushort ReadAddrFromMemory(AddressingMode mode)
    {
        switch (mode)
        {
            case AddressingMode.Absolute:
            {
                var addr = Bus.Read16(ProgramCounter);
                ProgramCounter += 2;
                return addr;
            }
            case AddressingMode.AbsoluteX:
            {
                ushort baseAddr = Bus.Read16(ProgramCounter);
                ProgramCounter += 2;
                return (ushort)((baseAddr + X) % 0xFFFF);
            }
            case AddressingMode.AbsoluteY:
            {
                ushort baseAddr = Bus.Read16(ProgramCounter);
                ProgramCounter += 2;
                return (ushort)((baseAddr + Y) % 0xFFFF);
            }
            case AddressingMode.Indirect:
            {
                var addr = Bus.Read16(ProgramCounter);
                ProgramCounter += 2;
                return Bus.Read16(addr);
            }
            case AddressingMode.IndexedIndirect:
            {
                var baseAddr = Bus.Read8(ProgramCounter++);
                baseAddr += X;
                return Bus.Read16(baseAddr);
            }
            case AddressingMode.IndirectIndexed:
            {
                var targetZpAddr = Bus.Read8(ProgramCounter++);
                var baseAddr = Bus.Read16(targetZpAddr);
                
                baseAddr += Y;
                return baseAddr;
            }
            case AddressingMode.ZeroPage:
            {
                var addr = Bus.Read8(ProgramCounter++);
                return addr;
            }
            case AddressingMode.ZeroPageX:
            {
                ushort baseAddr = Bus.Read8(ProgramCounter++);
                return (ushort)((baseAddr + X) % 0xFF);
            }
            case AddressingMode.ZeroPageY:
            {
                ushort baseAddr = Bus.Read8(ProgramCounter++);
                return (ushort)((baseAddr + Y) % 0xFF);
            }
            default:
                throw new Exception($"Cant read short from memory using {mode}");
        }
    }

    public byte ReadByteFromMemory(AddressingMode mode)
    {
        switch (mode)
        {
            case AddressingMode.Accumulator:
            {
                return Accumulator;
            }
            case AddressingMode.Immediate:
            {
                return Bus.Read8(ProgramCounter++);
            }
            case AddressingMode.Absolute:
            {
                var addr = Bus.Read16(ProgramCounter);
                ProgramCounter += 2;
                return Bus.Read8(addr);
            }
            case AddressingMode.AbsoluteX:
            {
                ushort baseAddr = Bus.Read16(ProgramCounter);
                var addr = (ushort)((baseAddr + X) % 0xFFFF);
                ProgramCounter += 2;
                return Bus.Read8(addr);
            }
            case AddressingMode.AbsoluteY:
            {
                ushort baseAddr = Bus.Read16(ProgramCounter);
                var addr = (ushort)((baseAddr + Y) % 0xFFFF);
                ProgramCounter += 2;
                return Bus.Read8(addr);
            }
            case AddressingMode.ZeroPage:
            {
                var addr = Bus.Read8(ProgramCounter++);
                return Bus.Read8(addr);
            }
            case AddressingMode.ZeroPageX:
            {
                ushort baseAddr = Bus.Read8(ProgramCounter++);
                return Bus.Read8((ushort)((baseAddr + X) % 0xFF));
            }
            case AddressingMode.ZeroPageY:
            {
                ushort baseAddr = Bus.Read8(ProgramCounter++);
                return Bus.Read8((ushort)((baseAddr + Y) % 0xFF));
            }
            case AddressingMode.Indirect:
            {
                var addr = Bus.Read16(ProgramCounter);
                ProgramCounter += 2;
                var derefAddr = Bus.Read16(addr);

                return Bus.Read8(derefAddr);
            }
            case AddressingMode.IndexedIndirect:
            {
                var baseAddr = Bus.Read8(ProgramCounter++);
                baseAddr = (byte)((baseAddr + X) % 0xFF);
                var targetAddr = Bus.Read16(baseAddr);
                return Bus.Read8(targetAddr);
            }
            case AddressingMode.IndirectIndexed:
            {
                var targetZpAddr = Bus.Read8(ProgramCounter++);
                var baseAddr = Bus.Read16(targetZpAddr);
                
                baseAddr += Y;
                return Bus.Read8(baseAddr);
            }
            default:
                throw new Exception($"Cant read byte from memory using {mode}");
        }
    }
    
    public void CheckAndSetNegative(byte value)
    {
        Status.SetFlag(Flag.Negative, (value & 0x1 << 7) == 0x1 << 7);
    } 
    
    public void CheckAndSetZero(byte value)
    {
        Status.SetFlag(Flag.Zero, value == 0);
    } 
}