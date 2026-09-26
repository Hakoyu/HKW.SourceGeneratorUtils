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
    /// 实现接口
    /// </summary>
    /// <param name="symbol">符号</param>
    /// <param name="interfaceFullName">接口全名</param>
    /// <returns>是否实现</returns>
    public static INamedTypeSymbol? GetInterface(this ITypeSymbol symbol, string interfaceFullName)
    {
        return symbol.Interfaces.FirstOrDefault(i =>
            i.GetFullName() == interfaceFullName
            || i.OriginalDefinition.GetFullName() == interfaceFullName
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
        return symbol.Interfaces.Any(i =>
            i.GetFullName() == interfaceFullName
            || i.OriginalDefinition.GetFullName() == interfaceFullName
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
            if (currentType.OriginalDefinition?.GetFullName() == GeneratorHelper.TaskResultFullName)
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
        return $"{typeSymbol.ContainingNamespace.ToString().Replace('.', '_')}_{typeSymbol.Name}";
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
    /// <returns>名称</returns>
    public static string GetFullName(this ITypeSymbol typeSymbol)
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
        while (currentType != null)
        {
            var typeName = currentType.GetFullName();
            if (typeName == baseTypeFullName)
                return true;
            typeName = currentType.OriginalDefinition.GetFullName();
            if (typeName == baseTypeFullName)
                return true;
            currentType = currentType.BaseType;
        }
        return false;
    }
}
