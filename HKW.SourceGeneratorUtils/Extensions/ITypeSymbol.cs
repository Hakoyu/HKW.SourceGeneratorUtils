using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class ITypeSymbolExtensions
{
    /// <summary>
    /// 是空类型
    /// </summary>
    /// <param name="symbol">符号类型</param>
    /// <param name="specialType">特殊类型</param>
    /// <returns>是否空类型</returns>
    public static bool IsSpecialType(this ITypeSymbol symbol, SpecialType specialType)
    {
        return symbol.SpecialType == specialType;
    }

    /// <summary>
    /// 获取接口
    /// </summary>
    /// <param name="symbol">符号</param>
    /// <param name="interfaceFullName">接口全名</param>
    /// <returns>接口</returns>
    public static INamedTypeSymbol? GetInterface(this ITypeSymbol symbol, string interfaceFullName)
    {
        var isGlobal = interfaceFullName.IsGlobalName();
        return symbol.AllInterfaces.FirstOrDefault(i =>
            i.GetFullName(isGlobal) == interfaceFullName
            || i.OriginalDefinition.GetFullName(isGlobal) == interfaceFullName
        );
    }

    /// <summary>
    /// 实现接口
    /// </summary>
    /// <param name="symbol">符号</param>
    /// <param name="interfaceFullName">接口全名</param>
    /// <returns>是否实现</returns>
    public static bool HasInterface(this ITypeSymbol symbol, string interfaceFullName)
    {
        var isGlobal = interfaceFullName.IsGlobalName();
        return symbol.AllInterfaces.Any(i =>
            i.GetFullName(isGlobal) == interfaceFullName
            || i.OriginalDefinition.GetFullName(isGlobal) == interfaceFullName
        );
    }

    /// <summary>
    /// 获取任务返回值
    /// </summary>
    /// <param name="symbol">符号</param>
    /// <returns>如果类型是 <see cref="Task{T}"/> 则返回任务返回值, 否则返回 <see langword="null"/></returns>
    public static INamedTypeSymbol? GetTaskResult(this ITypeSymbol symbol)
    {
        var currentType = symbol;
        while (currentType != null)
        {
            if (
                currentType.OriginalDefinition?.GetGlobalFullName()
                == GeneratorHelper.TaskResultFullName
            )
                return ((INamedTypeSymbol)currentType).TypeArguments[0] as INamedTypeSymbol;
            currentType = currentType.BaseType;
        }
        return null;
    }

    /// <summary>
    /// 获取点替换为下划线的全名
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <returns>全名</returns>
    public static string GetUnderlineFullName(this ITypeSymbol typeSymbol)
    {
        return $"{typeSymbol.ContainingNamespace.ToString().ReplaceDotToUnderline()}_{typeSymbol.Name}";
    }

    /// <summary>
    /// 是名称
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <param name="typeFullName">类型全名</param>
    /// <returns>名称</returns>
    public static bool IsName(this ITypeSymbol typeSymbol, string typeFullName)
    {
        var isGlobal = typeFullName.IsGlobalName();
        return typeSymbol.Name == typeFullName || typeSymbol.GetFullName(isGlobal) == typeFullName;
    }

    /// <summary>
    /// 获取名称
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <returns>名称</returns>
    public static string GetName(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }

    /// <summary>
    /// 获取全名称
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <param name="global">全局</param>
    /// <returns>名称</returns>
    public static string GetFullName(this ITypeSymbol typeSymbol, bool global = false)
    {
        return global
            ? typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            : typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }

    /// <summary>
    /// 获取全局全名称
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <returns>名称</returns>
    public static string GetGlobalFullName(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    /// <summary>
    /// 继承自基类类型
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <param name="baseTypeFullName">基类全名</param>
    /// <returns>是否继承自</returns>
    public static bool InheritedFrom(this ITypeSymbol typeSymbol, string baseTypeFullName)
    {
        var currentType = typeSymbol;
        var isGlobal = baseTypeFullName.IsGlobalName();
        while (currentType != null)
        {
            var typeName = currentType.GetFullName(isGlobal);
            if (typeName == baseTypeFullName)
                return true;
            typeName = currentType.OriginalDefinition.GetFullName(isGlobal);
            if (typeName == baseTypeFullName)
                return true;
            currentType = currentType.BaseType;
        }
        return false;
    }
}
