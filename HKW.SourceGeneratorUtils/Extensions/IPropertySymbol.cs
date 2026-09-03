using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class IPropertySymbolExtensions
{
    /// <summary>
    /// 获取Get方法字符串
    /// </summary>
    /// <param name="propertySymbol">属性</param>
    /// <returns>属性Get方法</returns>
    /// <remarks><code><![CDATA[
    ///
    /// /// IN:
    /// public string FullName => _fullName;
    /// /// OUT:
    /// return _fullName;
    ///
    /// /// IN:
    /// public string FullName => $"{FirstName}_{LastName}";
    /// /// OUT:
    /// return $"{FirstName}_{LastName}";
    ///
    /// /// IN:
    /// public string FullName
    /// {
    ///     get { return $"{FirstName}_{LastName}"; }
    /// }
    /// /// OUT:
    /// return $"{FirstName}_{LastName}";
    /// ]]></code></remarks>
    public static string? GetGetMethodContent(this IPropertySymbol propertySymbol)
    {
        if (propertySymbol.GetMethod is null)
            return null;
        var getMethodStr = propertySymbol
            .GetMethod.DeclaringSyntaxReferences.First()
            .GetSyntax()
            .ToString();
        var sb = new StringBuilder(getMethodStr);
        // 判断是否为get块
        if (getMethodStr.EndsWith("}"))
        {
            // 删除get块左右大括号
            sb.Remove(sb.Length - 1, 1);
            sb.Remove(0, getMethodStr.IndexOf("{") + 1);
        }
        else
        {
            // 删除表达式符号
            sb.Remove(0, getMethodStr.IndexOf("=>") + 2);
            sb.Insert(0, "return ");
            sb.Append(';');
        }
        return sb.ToString();
    }

    /// <summary>
    /// 尝试获取Get方法字符串
    /// </summary>
    /// <param name="propertySymbol">属性</param>
    /// <param name="getMethodStr">Get方法字符串</param>
    /// <returns>是否获取成功</returns>
    public static bool TryGetGetMethodContent(
        this IPropertySymbol propertySymbol,
        out string getMethodStr
    )
    {
        getMethodStr = GetGetMethodContent(propertySymbol)!;
        return getMethodStr is not null;
    }
}
