namespace TestLib.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.That(Class1.Add(2, 2), Is.EqualTo(4));
    }
}