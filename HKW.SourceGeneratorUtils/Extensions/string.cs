using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 首字母小写
    /// </summary>
    /// <param name="str"></param>
    /// <returns>首字母为小写的字符串</returns>
    public static string FirstLetterToLower(this string str)
    {
        if (string.IsNullOrWhiteSpace(str) || char.IsLower(str, 0))
            return str;
        var array = str.ToCharArray();
        array[0] = char.ToLowerInvariant(array[0]);
        return new string(array);
    }

    /// <summary>
    /// 将符号 <c>&lt;&gt;</c> 替换为 <c>{}</c>
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>替换完成的字符串</returns>
    public static string ReplaceBraces(this string str)
    {
        var chars = str.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = chars[i] switch
            {
                '<' => '{',
                '>' => '}',
                _ => chars[i],
            };
        }
        return new string(chars);
    }

    private static string[] _lineSeparator = ["\r\n", "\r", "\n"];

    /// <summary>
    /// 分割行
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="options">设置</param>
    /// <returns>分割完成的字符串集合</returns>
    public static string[] SplitLine(
        this string str,
        StringSplitOptions options = StringSplitOptions.None
    )
    {
        if (str is null || str.Length == 0)
            return Array.Empty<string>();
        return str.Split(_lineSeparator, options);
    }
}
