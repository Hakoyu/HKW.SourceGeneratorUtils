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
    /// 获取Get方法
    /// </summary>
    /// <param name="propertySymbol">属性</param>
    /// <param name="isFunc">使用自身静态方法</param>
    /// <returns>属性Get方法信息</returns>
    /// <remarks><code><![CDATA[
    ///
    /// /// IN:
    /// public string FullName => _fullName;
    /// /// OUT:
    /// _fullName
    ///
    /// /// IN:
    /// public string FullName => $"{FirstName}_{LastName}";
    /// /// OUT:
    /// $"{FirstName}_{LastName}"
    ///
    /// /// IN:
    /// public string FullName
    /// {
    ///     get { return $"{FirstName}_{LastName}"; }
    /// }
    /// /// OUT:
    /// { return $"{FirstName}_{LastName}"; }
    /// ]]></code></remarks>
    public static string? GetGetMethodStr(this IPropertySymbol propertySymbol, out bool isFunc)
    {
        isFunc = false;
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
            isFunc = true;
            // 删除get块左右大括号
            sb.Remove(sb.Length - 1, 1);
            sb.Remove(0, getMethodStr.IndexOf("{") + 1);
        }
        else
        {
            // 删除表达式符号
            sb.Remove(0, getMethodStr.IndexOf("=>") + 2);
        }
        return sb.ToString();
    }
}
