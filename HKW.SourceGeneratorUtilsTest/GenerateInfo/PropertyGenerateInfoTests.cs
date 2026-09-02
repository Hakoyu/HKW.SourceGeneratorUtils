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
        var propertyInfo = new PropertyGenerateInfo(
            "Count",
            "int",
            new PropertyGetMethodGenerateInfo(";")
        )
        {
            Accessibility = Accessibility.Public,
            SetMethod = new PropertySetMethodGenerateInfo(";"),
            Default = "7",
        };

        var result = TestHelper.PropertyCompilation<int>(propertyInfo);

        Assert.AreEqual(7, result);
    }

    [TestMethod]
    public void PropertyWithPrivateSetter()
    {
        var propertyInfo = new PropertyGenerateInfo(
            "Count",
            "int",
            new PropertyGetMethodGenerateInfo(";")
        )
        {
            Accessibility = Accessibility.Public,
            SetMethod = new PropertySetMethodGenerateInfo(";")
            {
                Accessibility = Accessibility.Private,
            },
        };

        var result = TestHelper.PropertyCompilation<int>(propertyInfo, 42);

        Assert.AreEqual(42, result);
    }

    [TestMethod]
    public void ReferenceTypeProperty()
    {
        var propertyInfo = new PropertyGenerateInfo(
            "Values",
            "System.Collections.Generic.List<int>",
            new PropertyGetMethodGenerateInfo(";")
        )
        {
            Accessibility = Accessibility.Internal,
            SetMethod = new PropertySetMethodGenerateInfo(";"),
            Default = "new System.Collections.Generic.List<int> { 1, 2, 3 }",
        };

        var result = TestHelper.PropertyCompilation<List<int>>(propertyInfo);

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }
}
#pragma warning restore S6562
