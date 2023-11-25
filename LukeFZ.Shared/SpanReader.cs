using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LukeFZ.Shared;

public ref struct SpanReader(ReadOnlySpan<byte> data, int offset = 0, bool littleEndian = true)
{
    public static SpanWriter LittleEndian(Span<byte> data, int offset = 0) => new(data, offset);
    public static SpanWriter BigEndian(Span<byte> data, int offset = 0) => new(data, offset, false);

    public int Offset = offset;
    public readonly byte Peek => _data[Offset];

    private readonly ReadOnlySpan<byte> _data = data;
    private readonly bool _littleEndian = littleEndian;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte() => _data[Offset++];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        var val = _data.Slice(Offset, length);
        Offset += length;
        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        var val = MemoryMarshal.Read<T>(_data.Slice(Offset));
        Offset += Unsafe.SizeOf<T>();
        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64()
    {
        var val = Read<ulong>();
        return _littleEndian ? val : BinaryPrimitives.ReverseEndianness(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32()
    {
        var val = Read<uint>();
        return _littleEndian ? val : BinaryPrimitives.ReverseEndianness(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16()
    {
        var val = Read<ushort>();
        return _littleEndian ? val : BinaryPrimitives.ReverseEndianness(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64()
    {
        var val = Read<long>();
        return _littleEndian ? val : BinaryPrimitives.ReverseEndianness(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32()
    {
        var val = Read<int>();
        return _littleEndian ? val : BinaryPrimitives.ReverseEndianness(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16()
    {
        var val = Read<short>();
        return _littleEndian ? val : BinaryPrimitives.ReverseEndianness(val);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadVarUInt64()
    {
        var value = 0uL;
        var shift = 0;
        do
        {
            var b = ReadByte();
            value |= (b & 0x7fu) << shift;
            if (b < 0x80)
                break;

            shift += 7;

        } while (64 > shift);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadVarInt64()
    {
        return (long)ReadVarUInt64();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadVarUInt32()
    {
        return (uint)ReadVarUInt64();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadVarInt32()
    {
        return (int)ReadVarUInt64();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly int ReadNullTerminatedInternal()
    {
        var length = 0;

        while (_data.Length > Offset + length)
        {
            if (_data[Offset + length++] == 0)
                return length;
        }

        throw new InvalidDataException("Failed to find string in span.");
    }

    public ReadOnlySpan<byte> ReadNullTerminatedStringSpan()
    {
        var length = ReadNullTerminatedInternal();
        var val = _data.Slice(Offset, length - 1);
        Offset += length;

        return val;
    }

    public string ReadNullTerminatedString()
        => Encoding.UTF8.GetString(ReadNullTerminatedStringSpan());
}