public class BitArray
{
    public const uint NONE = 0;
    public const uint FULL = 0xffffffff;

    public static bool HasFlag(uint value, uint flags)
    {
        return (value & flags) != 0;
    }

    public static bool HasFlagByPos(uint value, uint pos)
    {
        if (pos >= 32)
        {
            return false;
        }

        return (value & (uint)(1 << (byte)pos)) != 0;
    }

    public static uint AddFlag(uint value, uint flags)
    {
        return value | flags;
    }

    public static uint AddFlagByPos(uint value, uint pos)
    {
        if (pos >= 32)
        {
            return value;
        }

        return value | (uint)(1 << (byte)pos);
    }

    public static uint RemoveFlag(uint value, uint flags)
    {
        return value & (~flags);
    }

    public static uint RemoveFlagByPos(uint value, uint pos)
    {
        if (pos >= 32)
        {
            return value;
        }

        return value & (~((uint)(1 << (byte)pos)));
    }

    protected uint Value;

    public BitArray()
    {
        Value = 0;
    }

    public bool HasFlag(uint flags)
    {
        return HasFlag(Value, flags);
    }

    public bool HasFlagByPos(uint pos)
    {
        return HasFlagByPos(Value, pos);
    }

    public void AddFlag(uint flags)
    {
        Value = AddFlag(Value, flags);
    }

    public void AddFlagByPos(uint pos)
    {
        Value = AddFlagByPos(Value, pos);
    }

    public void RemoveFlag(uint flags)
    {
        Value = Value & (~flags);
    }

    public void RemoveFlagByPos(uint pos)
    {
        Value = RemoveFlagByPos(Value, pos);
    }

    public uint GetValue()
    {
        return Value;
    }
}