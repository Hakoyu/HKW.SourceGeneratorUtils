using System.Reflection;
using HKW.SourceGeneratorUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HKW.SourceGeneratorUtilsTest;

#pragma warning disable S6562
[TestClass]
public sealed class PropertyGenerateInfoTests
{
    [TestMethod]
    public void AutoPropertyWithDefaultValue()
    {
        var propertyInfo = new PropertyGenerateInfo("int", "Count", new(";"))
        {
            Accessibility = Accessibility.Public,
            SetMethod = new(";"),
            Default = "7",
        };

        var result = TestHelper.PropertyCompilation<int>(propertyInfo);

        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void PropertyWithPrivateSetter()
    {
        var propertyInfo = new PropertyGenerateInfo("int", "Count", new(";"))
        {
            Accessibility = Accessibility.Public,
            SetMethod = new(";") { Accessibility = Accessibility.Private },
        };

        var result = TestHelper.PropertyCompilation<int>(propertyInfo, 42);

        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void ReferenceTypeProperty()
    {
        var propertyInfo = new PropertyGenerateInfo(
            "System.Collections.Generic.List<int>",
            "Values",
            new(";")
        )
        {
            Accessibility = Accessibility.Internal,
            SetMethod = new(";"),
            Default = "new System.Collections.Generic.List<int> { 1, 2, 3 }",
        };

        var result = TestHelper.PropertyCompilation<List<int>>(propertyInfo);

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    public void IsStatic()
    {
        var propertyInfo = new PropertyGenerateInfo("int", "Value", new())
        {
            Accessibility = Accessibility.Internal,
            Default = "6",
            IsStatic = true,
        };

        var result = TestHelper.PropertyCompilation<int>(propertyInfo);

        Assert.AreEqual(6, result);
    }

    [TestMethod]
    public void PartialProperty()
    {
        var propertyInfo = new PropertyGenerateInfo("int", "Count", new(";"))
        {
            Accessibility = Accessibility.Public,
            GenerateType = PropertyGenerateType.Partial,
            SetMethod = new(";"),
        };

        var source = $$"""
            public partial class Program
            {
                {{propertyInfo}}
            }

            public partial class Program
            {
                private int _count;

                public partial int Count
                {
                    get => _count;
                    set => _count = value;
                }
            }
            """;

        var syntaxTree = CSharpSyntaxTree.ParseText(
            source,
            cancellationToken: CancellationToken.None
        );

        var compilation = CSharpCompilation.Create(
            nameof(PartialProperty),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var error = compilation
            .GetDiagnostics(CancellationToken.None)
            .FirstOrDefault(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        DiagnosticException.ThrowIfError(error);

        using var ms = new MemoryStream();
        compilation.Emit(ms, cancellationToken: CancellationToken.None);
        ms.Seek(0, SeekOrigin.Begin);

        var assembly = Assembly.Load(ms.ToArray());
        var typeInfo = assembly.DefinedTypes.First();
        var property = typeInfo.DeclaredProperties.First();
        var field = typeInfo.DeclaredFields.First();

        TestHelper.GenerateInfoCheck(propertyInfo, property);

        var obj = assembly.CreateInstance(typeInfo.FullName!);
        property.SetValue(obj, 7);
        var result = (int)property.GetValue(obj)!;
        Assert.AreEqual(7, result);
        field.SetValue(obj, 8);
        result = (int)property.GetValue(obj)!;
        Assert.AreEqual(8, result);
    }
}
#pragma warning restore S6562
