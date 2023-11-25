using System.Runtime.InteropServices;

namespace LukeFZ.Shared;

public static class SpanExtensions
{
    public static Span<TOut> As<TIn, TOut>(this Span<TIn> span)
        where TIn : unmanaged
        where TOut : unmanaged
        => MemoryMarshal.Cast<TIn, TOut>(span);

    public static ReadOnlySpan<TOut> As<TIn, TOut>(this ReadOnlySpan<TIn> span)
        where TIn : unmanaged
        where TOut : unmanaged
        => MemoryMarshal.Cast<TIn, TOut>(span);

    public static Span<byte> AsBytes<TIn>(this Span<TIn> span)
        where TIn : unmanaged
        => MemoryMarshal.AsBytes(span);

    public static ReadOnlySpan<byte> AsBytes<TIn>(this ReadOnlySpan<TIn> span)
        where TIn : unmanaged
        => MemoryMarshal.AsBytes(span);
}