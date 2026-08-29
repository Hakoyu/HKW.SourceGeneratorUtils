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
    internal static Compilation Compilation = null!;

    /// <summary>
    /// Void类型
    /// </summary>
    public static ITypeSymbol TypeVoid { get; private set; } = null!;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="compilation"></param>
    public static void Initialize(Compilation compilation)
    {
        Compilation = compilation;
        TypeVoid = Compilation.GetSpecialType(SpecialType.System_Void);
    }

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
    /// 到字符串
    /// </summary>
    /// <param name="accessibility"></param>
    /// <returns>字符串</returns>
    public static string ToStr(this Accessibility accessibility)
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
    /// 到字符串
    /// </summary>
    /// <param name="methodType"></param>
    /// <returns></returns>
    public static string ToStr(this MethodGenerateType methodType)
    {
        return methodType switch
        {
            MethodGenerateType.Static => "static",
            MethodGenerateType.Partial => "partial",
            MethodGenerateType.Override => "override",
            MethodGenerateType.Abstract => "abstract",
            MethodGenerateType.Virtual => "virtual",
            _ => "",
        };
    }
}
