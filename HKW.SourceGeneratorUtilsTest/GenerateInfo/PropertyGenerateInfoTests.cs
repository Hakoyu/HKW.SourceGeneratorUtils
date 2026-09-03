using HKW.SourceGeneratorUtils;
using Microsoft.CodeAnalysis;

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
}
#pragma warning restore S6562
