using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class IMethodSymbolExtensions
{
    /// <summary>
    /// 构建调用语句
    /// </summary>
    /// <param name="methodSymbol">方法</param>
    /// <param name="source">源</param>
    /// <returns>调用语句</returns>
    /// <exception cref="ArgumentException">方法参数大于 0 </exception>
    public static string BuildInvocationStatement(
        this IMethodSymbol methodSymbol,
        string source = ""
    )
    {
        if (methodSymbol.Parameters.Length > 0)
        {
            throw new ArgumentException(
                $"Method '{methodSymbol.Name}' must have 0 parameters, but actually has {methodSymbol.Parameters.Length}.",
                nameof(methodSymbol)
            );
        }
        var isTask = methodSymbol.ReturnType.InheritedFrom(GeneratorHelper.TaskTypeFullName);
        return $"{(string.IsNullOrWhiteSpace(source) ? "" : source + ".")}{(isTask ? "await " : "")}{methodSymbol.Name}();";
    }

    /// <summary>
    /// 构建调用语句
    /// </summary>
    /// <param name="methodSymbol">方法</param>
    /// <param name="source">源</param>
    /// <param name="parameters">参数</param>
    /// <returns>调用语句</returns>
    /// <exception cref="ArgumentException">方法参数数量不等于输入参数数量</exception>
    public static string BuildInvocationStatement(
        this IMethodSymbol methodSymbol,
        string source = "",
        params string[] parameters
    )
    {
        if (methodSymbol.Parameters.Length != parameters.Length)
        {
            throw new ArgumentException(
                $"Method '{methodSymbol.Name}' expects {methodSymbol.Parameters.Length} parameters, but received {parameters.Length}.",
                nameof(parameters)
            );
        }
        var isTask = methodSymbol.ReturnType.InheritedFrom(GeneratorHelper.TaskTypeFullName);
        return $"{(string.IsNullOrWhiteSpace(source) ? "" : source + ".")}{(isTask ? "await " : "")}{methodSymbol.Name}({string.Join(", ", parameters)});";
    }
}
