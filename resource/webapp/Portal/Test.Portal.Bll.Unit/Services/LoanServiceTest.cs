using FluentAssertions;
using Moq;
using Portal.Bll.Services;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;
using Test.Portal.Core.Extensions;

namespace Test.Portal.Bll.Unit.Services;

public class LoanServiceTest
{
    [Fact]
    public async Task Test_GetAllLoansOverviewAsync_Correct()
    {
        var loanProvider = new Mock<ILoanProvider>();
        var personService = new Mock<IPersonService>();
        var transactionProvider = new Mock<ITransactionProvider>();

        var loanId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var loansWithPersons = new List<LoanWithPersons>
        {
            new()
            {
                LoanDto = new LoanDto
                {
                    Id = loanId
                },
                Persons = new List<PersonDto>()
                {
                    new()
                    {
                        Id = personId
                    }
                }
            }
        };

        var loansLatestStates = new List<LoanLatestState>
        {
            new()
            {
                LoanId = loansWithPersons[0].LoanDto.Id,
                TotalTransacted = 100
            }
        };

        loanProvider.Setup(x => x.GetAllLoansWithPersonsAsync()).ReturnsAsync(loansWithPersons);
        transactionProvider.Setup(x => x.GetAllLoansLatestStates()).ReturnsAsync(loansLatestStates);

        var loanService = new LoanService(loanProvider.Object, personService.Object, transactionProvider.Object, null!);

        var result = (await loanService.GetAllLoansOverviewAsync()).GetValue().ToList();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().LoanDto.Id.Should().Be(loansWithPersons[0].LoanDto.Id);
        result.First().Persons.First().Id.Should().Be(loansWithPersons[0].Persons.First().Id);
        result.First().LoanTotalReturned.Should().Be(loansLatestStates.First().TotalTransacted);
    }

    [Fact]
    public async Task Test_GetAllLoansByPersonIdAsync_Correct()
    {
        var loanProvider = new Mock<ILoanProvider>();
        var personService = new Mock<IPersonService>();
        var transactionProvider = new Mock<ITransactionProvider>();

        var loanId1 = Guid.NewGuid();
        var loanId2 = Guid.NewGuid();
        var correctPersonId = Guid.NewGuid();
        var loansWithPersons = new List<LoanWithPersons>
        {
            new()
            {
                LoanDto = new LoanDto
                {
                    Id = loanId1
                },
                Persons = new List<PersonDto>()
                {
                    new()
                    {
                        Id = correctPersonId
                    }
                }
            }
        };

        var loansLatestStates = new List<LoanLatestState>
        {
            new()
            {
                LoanId = loanId1,
                TotalTransacted = 100
            },
            new()
            {
                LoanId = loanId2,
                TotalTransacted = 200
            }
        };

        loanProvider.Setup(x => x.GetLoansByPersonIdAsync(It.IsAny<Guid>())).ReturnsAsync(loansWithPersons);
        transactionProvider.Setup(x => x.GetAllLoansLatestStates()).ReturnsAsync(loansLatestStates);

        var loanService = new LoanService(loanProvider.Object, personService.Object, transactionProvider.Object, null!);

        var result = (await loanService.GetAllLoansByPersonIdAsync(correctPersonId)).GetValue().ToList();

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().LoanDto.Id.Should().Be(loanId1);
        result.First().Persons.First().Id.Should().Be(correctPersonId);
        result.First().LoanTotalReturned.Should().Be(loansLatestStates.First().TotalTransacted);
    }

    [Fact]
    public async Task Test_ApproveLoanAsync_Correct()
    {
        var loanProvider = new Mock<ILoanProvider>();
        var personService = new Mock<IPersonService>();
        var transactionProvider = new Mock<ITransactionProvider>();

        var loanId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var loansWithPersons = new List<LoanWithPersons>
        {
            new()
            {
                LoanDto = new LoanDto
                {
                    Id = loanId
                },
                Persons = new List<PersonDto>()
                {
                    new()
                    {
                        Id = personId
                    }
                }
            }
        };

        loanProvider.Setup(x => x.GetLoanWithPersonsByIdAsync(It.IsAny<Guid>())).ReturnsAsync(loansWithPersons[0]);
        loanProvider.Setup(x => x.SetLoanToApprovedAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        var loanService = new LoanService(loanProvider.Object, personService.Object, transactionProvider.Object, null!);

        var result = (await loanService.ApproveLoanAsync(loanId)).GetValue();

        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task Test_ApplyForLoanAsync_Correct()
    {
        var personServiceMock = new Mock<IPersonService>();
        var loanProviderMock = new Mock<ILoanProvider>();
        
        var loanApplication = new LoanApplication
        {
            Loan = new LoanBm()
            {
                Interest = 0.1m,
                LoanBaseAmount = 1000,
                Purpose = "House"
            },
            Guids = new List<Guid>
            {
                Guid.NewGuid()
            }
        };

        var personDto = new PersonDto
        {
            Id = loanApplication.Guids.First()
        };

        var mockDateTime = new DateTime(2001, 1, 1);
        var mockDbGeneratedGuid = Guid.NewGuid();
        
        var loanInitialized = new LoanDto();
        loanInitialized.Init(loanApplication.Loan, mockDateTime);
        loanInitialized.Id = mockDbGeneratedGuid;
        
        personServiceMock.Setup(x => x.GetPersonByIdAsync(It.IsAny<Guid>())).ReturnsAsync(personDto);
        loanProviderMock.Setup(x =>
                x.CreateLoanAsync(
                    It.IsAny<LoanDto>(),
                    It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(loanInitialized);

        var loanService = new LoanService(loanProviderMock.Object, personServiceMock.Object, null!, null!);
        
        var result = (await loanService.ApplyForLoanAsync(loanApplication)).GetValue();

        result.Should().NotBeNull();
        result?.Id.Should().Be(mockDbGeneratedGuid);
        result?.StartDatetimeUtc.Should().Be(mockDateTime);
    }
}