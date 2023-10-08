namespace Portal.Core.Generation;

public interface IRandomGenerator
{
    String GenerateRandomSsn();
    decimal GenerateRandomPaymentAmount(decimal totalAmount);
}