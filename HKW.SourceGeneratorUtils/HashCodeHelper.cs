using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.SourceGeneratorUtils;

#pragma warning disable S2436
/// <summary>
/// HashCodeHelper
/// </summary>
public static class HashCodeHelper
{
    private const int Seed = unchecked((int)2166136261);
    private const int Prime = 16777619;

    /// <summary>
    /// 合并两个值的哈希码
    /// </summary>
    /// <typeparam name="T1">值1的类型</typeparam>
    /// <typeparam name="T2">值2的类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <returns>合并后的哈希值</returns>
    public static int Combine<T1, T2>(T1 value1, T2 value2)
    {
        var hash = Seed;
        hash = Mix(hash, GetHashCode(value1));
        hash = Mix(hash, GetHashCode(value2));
        return Finish(hash, 2);
    }

    /// <summary>
    /// 合并三个值的哈希码
    /// </summary>
    /// <typeparam name="T1">值1的类型</typeparam>
    /// <typeparam name="T2">值2的类型</typeparam>
    /// <typeparam name="T3">值3的类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="value3">值3</param>
    /// <returns>合并后的哈希值</returns>
    public static int Combine<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
    {
        var hash = Seed;
        hash = Mix(hash, GetHashCode(value1));
        hash = Mix(hash, GetHashCode(value2));
        hash = Mix(hash, GetHashCode(value3));
        return Finish(hash, 3);
    }

    /// <summary>
    /// 合并四个值的哈希码
    /// </summary>
    /// <typeparam name="T1">值1的类型</typeparam>
    /// <typeparam name="T2">值2的类型</typeparam>
    /// <typeparam name="T3">值3的类型</typeparam>
    /// <typeparam name="T4">值4的类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="value3">值3</param>
    /// <param name="value4">值4</param>
    /// <returns>合并后的哈希值</returns>
    public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
    {
        var hash = Seed;
        hash = Mix(hash, GetHashCode(value1));
        hash = Mix(hash, GetHashCode(value2));
        hash = Mix(hash, GetHashCode(value3));
        hash = Mix(hash, GetHashCode(value4));
        return Finish(hash, 4);
    }

    /// <summary>
    /// 合并五个值的哈希码
    /// </summary>
    /// <typeparam name="T1">值1的类型</typeparam>
    /// <typeparam name="T2">值2的类型</typeparam>
    /// <typeparam name="T3">值3的类型</typeparam>
    /// <typeparam name="T4">值4的类型</typeparam>
    /// <typeparam name="T5">值5的类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="value3">值3</param>
    /// <param name="value4">值4</param>
    /// <param name="value5">值5</param>
    /// <returns>合并后的哈希值</returns>
    public static int Combine<T1, T2, T3, T4, T5>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5
    )
    {
        var hash = Seed;
        hash = Mix(hash, GetHashCode(value1));
        hash = Mix(hash, GetHashCode(value2));
        hash = Mix(hash, GetHashCode(value3));
        hash = Mix(hash, GetHashCode(value4));
        hash = Mix(hash, GetHashCode(value5));
        return Finish(hash, 5);
    }

    /// <summary>
    /// 合并六个值的哈希码
    /// </summary>
    /// <typeparam name="T1">值1的类型</typeparam>
    /// <typeparam name="T2">值2的类型</typeparam>
    /// <typeparam name="T3">值3的类型</typeparam>
    /// <typeparam name="T4">值4的类型</typeparam>
    /// <typeparam name="T5">值5的类型</typeparam>
    /// <typeparam name="T6">值6的类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="value3">值3</param>
    /// <param name="value4">值4</param>
    /// <param name="value5">值5</param>
    /// <param name="value6">值6</param>
    /// <returns>合并后的哈希值</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5,
        T6 value6
    )
    {
        var hash = Seed;
        hash = Mix(hash, GetHashCode(value1));
        hash = Mix(hash, GetHashCode(value2));
        hash = Mix(hash, GetHashCode(value3));
        hash = Mix(hash, GetHashCode(value4));
        hash = Mix(hash, GetHashCode(value5));
        hash = Mix(hash, GetHashCode(value6));
        return Finish(hash, 6);
    }

    /// <summary>
    /// 合并七个值的哈希码
    /// </summary>
    /// <typeparam name="T1">值1的类型</typeparam>
    /// <typeparam name="T2">值2的类型</typeparam>
    /// <typeparam name="T3">值3的类型</typeparam>
    /// <typeparam name="T4">值4的类型</typeparam>
    /// <typeparam name="T5">值5的类型</typeparam>
    /// <typeparam name="T6">值6的类型</typeparam>
    /// <typeparam name="T7">值7的类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="value3">值3</param>
    /// <param name="value4">值4</param>
    /// <param name="value5">值5</param>
    /// <param name="value6">值6</param>
    /// <param name="value7">值7</param>
    /// <returns>合并后的哈希值</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5,
        T6 value6,
        T7 value7
    )
    {
        var hash = Seed;
        hash = Mix(hash, GetHashCode(value1));
        hash = Mix(hash, GetHashCode(value2));
        hash = Mix(hash, GetHashCode(value3));
        hash = Mix(hash, GetHashCode(value4));
        hash = Mix(hash, GetHashCode(value5));
        hash = Mix(hash, GetHashCode(value6));
        hash = Mix(hash, GetHashCode(value7));
        return Finish(hash, 7);
    }

