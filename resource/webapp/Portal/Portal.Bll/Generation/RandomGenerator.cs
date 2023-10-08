using Portal.Core.Generation;

namespace Portal.Bll.Generation;

public class RandomGenerator : IRandomGenerator
{

    private readonly Random _random_ = new();

    public String GenerateSsn()
    {
        var areaNumber = _random_.Next(1, 899);
        var groupNumber = _random_.Next(1, 99);
        var serialNumber = _random_.Next(1, 9999);
        return $"{areaNumber:D3}-{groupNumber:D2}-{serialNumber:D4}";
    }

    public decimal GenerateRandomPaymentAmount(decimal totalAmount)
    {
        decimal minPercentage = 0.0001m;
        decimal maxPercentage = 0.0009m;

        var randomPercentage = (decimal)_random_.NextDouble() * (maxPercentage - minPercentage) + minPercentage;

        return totalAmount * randomPercentage;
    }
}