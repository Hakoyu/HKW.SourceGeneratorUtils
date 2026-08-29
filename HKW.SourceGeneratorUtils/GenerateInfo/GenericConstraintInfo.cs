namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 泛型约束信息
/// </summary>
public class GenericConstraintInfo
{
    /// <inheritdoc/>
    /// <param name="genericName">泛型名称</param>
    /// <param name="constraint">约束</param>
    public GenericConstraintInfo(string genericName, string constraint)
    {
        GenericName = genericName;
        Constraint = constraint;
    }

    /// <summary>
    /// 泛型名称
    /// </summary>
    public string GenericName { get; set; }

    /// <summary>
    /// 约束
    /// </summary>
    public string Constraint { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"where {GenericName} : {Constraint}";
    }
}