#pragma warning disable S107
    /// <summary>
    /// 合并八个值的哈希码
    /// </summary>
    /// <typeparam name="T1">值1的类型</typeparam>
    /// <typeparam name="T2">值2的类型</typeparam>
    /// <typeparam name="T3">值3的类型</typeparam>
    /// <typeparam name="T4">值4的类型</typeparam>
    /// <typeparam name="T5">值5的类型</typeparam>
    /// <typeparam name="T6">值6的类型</typeparam>
    /// <typeparam name="T7">值7的类型</typeparam>
    /// <typeparam name="T8">值8的类型</typeparam>
    /// <param name="value1">值1</param>
    /// <param name="value2">值2</param>
    /// <param name="value3">值3</param>
    /// <param name="value4">值4</param>
    /// <param name="value5">值5</param>
    /// <param name="value6">值6</param>
    /// <param name="value7">值7</param>
    /// <param name="value8">值8</param>
    /// <returns>合并后的哈希值</returns>
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(
        T1 value1,
        T2 value2,
        T3 value3,
        T4 value4,
        T5 value5,
        T6 value6,
        T7 value7,
        T8 value8
    )
#pragma warning restore S107
    {
        var hash = Seed;
        hash = Mix(hash, GetHashCode(value1));
        hash = Mix(hash, GetHashCode(value2));
        hash = Mix(hash, GetHashCode(value3));
        hash = Mix(hash, GetHashCode(value4));
        hash = Mix(hash, GetHashCode(value5));
        hash = Mix(hash, GetHashCode(value6));
        hash = Mix(hash, GetHashCode(value7));
        hash = Mix(hash, GetHashCode(value8));
        return Finish(hash, 8);
    }

    /// <summary>
    /// 合并对象数组中所有值的哈希码
    /// </summary>
    /// <param name="values">要合并的对象数组</param>
    /// <returns>合并后的哈希值；如果 <paramref name="values"/> 为 <see langword="null"/>，则返回 0</returns>
    public static int Combine(params object?[]? values)
    {
        if (values is null)
            return 0;

        var hash = Seed;
        foreach (var value in values)
            hash = Mix(hash, value?.GetHashCode() ?? 0);

        return Finish(hash, values.Length);
    }

    /// <summary>
    /// 合并泛型数组中所有值的哈希码
    /// </summary>
    /// <typeparam name="T">数组元素的类型</typeparam>
    /// <param name="values">要合并的值数组</param>
    /// <returns>合并后的哈希值；如果 <paramref name="values"/> 为 <see langword="null"/>，则返回 0</returns>
    public static int Combine<T>(params T[]? values)
    {
        if (values is null)
            return 0;

        var hash = Seed;
        foreach (var value in values)
            hash = Mix(hash, GetHashCode(value));

        return Finish(hash, values.Length);
    }

    private static int GetHashCode<T>(T value)
    {
        if (value is null)
            return 0;

        return EqualityComparer<T>.Default.GetHashCode(value);
    }

    private static int Mix(int hash, int value)
    {
        unchecked
        {
            return (hash ^ value) * Prime;
        }
    }

    private static int Finish(int hash, int count)
    {
        unchecked
        {
            hash += count * 4;
            hash ^= hash >> 16;
            hash *= unchecked((int)0x85EBCA6B);
            hash ^= hash >> 13;
            hash *= unchecked((int)0xC2B2AE35);
            hash ^= hash >> 16;
            return hash;
        }
    }
}
#pragma warning restore S2436
