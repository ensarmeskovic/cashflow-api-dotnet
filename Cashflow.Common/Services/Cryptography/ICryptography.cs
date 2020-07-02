namespace Cashflow.Common.Services.Cryptography
{
    public interface ICryptography
    {
        string CreateSalt();

        string CreateHash(string value, string salt);

        bool Validate(string value, string salt, string hash);

        
        string Encrypt(string clearText, string encryptionKey = "T9Qh<6d:[x>pxAGt");

        string Decrypt(string cipherText, string encryptionKey = "T9Qh<6d:[x>pxAGt");
    }
}
