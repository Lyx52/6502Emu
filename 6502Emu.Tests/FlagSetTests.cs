using Xunit;

namespace Emu.Tests;

public class FlagSetTests
{
    [Fact]
    public void TestFlagSet()
    {
        var flags = new FlagSet();

        flags.SetFlag(Flag.Carry, true);
        flags.SetFlag(Flag.Decimal, true);
        flags.SetFlag(Flag.Negative, true);
        flags.SetFlag(Flag.Overflow, true);
        flags.SetFlag(Flag.BreakCommand, true);
        flags.SetFlag(Flag.Zero, true);
        flags.SetFlag(Flag.InterruptDisable, true);
        
        Assert.True(flags.IsSet(Flag.Carry));
        Assert.True(flags.IsSet(Flag.Decimal));
        Assert.True(flags.IsSet(Flag.Negative));
        Assert.True(flags.IsSet(Flag.Overflow));
        Assert.True(flags.IsSet(Flag.BreakCommand));
        Assert.True(flags.IsSet(Flag.Zero));
        Assert.True(flags.IsSet(Flag.InterruptDisable));
        Assert.Equal(0xFF, flags.Flags);
    }
}