using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 常用数据
/// </summary>
public static class SourceGeneratorHelper
{
    /// <summary>
    /// 异步类型全名
    /// </summary>
    public const string TaskTypeFullName = "global::System.Threading.Tasks.Task";

    /// <summary>
    /// 异步结果类型全名
    /// </summary>
    public const string TaskResultFullName = "global::System.Threading.Tasks.Task<int>";

    /// <summary>
    /// 生成代码特性
    /// </summary>
    public static string GeneratedCodeAttribute { get; private set; }

    /// <summary>
    /// 从不在调试菜单显示标签
    /// </summary>
    public const string DebuggerBrowsableNeverAttribute =
        "[global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]";

#pragma warning disable S2223
    internal static Compilation Compilation = null!;
#pragma warning restore S2223

    /// <summary>
    /// Void类型
    /// </summary>
    public static ITypeSymbol TypeVoid { get; private set; } = null!;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="compilation">编译</param>
    public static void Initialize(Compilation compilation)
    {
        GeneratedCodeAttribute =
            $"[global::System.CodeDom.Compiler.GeneratedCode(\"{System.Reflection.Assembly.GetCallingAssembly().GetName().Name}\",\"{System.Reflection.Assembly.GetCallingAssembly().GetName().Version}\")]";
        var generatedCodeAttribute = new AttributeGenerateInfo(GeneratedCodeAttribute);
        ObjectGenerateInfo.DefaultAttributes = [generatedCodeAttribute];
        MethodGenerateInfo.DefaultAttributes = [generatedCodeAttribute];
        PropertyGenerateInfo.DefaultAttributes = [generatedCodeAttribute];
        FieldGenerateInfo.DefaultAttributes =
        [
            generatedCodeAttribute,
            new(DebuggerBrowsableNeverAttribute),
        ];
        Compilation = compilation;
        TypeVoid = Compilation.GetSpecialType(SpecialType.System_Void);
    }
}
