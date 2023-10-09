namespace Portal.Core.Generation;

public interface IRandomGenerator
{
    String GenerateSsn();
    decimal GenerateRandomPaymentAmount(decimal totalAmount);
}