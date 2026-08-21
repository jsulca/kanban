using System.Security.Cryptography;
using System.Text;

namespace Kanban.SharedKernel;

public class CryptographyHelper
{
    //The key used for generating the encrypted string
    private const string _CryptoKey = @"*1Am|9Kh)+/?sfGq88";

    //The Initialization Vector for the DES encryption routine
    private static readonly byte[] _Iv = { 241, 0, 34, 19, 23, 32, 44, 151 };

    /// <summary>
    /// </summary>
    /// <param name="sourceString"></param>
    /// <returns></returns>
    public static string Encrypt(string sourceString)
    {
        return Encrypt(sourceString, _CryptoKey);
    }

    /// <summary>
    /// </summary>
    /// <param name="sourceString"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Encrypt(string sourceString, string key)
    {
        var buffer = Encoding.ASCII.GetBytes(sourceString);
        var des = new TripleDESCryptoServiceProvider();
        var MD5 = new MD5CryptoServiceProvider();
        des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
        des.IV = _Iv;
        var result = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        des.Clear();
        MD5.Clear();
        return result;
    }

    /// <summary>
    /// </summary>
    /// <param name="encryptedString"></param>
    /// <returns></returns>
    public static string Decrypt(string encryptedString)
    {
        return Decrypt(encryptedString, _CryptoKey);
    }

    /// <summary>
    /// </summary>
    /// <param name="encryptedString"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Decrypt(string encryptedString, string key)
    {
        var buffer = Convert.FromBase64String(encryptedString);
        var des = new TripleDESCryptoServiceProvider();
        var MD5 = new MD5CryptoServiceProvider();
        des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(key));
        des.IV = _Iv;
        var result = Encoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        des.Clear();
        MD5.Clear();
        return result;
    }
}