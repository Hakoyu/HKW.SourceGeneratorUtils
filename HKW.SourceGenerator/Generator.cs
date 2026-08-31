// Source from https://github.com/SparkyTD/ReactiveCommand.SourceGenerator

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using HKW.SourceGeneratorUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HKW.SourceGenerator;

[Generator]
internal partial class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var compilation = context.CompilationProvider.Select(static (c, _) => c);

        context.RegisterSourceOutput(
            compilation,
            static (spc, compilation) =>
            {
                SourceGeneratorHelper.Initialize(compilation);
                var assemblyInfo = new AssemblyInfo(spc, compilation);
                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    ParseSyntaxTree(assemblyInfo, syntaxTree);
                }
            }
        );
    }

    private static void ParseSyntaxTree(AssemblyInfo assemblyInfo, SyntaxTree syntaxTree)
    {
        var semanticModel = assemblyInfo.Compilation.GetSemanticModel(syntaxTree);
        var syntaxTreeInfo = new SyntaxTreeInfo(syntaxTree, semanticModel);
        var declaredClasses = syntaxTree
            .GetRoot()
            .DescendantNodesAndSelf()
            .OfType<ClassDeclarationSyntax>();
        foreach (var declaredClass in declaredClasses)
        {
            var classSymbol = (INamedTypeSymbol)
                ModelExtensions.GetDeclaredSymbol(syntaxTreeInfo.SemanticModel, declaredClass);
            if (
                classSymbol
                    .GetAttributes()
                    .FirstOrDefault(x =>
                        x.AttributeClass.ToString() == typeof(SourceGeneratorTestAttribute).FullName
                    )
                is not AttributeData attributeData
            )
                continue;
            var stringStream = new StringWriter();
            var _writer = new IndentedTextWriter(stringStream);
            _writer.Write(
                $$"""
                using HKW.SourceGenerator;
                namespace HKW.SourceGeneratorDemo;
                partial class {{classSymbol.Name}}
                {

                """
            );
            var infos = new List<MethodGenerateInfo>();
            foreach (var property in classSymbol.GetMembers().OfType<IPropertySymbol>())
            {
                var method = property.GetGetMethodStr();
                if (method is not null)
                {
                    var info = new MethodGenerateInfo($"{property.Name}Func", property.Type, method)
                    {
                        Accessibility = Accessibility.Public,
                        Comment = """
                            /// aaaa
                            /// bbb
                            /// ccc
                            """,
                    };
                    infos.Add(info);
                }
            }
            _writer.Indent++;
            foreach (var data in infos)
            {
                data.WriteTo(_writer);
            }
            _writer.Indent--;
            _writer.WriteLine("}");
            assemblyInfo.ProductionContext.AddSource(
                $"TestSourceGenerator_{classSymbol.Name}.g.cs",
                stringStream.ToString()
            );
            var dic = new AttributeParamDictionary(attributeData);
            Debug.WriteLine(dic);
        }
    }

    //private static ClassInfo? ClassValidator(
    //    AssemblyInfo assemblyInfo,
    //    SyntaxTreeInfo syntaxTreeInfo,
    //    ClassDeclarationSyntax declaredClass
    //)
    //{
    //    var classSymbol = (INamedTypeSymbol)
    //        ModelExtensions.GetDeclaredSymbol(syntaxTreeInfo.SemanticModel, declaredClass)!;
    //    if (
    //        classSymbol.AllInterfaces.Any(i => i.ToString() == TypeFullNames.IReactiveObject)
    //        is false
    //    )
    //        return null; // 如果没有实现IReactiveObject接口,则跳过

    //    // 如果不是分布类型,则触发异常
    //    if (declaredClass.Modifiers.Any(SyntaxKind.PartialKeyword) is false)
    //    {
    //        var diagnostic = Diagnostic.Create(
    //            Descriptors.NotPartialClass,
    //            classSymbol.Locations[0]
    //        );
    //        assemblyInfo.ProductionContext.ReportDiagnostic(diagnostic);
    //        return null;
    //    }
    //    var classNamespace = classSymbol.ContainingNamespace.ToString();
    //    var typeName = declaredClass.Identifier.ValueText;
    //    var usings = ((CompilationUnitSyntax)syntaxTreeInfo.SyntaxTree.GetRoot()).Usings;
    //    var classInfo = new ClassInfo
    //    {
    //        Name = typeName,
    //        Namespace = classNamespace,
    //        Usings = usings,
    //        DeclarationSyntax = declaredClass,
    //    };

    //    // 如果实现了ReactiveObjectX,则标记
    //    if (classSymbol.InheritedFrom(TypeFullNames.ReactiveObjectX))
    //        classInfo.IsReactiveObjectX = true;

    //    // 分析所有成员
    //    foreach (var member in declaredClass.Members)
    //    {
    //        if (member is MethodDeclarationSyntax methodSyntax)
    //        {
    //            methodSyntax.GetLocation();
    //            var methodSymbol = (IMethodSymbol)
    //                ModelExtensions.GetDeclaredSymbol(syntaxTreeInfo.SemanticModel, methodSyntax)!;
    //            classInfo.MethodSymbols.Add(new(methodSyntax, methodSymbol));
    //        }
    //        else if (member is PropertyDeclarationSyntax propertySyntax)
    //        {
    //            var propertySymbol = (IPropertySymbol)
    //                ModelExtensions.GetDeclaredSymbol(
    //                    syntaxTreeInfo.SemanticModel,
    //                    propertySyntax
    //                )!;
    //            classInfo.PropertySymbols.Add(new(propertySyntax, propertySymbol));
    //        }
    //    }
    //    return classInfo;
    //}
}

public class SourceGeneratorTestAttribute : Attribute
{
    public SourceGeneratorTestAttribute() { }

    public SourceGeneratorTestAttribute(string s1)
    {
        S1 = s1;
    }

    public SourceGeneratorTestAttribute(string s1, string s2)
    {
        S1 = s1;
        S2 = s2;
    }

    public SourceGeneratorTestAttribute(string s1, string s2, string s3)
    {
        S1 = s1;
        S2 = s2;
        S3 = s3;
    }

    public SourceGeneratorTestAttribute(params int[] ints)
    {
        Ints = ints;
    }

    public string S1 { get; set; } = string.Empty;
    public string S2 { get; set; } = string.Empty;
    public string S3 { get; set; } = string.Empty;

    public int[] Ints { get; set; } = [];
}
