using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.DpApi
{
    public class ManagePk
    {
        private readonly static byte[] entropy = { 1, 2, 3, 4, 5, 6 };
        public ManagePk()
        {

        }

        public string Encrypt(string text)
        {
            try
            {
                // first, convert the text to byte array 
                byte[] originalText = Encoding.Unicode.GetBytes(text);

                // then use Protect() to encrypt your data 
                byte[] encryptedText = ProtectedData.Protect(originalText, entropy, DataProtectionScope.LocalMachine);

                //and return the encrypted message 
                return Convert.ToBase64String(encryptedText);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string Decrypt(string text)
        {
            try
            {
                // the encrypted text, converted to byte array 
                byte[] encryptedText = Convert.FromBase64String(text);

                // calling Unprotect() that returns the original text 
                byte[] originalText = ProtectedData.Unprotect(encryptedText, entropy, DataProtectionScope.LocalMachine);

                // finally, returning the result 
                return Encoding.Unicode.GetString(originalText);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
