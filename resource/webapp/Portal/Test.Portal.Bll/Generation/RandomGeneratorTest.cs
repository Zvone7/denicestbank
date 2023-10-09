using FluentAssertions;
using Portal.Bll.Generation;

namespace Test.Portal.Bll.Generation;

public class RandomGeneratorTest
{
    [Fact]
    public void TestSsnIsGeneratedInCorrectFormat()
    {
        var rn = new RandomGenerator();
        var ssn = rn.GenerateSsn();
        ssn.Should().MatchRegex(@"^\d{3}-\d{2}-\d{4}$");
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(2000)]
    [InlineData(3000)]
    [InlineData(4000)]
    [InlineData(5000)]
    [InlineData(6000)]
    [InlineData(7000)]
    [InlineData(8000)]
    [InlineData(9000)]
    [InlineData(10000)]
    public void TestRandomPaymentAmountIsGenerated(decimal totalAmount)
    {
        var rn = new RandomGenerator();
        var amount = rn.GenerateRandomPaymentAmount(totalAmount);
        amount.Should().BeGreaterThan(0);
        amount.Should().BeLessThan(totalAmount/100);
    }

}