using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class SourceGeneratorExtensions
{
    /// <summary>
    /// 从当前类中查找成员
    /// </summary>
    /// <typeparam name="TMemberType">成员类型</typeparam>
    /// <param name="symbol">当前类</param>
    /// <param name="memberName">成员名称</param>
    /// <returns>成员</returns>
    public static TMemberType? FindMember<TMemberType>(
        this INamedTypeSymbol symbol,
        string memberName
    )
        where TMemberType : ISymbol
    {
        var temp = symbol;
        return temp.GetMembers(memberName).OfType<TMemberType>().FirstOrDefault();
    }

    /// <summary>
    /// 从当前类以及父类中查找成员
    /// </summary>
    /// <typeparam name="TMemberType">成员类型</typeparam>
    /// <param name="symbol">当前类</param>
    /// <param name="memberName">成员名称</param>
    /// <returns>成员</returns>
    public static TMemberType? FindMemberIncludingBaseTypes<TMemberType>(
        this INamedTypeSymbol symbol,
        string memberName
    )
        where TMemberType : ISymbol
    {
        var temp = symbol;
        var member = temp.GetMembers(memberName).OfType<TMemberType>().FirstOrDefault();
        while (member is null && temp.BaseType is not null)
        {
            temp = temp.BaseType;
            member = temp.GetMembers(memberName).OfType<TMemberType>().FirstOrDefault();
        }
        return member;
    }

    /// <summary>
    /// 获取可访问性
    /// </summary>
    /// <param name="syntaxes"></param>
    /// <returns></returns>
    public static SyntaxToken GetAccessibility(this SyntaxTokenList syntaxes)
    {
        var accessibility = syntaxes.FirstOrDefault(m =>
            m.IsKind(SyntaxKind.PublicKeyword)
            || m.IsKind(SyntaxKind.InternalKeyword)
            || m.IsKind(SyntaxKind.PrivateKeyword)
        );
        return accessibility;
    }

    /// <summary>
    /// 尝试添加
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    /// <param name="dictionary">字典</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否添加成功</returns>
    public static bool TryAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value
    )
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out _))
            return false;

        dictionary.Add(key, value);
        return true;
    }

    /// <summary>
    /// 转换为代码
    /// </summary>
    /// <param name="accessibility">可访问性</param>
    /// <returns>代码</returns>
    public static string ToCode(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.Internal and Accessibility.Friend => "internal",
            Accessibility.Public => "public",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal and Accessibility.ProtectedAndFriend =>
                "protected internal",
            Accessibility.ProtectedOrInternal and Accessibility.ProtectedOrFriend =>
                "protected internal",
            _ => "",
        };
    }

    /// <summary>
    /// 转换为代码
    /// </summary>
    /// <param name="generateType">生成类型</param>
    /// <returns>代码</returns>
    public static string ToCode(this MethodGenerateType generateType)
    {
        return generateType switch
        {
            MethodGenerateType.Static => "static",
            MethodGenerateType.Partial => "partial",
            MethodGenerateType.Override => "override",
            MethodGenerateType.Abstract => "abstract",
            MethodGenerateType.Virtual => "virtual",
            _ => "",
        };
    }

    /// <summary>
    /// 转换为代码
    /// </summary>
    /// <param name="generateType">参数类型</param>
    /// <returns>代码</returns>
    public static string ToCode(this ParameterGenerateType generateType)
    {
        return generateType switch
        {
            ParameterGenerateType.Ref => "ref",
            ParameterGenerateType.Out => "out",
            ParameterGenerateType.In => "in",
            _ => "",
        };
    }

    /// <summary>
    /// 转换为代码
    /// </summary>
    /// <param name="generateType">生成类型</param>
    /// <returns>代码</returns>
    public static string ToCode(this PropertyMethodGenerateType generateType)
    {
        return generateType switch
        {
            PropertyMethodGenerateType.Get => "get",
            PropertyMethodGenerateType.Set => "set",
            _ => throw new NotSupportedException(),
        };
    }

    /// <summary>
    /// 转换为代码
    /// </summary>
    /// <param name="generateType">生成类型</param>
    /// <returns>代码</returns>
    public static string ToCode(this ObjectGenerateType generateType)
    {
        return generateType switch
        {
            ObjectGenerateType.Class => "class",
            ObjectGenerateType.StaticClass => "static class",
            ObjectGenerateType.PartialClass => "partial class",
            ObjectGenerateType.StaticPartialClass => "static partial class",
            ObjectGenerateType.AbstractClass => "abstract class",
            ObjectGenerateType.AbstractPartialClass => "abstract partial class",
            ObjectGenerateType.SealedClass => "sealed class",
            ObjectGenerateType.SealedPartialClass => "sealed partial class",
            ObjectGenerateType.Struct => "struct",
            ObjectGenerateType.PartialStruct => "partial struct",
            ObjectGenerateType.ReadOnlyStruct => "readonly struct",
            ObjectGenerateType.ReadOnlyPartialStruct => "readonly partial struct",
            ObjectGenerateType.RefStruct => "ref struct",
            ObjectGenerateType.PartialRefStruct => "partial ref struct",
            ObjectGenerateType.ReadOnlyRefStruct => "readonly ref struct",
            ObjectGenerateType.ReadOnlyPartialRefStruct => "readonly partial ref struct",
            ObjectGenerateType.Record => "record",
            ObjectGenerateType.PartialRecord => "partial record",
            ObjectGenerateType.AbstractRecord => "abstract record",
            ObjectGenerateType.AbstractPartialRecord => "abstract partial record",
            ObjectGenerateType.SealedRecord => "sealed record",
            ObjectGenerateType.SealedPartialRecord => "sealed partial record",
            ObjectGenerateType.RecordStruct => "record struct",
            ObjectGenerateType.PartialRecordStruct => "partial record struct",
            ObjectGenerateType.ReadOnlyRecordStruct => "readonly record struct",
            ObjectGenerateType.ReadOnlyPartialRecordStruct => "readonly partial record struct",
            _ => "",
        };
    }
}
