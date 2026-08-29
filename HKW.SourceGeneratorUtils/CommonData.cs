using System;
using System.Collections.Generic;
using System.Text;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 常用数据
/// </summary>
public static class CommonData
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
    public static string GeneratedCodeAttribute { get; } =
        $"[global::System.CodeDom.Compiler.GeneratedCode(\"{System.Reflection.Assembly.GetCallingAssembly().GetName().Name}\",\"{System.Reflection.Assembly.GetCallingAssembly().GetName().Version}\")]";

    /// <summary>
    /// 从不在调试菜单显示标签
    /// </summary>
    public const string DebuggerBrowsableNeverAttribute =
        "[global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]";
}
