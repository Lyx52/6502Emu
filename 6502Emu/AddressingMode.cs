namespace Emu;

public enum AddressingMode
{
    Implied,
    Immediate,
    Accumulator,
    Relative,
    ZeroPage,
    ZeroPageX,
    Absolute,
    AbsoluteY,
    AbsoluteX,
    Indirect,
    IndirectX,
    IndirectY
}