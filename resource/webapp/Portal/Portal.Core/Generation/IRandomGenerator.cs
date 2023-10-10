namespace Portal.Core.Generation;

public interface IRandomGenerator
{
    String GenerateSsn();
    Decimal GenerateRandomPaymentAmount(Decimal totalAmount);
}