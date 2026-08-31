using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class IndentedTextWriterExtensions
{
    /// <summary>
    /// 写入信息
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="info">成员信息</param>
    public static void WriteInfo(this IndentedTextWriter writer, IMemberGenerateInfo info)
    {
        info.WriteTo(writer);
    }

    /// <summary>
    /// 写入信息
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="info">对象信息</param>
    public static void WriteInfo(this IndentedTextWriter writer, ObjectGenerateInfo info)
    {
        info.WriteTo(writer);
    }

    /// <summary>
    /// 写入至
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="writer">写入器</param>
    /// <param name="collection">集合</param>
    /// <param name="separator">分隔符</param>
    /// <param name="toString">转换为字符串</param>
    public static void WriteCollection<T>(
        this IndentedTextWriter writer,
        ICollection<T>? collection,
        string separator = "",
        Func<T, string>? toString = null
    )
    {
        if (collection is null || collection.Count == 0)
            return;
        var count = collection.Count;
        var i = 0;
        if (toString is null)
        {
            foreach (var item in collection)
            {
                writer.Write(item);
                if (++i < count)
                    writer.Write(separator);
            }
        }
        else
        {
            foreach (var item in collection)
            {
                writer.Write(toString(item));
                if (++i < count)
                    writer.Write(separator);
            }
        }
    }

    /// <summary>
    /// 写入行至
    /// </summary>
    /// <typeparam name="T">项目类型</typeparam>
    /// <param name="collection">集合</param>
    /// <param name="writer">写入器</param>
    /// <param name="toString">转换为字符串</param>
    public static void WriteLineCollection<T>(
        this IndentedTextWriter writer,
        ICollection<T>? collection,
        Func<T, string>? toString = null
    )
    {
        if (collection is null || collection.Count == 0)
            return;
        if (toString is null)
        {
            foreach (var item in collection)
                writer.WriteLine(item);
        }
        else
        {
            foreach (var item in collection)
                writer.WriteLine(toString(item));
        }
    }

    /// <summary>
    /// 写入
    /// </summary>
    /// <param name="writer">写入器</param>
    /// <param name="str">字符串</param>
    /// <param name="append">附加字符串</param>
    /// <param name="checkMode">检查模式</param>
    public static void WriteIf(
        this IndentedTextWriter writer,
        string str,
        string append = "",
        StringEmptyCheckMode checkMode = StringEmptyCheckMode.IsNotNullOrWhiteSpace
    )
    {
        if (checkMode.Check(str) is false)
            return;
        writer.Write(str);
        writer.Write(append);
    }

    /// <summary>
    /// 检查
    /// </summary>
    /// <param name="checkMode">检查模式</param>
    /// <param name="str">字符串</param>
    /// <returns>检查结果</returns>
    public static bool Check(this StringEmptyCheckMode checkMode, string str)
    {
        return checkMode switch
        {
            StringEmptyCheckMode.IsNotNull => str is not null,
            StringEmptyCheckMode.IsNotNullOrEmpty => string.IsNullOrEmpty(str) is false,
            StringEmptyCheckMode.IsNotNullOrWhiteSpace => string.IsNullOrWhiteSpace(str) is false,
            _ => false,
        };
    }

    //public const string DefaultTabString = "    ";

    //public static TabString[] TabStrings { get; set; } = [new(' ', 4), new('\t', 1)];

    //public static string TrimIndent(this string str, int indent)
    //{
    //    if (indent < 0)
    //        throw new ArgumentOutOfRangeException(nameof(indent));

    //    if (str.Length == 0)
    //        return str;

    //    var sb = new StringBuilder(str);
    //    for (var i = 0; i < str.Length; i++)
    //    {
    //        var c = str[i];
    //        var j = 0;
    //        for (; j < TabStrings.Length; j++)
    //        {
    //            if (TabStrings[j].Char == c)
    //            {
    //                sb.Remove(0, 1);
    //                break;
    //            }
    //        }
    //        if (j == TabStrings.Length)
    //            break;
    //    }
    //    for (var i = 0; i < indent; i++)
    //        sb.Insert(0, DefaultTabString);
    //    return sb.ToString();
    //}
    //public static string TrimIndent(this string str, int indent)
    //{
    //    if (indent < 0)
    //        throw new ArgumentOutOfRangeException(nameof(indent));

    //    if (str.Length == 0)
    //        return str;
    //    for (var i = 0; i < TabStrings.Length; i++)
    //        if (TabStrings[i].Char != str[0])
    //            return str;

    //    var sb = new StringBuilder(str);
    //    var tabCharCounts = new int[TabStrings.Length];
    //    for (var i = 0; i < str.Length; i++)
    //    {
    //        var c = str[i];
    //        for (var j = 0; j < TabStrings.Length; j++)
    //        {
    //            if (TabStrings[j].Char == c)
    //            {
    //                tabCharCounts[j]++;
    //                sb.Remove(0, 1);
    //                break;
    //            }
    //            else
    //                i = str.Length;
    //        }
    //    }
    //    var count = 0;
    //    for (var i = 0; i < tabCharCounts.Length; i++)
    //        count += (int)Math.Ceiling((double)tabCharCounts[i] / TabStrings[i].Count);
    //    for (var i = 0; i < count; i++)
    //        sb.Insert(0, DefaultTabString);
    //    return sb.ToString();
    //}
}

/// <summary>
/// 字符串空检查模式
/// </summary>
public enum StringEmptyCheckMode
{
    /// <summary>
    /// 不是 <see langword="null"/>
    /// </summary>
    IsNotNull = 1,

    /// <summary>
    /// 不是 <see langword="null"/> 或 <see cref="string.Empty"/>
    /// </summary>
    IsNotNullOrEmpty = 2,

    /// <summary>
    /// 不是 <see langword="null"/> 或 <see cref="string.Empty"/> 或 空字符
    /// </summary>
    IsNotNullOrWhiteSpace = 3,
}

/// <summary>
/// 缩进字符串
/// </summary>
public readonly struct TabString : IEquatable<char>
{
    /// <summary>
    /// 字符串
    /// </summary>
    public readonly char Char;

    /// <summary>
    /// 数量
    /// </summary>
    public readonly int Count;

    /// <inheritdoc/>
    /// <summary>默认为<c>    </c>(四个空格)</summary>
    public TabString()
    {
        Char = ' ';
        Count = 4;
    }

    /// <inheritdoc/>
    /// <param name="char">字符</param>
    /// <param name="count">数量</param>
    public TabString(char @char, int count)
    {
        Char = @char;
        Count = count;
    }

    /// <inheritdoc/>
    public bool Equals(char other)
    {
        return Char == other;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not char c)
            return false;
        return Equals(c);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Char.GetHashCode();
    }
}
