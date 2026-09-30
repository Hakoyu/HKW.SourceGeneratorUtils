using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 常用数据
/// </summary>
public static class GeneratorHelper
{
    /// <summary>
    /// 全局前缀
    /// </summary>
    public const string GlobalPrefix = "global::";

    /// <summary>
    /// 程序集
    /// </summary>
    public static Assembly Assembly { get; private set; } = null!;

    /// <summary>
    /// 程序集名称
    /// </summary>
    public static AssemblyName AssemblyName { get; private set; } = null!;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Initialize()
    {
        Assembly = System.Reflection.Assembly.GetCallingAssembly();
        AssemblyName = Assembly.GetName();
        GeneratedCodeAttributeName =
            $"[global::System.CodeDom.Compiler.GeneratedCode(\"{AssemblyName.Name}\",\"{AssemblyName.Version}\")]";
        GeneratedCodeAttribute = new(GeneratedCodeAttributeName);
        DebuggerBrowsableNeverAttribute = new(DebuggerBrowsableNeverAttributeName);
        ObjectGenerateInfo.DefaultAttributes = [GeneratedCodeAttribute];
        MethodGenerateInfo.DefaultAttributes = [GeneratedCodeAttribute];
        PropertyGenerateInfo.DefaultAttributes = [GeneratedCodeAttribute];
        FieldGenerateInfo.DefaultAttributes =
        [
            GeneratedCodeAttribute,
            DebuggerBrowsableNeverAttribute,
        ];
    }

    /// <summary>
    /// 异步类型全名
    /// </summary>
    public const string TaskTypeFullName = "global::System.Threading.Tasks.Task";

    /// <summary>
    /// 异步结果类型全名
    /// </summary>
    public const string TaskResultFullName = "global::System.Threading.Tasks.Task<TResult>";

    /// <summary>
    /// 生成代码特性名称
    /// </summary>
    public static string GeneratedCodeAttributeName { get; private set; } = null!;

    /// <summary>
    /// 生成代码特性
    /// </summary>
    public static AttributeGenerateInfo GeneratedCodeAttribute { get; private set; } = null!;

    /// <summary>
    /// 从不在调试菜单显示标签特性名称
    /// </summary>
    public const string DebuggerBrowsableNeverAttributeName =
        "[global::System.Diagnostics.DebuggerBrowsable(global::System.Diagnostics.DebuggerBrowsableState.Never)]";

    /// <summary>
    /// 从不在调试菜单显示标签特性
    /// </summary>
    public static AttributeGenerateInfo DebuggerBrowsableNeverAttribute { get; private set; } =
        null!;

    /// <summary>
    /// 对象名称
    /// </summary>
    public const string ObjectName = "object";

    /// <summary>
    /// 字符串名称
    /// </summary>
    public const string StringName = "string";

    /// <summary>
    /// 布尔名称
    /// </summary>
    public const string BoolName = "bool";

    /// <summary>
    /// 字节名称
    /// </summary>
    public const string ByteName = "byte";

    /// <summary>
    /// 有符号字节名称
    /// </summary>
    public const string SByteName = "sbyte";

    /// <summary>
    /// 短整型名称
    /// </summary>
    public const string ShortName = "short";

    /// <summary>
    /// 无符号短整型名称
    /// </summary>
    public const string UShortName = "ushort";

    /// <summary>
    /// 整型名称
    /// </summary>
    public const string IntName = "int";

    /// <summary>
    /// 无符号整型名称
    /// </summary>
    public const string UIntName = "uint";

    /// <summary>
    /// 长整型名称
    /// </summary>
    public const string LongName = "long";

    /// <summary>
    /// 无符号长整型名称
    /// </summary>
    public const string ULongName = "ulong";

    /// <summary>
    /// 本机整型名称
    /// </summary>
    public const string NIntName = "nint";

    /// <summary>
    /// 无符号本机整型名称
    /// </summary>
    public const string NUIntName = "nuint";

    /// <summary>
    /// 字符名称
    /// </summary>
    public const string CharName = "char";

    /// <summary>
    /// 单精度浮点名称
    /// </summary>
    public const string FloatName = "float";

    /// <summary>
    /// 双精度浮点名称
    /// </summary>
    public const string DoubleName = "double";

    /// <summary>
    /// 十进制名称
    /// </summary>
    public const string DecimalName = "decimal";

    /// <summary>
    /// 空名称
    /// </summary>
    public const string VoidName = "void";
}
