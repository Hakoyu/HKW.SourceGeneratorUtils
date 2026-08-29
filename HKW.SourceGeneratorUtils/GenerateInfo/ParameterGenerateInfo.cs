using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 参数信息
/// </summary>
public class ParameterGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    public ParameterGenerateInfo(string name, ITypeSymbol type)
    {
        Name = name;
        Type = type;
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="attributes">特性</param>
    public ParameterGenerateInfo(
        string name,
        ITypeSymbol type,
        params AttributeGenerateInfo[] attributes
    )
    {
        Name = name;
        Type = type;
        Attributes = new(attributes);
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型名称</param>
    public ParameterGenerateInfo(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型名称</param>
    /// <param name="attributes">特性</param>
    public ParameterGenerateInfo(
        string name,
        string typeName,
        params AttributeGenerateInfo[] attributes
    )
    {
        Name = name;
        TypeName = typeName;
        Attributes = new(attributes);
    }

    /// <summary>
    /// 特性
    /// </summary>
    public List<AttributeGenerateInfo>? Attributes { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public ITypeSymbol? Type { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

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

        var def = string.Empty;
        if (string.IsNullOrWhiteSpace(Default) is false)
            def = $" = {Default}";
        var typeName = Type is null ? TypeName : Type.GetName();
        return $"{att}{typeName} {Name}{def}";
    }
}
