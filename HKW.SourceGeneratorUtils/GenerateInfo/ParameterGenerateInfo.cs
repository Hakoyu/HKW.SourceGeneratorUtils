using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 参数信息
/// </summary>
public class ParameterGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="type">类型</param>
    /// <param name="name">名称</param>
    /// <param name="attributes">特性</param>
    public ParameterGenerateInfo(
        ITypeSymbol type,
        string name,
        params AttributeGenerateInfo[] attributes
    )
        : this(type.GetFullName(), name, attributes) { }

    /// <inheritdoc/>
    /// <param name="typeName">类型名称</param>
    /// <param name="name">名称</param>
    /// <param name="attributes">特性</param>
    public ParameterGenerateInfo(
        string typeName,
        string name,
        params AttributeGenerateInfo[] attributes
    )
    {
        TypeName = typeName;
        Name = name;
        Attributes = new(attributes);
    }

    /// <summary>
    /// 特性
    /// </summary>
    public List<AttributeGenerateInfo>? Attributes { get; set; }

    /// <summary>
    /// 参数类型
    /// </summary>
    public ParameterGenerateType GenerateType { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 默认值
    /// </summary>
    public string Default { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override string ToString()
    {
        var att = string.Empty;
        if (Attributes is not null && Attributes.Count > 0)
            att = string.Join("", Attributes) + " ";

        var paramType = string.Empty;
        if (GenerateType != ParameterGenerateType.None)
            paramType = GenerateType.ToCode() + " ";

        var def = string.Empty;
        if (string.IsNullOrWhiteSpace(Default) is false)
            def = $" = {Default}";
        return $"{att}{paramType}{TypeName} {Name}{def}";
    }
}

/// <summary>
/// 参数传递类型
/// </summary>
public enum ParameterGenerateType
{
    /// <summary>
    /// 无修饰符
    /// </summary>
    None,

    /// <summary>
    /// 本地传递, 意为扩展方法
    /// </summary>
    This,

    /// <summary>
    /// 按引用传递
    /// </summary>
    Ref,

    /// <summary>
    /// 输出参数
    /// </summary>
    Out,

    /// <summary>
    /// 只读引用参数
    /// </summary>
    In,

    /// <summary>
    /// 可变参数
    /// </summary>
    Params,
}
