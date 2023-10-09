using FluentAssertions;
using Moq;
using Portal.Bll.Services;
using Portal.Core.Generation;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;

namespace Test.Portal.Bll.Unit.Services;

public class PersonServiceTest
{
    [Fact]
    public async Task Test_GetPersonByIdAsync_Correct()
    {
        var personProviderMock = new Mock<IPersonProvider>();
        var personId = Guid.NewGuid();
        var personDto = new PersonDto
        {
            Id = personId
        };
        personProviderMock.Setup(x => x.GetPersonByIdAsync(It.IsAny<Guid>())).ReturnsAsync(personDto);

        var personService = new PersonService(personProviderMock.Object, null!, null!);

        var result = await personService.GetPersonByIdAsync(personId);

        result.Should().NotBeNull();
        result?.Id.Should().Be(personId);
    }

    [Fact]
    public async Task Test_TryCreatePersonAsync_When_ExistingPerson()
    {
        var personProviderMock = new Mock<IPersonProvider>();
        var loggerMock = new TestLogger<IPersonService>();
        var personId = Guid.NewGuid();
        var personDto = new PersonDto
        {
            Id = personId,
        };
        var personAadInfo = new PersonAadInfo()
        {
            Id = personId,
        };
        personProviderMock
            .Setup(x =>
                x.GetPersonByIdAsync(It.IsAny<Guid>()))!
            .ReturnsAsync(personDto);

        var personService = new PersonService(personProviderMock.Object, null!, loggerMock);

        var result = await personService.TryCreatePersonAsync(personAadInfo);

        result.Should().BeNull();
    }
    
    [Fact]
    public async Task Test_TryCreatePersonAsync_When_NewPerson()
    {
        var personProviderMock = new Mock<IPersonProvider>();
        var randomGeneratorMock = new Mock<IRandomGenerator>();
        var loggerMock = new TestLogger<IPersonService>();
        var personId = Guid.NewGuid();
        var ssnMocked = "123-xx-yyyy";
        var roleMocked = PersonRole.adviser.ToString();
        var personDto = new PersonDto
        {
            Id = personId,
            Role = roleMocked,
            Ssn = ssnMocked
        };
        PersonDto? nonExistingPerson = null;
        var personAadInfo = new PersonAadInfo()
        {
            Id = personId,
            Email = "totaly_real_adviser@domain.com",
            FullName = "Totally Adviser"
        };
        personProviderMock
            .Setup(x =>
                x.GetPersonByIdAsync(It.IsAny<Guid>()))!
            .ReturnsAsync(nonExistingPerson);
        personProviderMock
            .Setup(x =>
                x.CreatePersonAsync(It.IsAny<PersonDto>()))
            .ReturnsAsync(personDto);
        randomGeneratorMock.Setup(x =>
                x.GenerateSsn())
            .Returns(ssnMocked);

        var personService = new PersonService(personProviderMock.Object, randomGeneratorMock.Object, loggerMock);

        var result = await personService.TryCreatePersonAsync(personAadInfo);

        result.Should().NotBeNull();
        result?.Id.Should().Be(personId);
        result?.Role.Should().Be(roleMocked);
        result?.Ssn.Should().Be(ssnMocked);
    }
}