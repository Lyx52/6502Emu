namespace Emu;

public enum AddressingMode
{
    Implied,
    Immediate,
    Accumulator,
    Relative,
    ZeroPage,
    ZeroPageX,
    ZeroPageY,
    Absolute,
    AbsoluteY,
    AbsoluteX,
    Indirect,
    IndexedIndirect,
    IndirectIndexed
}