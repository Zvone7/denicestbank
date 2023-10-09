using FluentAssertions;
using Portal.Core.Extensions;
using Portal.Models;

namespace Test.Portal.Core.Extensions;

public class TestLoanExtensions
{
    [Theory]
    [InlineData("A", 4, 3, false)]
    [InlineData("B", 4, 4, true)]
    [InlineData("C", 4, 5, true)]
    public void Test_IsPastDue(
        String testCase,
        Int32 durationInDays,
        Int32 ageInDays,
        Boolean expectedPastDue
    )
    {
        var dateTime = DateTime.UtcNow;
        var startDate = dateTime.AddDays(-ageInDays);
        var todayDate = dateTime;
        var loan = new LoanDto()
        {
            StartDatetimeUtc = startDate,
            DurationInDays = durationInDays
        };
        if (expectedPastDue)
            loan.IsPastDue(todayDate).Should().BeTrue();
        else
            loan.IsPastDue(todayDate).Should().BeFalse();
    }
    [Theory]
    [InlineData("A", 89, 100 ,0.9, true)]
    [InlineData("B", 90, 100,0.9, false)]
    [InlineData("C", 91, 100, 0.9, false)]
    public void Test_ShouldAcceptMorePayments(
        String testCase,
        decimal totalReturned,
        decimal totalAmount,
        decimal paidOffLimitPercent,
        Boolean expectedShouldAcceptMorePayments
    )
    {
        var loan = new LoanDto()
        {
            LoanTotalAmount = totalAmount
        };
        if (expectedShouldAcceptMorePayments)
            loan.ShouldAcceptMorePayments(totalReturned, paidOffLimitPercent).Should().BeTrue();
        else
            loan.ShouldAcceptMorePayments(totalReturned,paidOffLimitPercent).Should().BeFalse();
    }
}