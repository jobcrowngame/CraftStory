using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 暗号化マネージャー
/// </summary>
public class CryptMng : Single<CryptMng>
{
    string Prmkey = "4h5f2h4d31h4f1gf";
    string PrmVi = "ggsd5g1h6r3f1h0d";

    //暗号化
    public string EncryptString(string plainText)
    {
        byte[] key = Encoding.UTF8.GetBytes(Prmkey);
        byte[] iv = Encoding.UTF8.GetBytes(PrmVi);
        //Instantiate a new Aes object to perform string symmetric encryption
        Aes encryptor = Aes.Create();
        encryptor.Mode = CipherMode.CBC;
        //Set key and IV
        byte[] aesKey = new byte[16];
        Array.Copy(key, 0, aesKey, 0, 16);
        encryptor.Key = aesKey;
        encryptor.IV = iv;
        //Instantiate a new MemoryStream object to contain the encrypted bytes
        MemoryStream memoryStream = new MemoryStream();
        //Instantiate a new encryptor from our Aes object
        ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
        //Instantiate a new CryptoStream object to process the data and write it to the
        //memory stream
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
        //Convert the plainText string into a byte array
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        //Encrypt the input plaintext string
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        //Complete the encryption process
        cryptoStream.FlushFinalBlock();
        //Convert the encrypted data from a MemoryStream to a byte array
        byte[] cipherBytes = memoryStream.ToArray();
        //Close both the MemoryStream and the CryptoStream
        memoryStream.Close();
        cryptoStream.Close();
        //Convert the encrypted byte array to a base64 encoded string
        string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
        //Return the encrypted data as a string
        return cipherText;
    }
    public string DecryptString(string cipherText)
    {
        byte[] key = Encoding.UTF8.GetBytes(Prmkey);
        byte[] iv = Encoding.UTF8.GetBytes(PrmVi);
        //Instantiate a new Aes object to perform string symmetric encryption
        Aes encryptor = Aes.Create();
        encryptor.Mode = CipherMode.CBC;
        //Set key and IV
        byte[] aesKey = new byte[16];
        Array.Copy(key, 0, aesKey, 0, 16);
        encryptor.Key = aesKey;
        encryptor.IV = iv;
        //Instantiate a new MemoryStream object to contain the encrypted bytes
        MemoryStream memoryStream = new MemoryStream();
        //Instantiate a new encryptor from our Aes object
        ICryptoTransform aesDecryptor = encryptor.CreateDecryptor();
        //Instantiate a new CryptoStream object to process the data and write it to the
        //memory stream
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
        //Will contain decrypted plaintext
        string plainText = String.Empty;
        try
        {
            //Convert the ciphertext string into a byte array
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            //Decrypt the input ciphertext string
            cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
            //Complete the decryption process
            cryptoStream.FlushFinalBlock();
            //Convert the decrypted data from a MemoryStream to a byte array
            byte[] plainBytes = memoryStream.ToArray();
            //Convert the decrypted byte array to string
            plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            Logger.Error(cipherText);
        }
        finally
        {
            //Close both the MemoryStream and the CryptoStream
            memoryStream.Close();
            cryptoStream.Close();
        }
        //Return the decrypted data as a string
        return plainText;
    }
}
