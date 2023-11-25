using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LukeFZ.Shared;

public ref struct SpanWriter(Span<byte> data, int offset = 0, bool littleEndian = true)
{
    public static SpanWriter LittleEndian(Span<byte> data, int offset = 0) => new(data, offset);
    public static SpanWriter BigEndian(Span<byte> data, int offset = 0) => new(data, offset, false);

    public int Offset = offset;

    private readonly Span<byte> _data = data;
    private readonly bool _littleEndian = littleEndian;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte val) => _data[Offset++] = val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(ReadOnlySpan<byte> val)
    {
        var len = val.Length;
        val.CopyTo(_data.Slice(Offset, len));
        Offset += len;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T val) where T : unmanaged
    {
        MemoryMarshal.Write(_data.Slice(Offset), val);
        Offset += Unsafe.SizeOf<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64(ulong val)
    {
        if (!_littleEndian)
            val = BinaryPrimitives.ReverseEndianness(val);

        Write(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32(uint val)
    {
        if (!_littleEndian)
            val = BinaryPrimitives.ReverseEndianness(val);

        Write(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16(ushort val)
    {
        if (!_littleEndian)
            val = BinaryPrimitives.ReverseEndianness(val);

        Write(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64(long val)
    {
        if (!_littleEndian)
            val = BinaryPrimitives.ReverseEndianness(val);

        Write(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32(int val)
    {
        if (!_littleEndian)
            val = BinaryPrimitives.ReverseEndianness(val);

        Write(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16(short val)
    {
        if (!_littleEndian)
            val = BinaryPrimitives.ReverseEndianness(val);

        Write(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVarUInt64(ulong val)
    {
        do
        {
            var current = (byte)(val & 0x7f);
            if (val > 0x7f)
                current |= 0x80;

            WriteByte(current);

            val >>= 7;
        } while (val != 0);
    }
}