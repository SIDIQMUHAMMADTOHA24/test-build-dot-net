using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Share.Tools
{
    public class Kripto
    {
        private readonly byte[] key_document_decrypt;
        private readonly byte[] _nonce;
        public Kripto()
        {
            this.key_document_decrypt = Convert.FromBase64String(Const.private_key_certificate);
            this._nonce = Convert.FromBase64String(Const.nonce);
        }

        public string  Dekrip(string chiper)
        {
            try
            {
                //dekrip key document
                var a = Convert.FromBase64String(chiper);
                var plaint = SecretBox.Open(a, this._nonce, this.key_document_decrypt);//dekrip content form db
                return Encoding.UTF8.GetString(plaint);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        

        public string Enkrip(string plaint)
        {
            try
            {
                var chiper_send_to_client = SecretBox.Create(Encoding.UTF8.GetBytes(plaint), this._nonce, this.key_document_decrypt);
                return Convert.ToBase64String(chiper_send_to_client);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string Hashing(string plaint)
        {
            return BitConverter.ToString(Sodium.CryptoHash.Sha512(plaint)).Replace(@"-", "").ToLower();
        }
    }
}
