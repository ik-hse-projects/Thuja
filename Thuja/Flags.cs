namespace Thuja
{
    public struct Flags
    {
        private byte Value;
        
        public Flags(byte value)
        {
            Value = value;
        }

        public bool IsSet(Flag flag)
        {
            var mask = (byte) flag;
            return (Value & mask) != 0;
        }
        
        public Flags Set(Flag flag)
        {
            var mask = (byte) flag;
            return new Flags((byte)(Value | mask));
        }

        public bool Equals(Flags other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Flags other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    
    public enum Flag: byte
    {
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Blink = 8
    }
}