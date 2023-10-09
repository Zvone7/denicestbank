using FluentAssertions;
using LanguageExt.Common;
using Moq;
using Portal.Bll.Services;
using Portal.Core.Generation;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;
using Test.Portal.Core.Extensions;

namespace Test.Portal.Bll.Unit.Services;

public class TransactionServiceTest
{
    [Theory]
    [InlineData("A", true, 100, 1000, false, true)]
    [InlineData("B", false, 100, 1000, false, false)]
    [InlineData("C", true, 999, 1000, false, false)]
    [InlineData("D", true, 100, 1000, true, false)]
    public async Task Test_GenerateTransactionsAsync_Correct(
        String testCase,
        Boolean isApproved,
        decimal loanTotalReturned,
        decimal loanTotalAmount,
        Boolean isPastDue,
        Boolean shouldGenerateTransaction)
    {
        var loanServiceMock = new Mock<ILoanService>();
        var randomGeneratorMock = new Mock<IRandomGenerator>();
        var transactionProviderMock = new Mock<ITransactionProvider>();
        var mockedTransactionAmount = 0.1m * loanTotalReturned;
        var loanOverviews = new List<LoanOverview>()
        {
            CreateLoanOverview(isApproved, loanTotalReturned, loanTotalAmount, isPastDue)
        };
        loanServiceMock.Setup(
                x => x.GetAllLoansOverviewAsync())
            .ReturnsAsync(loanOverviews);
        randomGeneratorMock.Setup(x =>
                x.GenerateRandomPaymentAmount(
                    It.IsAny<decimal>()))
            .Returns(mockedTransactionAmount);

        transactionProviderMock.Setup(x =>
                x.InsertTransactionAsync(It.IsAny<TransactDto>()))
            .ReturnsAsync((TransactDto transaction) => transaction);

        var transactionService = new TransactionService(
            transactionProviderMock.Object,
            loanServiceMock.Object,
            randomGeneratorMock.Object,
            null!);

        var result = (await transactionService.GenerateTransactionsAsync()).GetValue().ToList();

        if (shouldGenerateTransaction)
        {
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            result.First().PersonId.Should().Be(loanOverviews.First().Persons.First().Id);
            result.First().Amount.Should().Be(mockedTransactionAmount);
        }
        else
        {
            result.Should().BeEmpty();
        }
    }

    [Theory]
    [InlineData("A", 1, 10, 50, true)]
    [InlineData("B", 42, 10, 30, false)]
    public async Task Test_GetLatestPayments(
        String testCase,
        int pageIndex,
        int pageSize,
        Int32 availablePaymentCount,
        Boolean shouldHaveResult)
    {
        var transactionProviderMock = new Mock<ITransactionProvider>();
        transactionProviderMock.Setup(x =>
                x.GetAllEnrichedTransactions())
            .ReturnsAsync(new Result<IEnumerable<PaymentVm>>(GenerateMockPayments(availablePaymentCount)));

        var transactionService = new TransactionService(
            transactionProviderMock.Object,
            null!,
            null!,
            null!);

        var result = (await transactionService.GetLatestPaymentsAsync(pageIndex, pageSize)).GetValue().ToList();

        if (shouldHaveResult)
        {
            result.Should().NotBeEmpty();
        }
        else
        {
            result.Should().BeEmpty();
        }
    }

    private LoanOverview CreateLoanOverview(
        Boolean isApproved,
        decimal loanTotalReturned,
        decimal loanTotalAmount,
        Boolean isPastDue
    )
    {
        return new LoanOverview(
            new LoanWithPersons()
            {
                LoanDto = new LoanDto()
                {
                    Id = Guid.NewGuid(),
                    IsApproved = isApproved,
                    LoanTotalAmount = loanTotalAmount,
                    StartDatetimeUtc = isPastDue ? DateTime.UtcNow.AddDays(-20) : DateTime.UtcNow.AddDays(0),
                    DurationInDays = 10
                },
                Persons = new List<PersonDto>()
                {
                    new()
                    {
                        Id = Guid.NewGuid()
                    }
                }
            },
            loanTotalReturned);
    }

    private IEnumerable<PaymentVm> GenerateMockPayments(Int32 amountOfPayments)
    {
        return Enumerable.Range(0, amountOfPayments)
            .Select(_ => new PaymentVm());
    }
}