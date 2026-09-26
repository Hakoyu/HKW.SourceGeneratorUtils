using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// 获取全名
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>全名</returns>
    public static string GetFullName(this Type type)
    {
        var name = type.Name;
        var tickIndex = name.IndexOf('`');
        if (tickIndex >= 0)
            name = name.Substring(0, tickIndex);

        if (!type.IsGenericTypeDefinition)
            return $"{type.Namespace}.{name}";

        var args = string.Join(", ", type.GetGenericArguments().Select(x => x.Name));
        return $"{type.Namespace}.{name}<{args}>";
    }

    /// <summary>
    /// 全局前缀
    /// </summary>
    public const string Global = "global::";

    /// <summary>
    /// 获取全局全名
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>全局全名</returns>
    public static string GetGlobalFullName(this Type type)
    {
        return Global + GetFullName(type);
    }
}
