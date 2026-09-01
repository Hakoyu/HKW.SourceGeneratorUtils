using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 常用数据
/// </summary>
public static class GeneratorHelper
{
    /// <summary>
    /// Void类型
    /// </summary>
    public static ITypeSymbol TypeVoid { get; private set; } = null!;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="productionContext">生产环境</param>
    /// <param name="compilation">编译</param>
    public static void Initialize(
        SourceProductionContext productionContext,
        Compilation compilation
    )
    {
        ProductionContext = productionContext;
        Compilation = compilation;

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

        TypeVoid = Compilation.GetSpecialType(SpecialType.System_Void);
    }

    /// <summary>
    /// 生产环境
    /// </summary>
    public static SourceProductionContext ProductionContext { get; private set; }

    /// <summary>
    /// 编译
    /// </summary>
    public static Compilation Compilation { get; private set; } = null!;

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
    public static string GeneratedCodeAttribute { get; private set; } = null!;

    /// <summary>
    /// 从不在调试菜单显示标签
    /// </summary>
    public const string DebuggerBrowsableNeverAttribute =
        "[global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]";
}
