namespace Cashflow.Common.Services.RandomGenerator
{
    public interface IRandomGenerator
    {
        string GenerateRandomNumberString(int length);
        string GenerateRandomAlphanumericString(int length);
    }
}
