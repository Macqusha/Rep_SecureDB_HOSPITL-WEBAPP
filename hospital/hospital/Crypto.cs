using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace hospital
{
    public class Crypto
    {
        public static byte[] myKey = { 131, 240, 69, 92, 59, 137, 79, 67 };
        public static byte[] myIV = { 60, 46,247,206,136,47,26,254 };
        
        public static string Encrypt(string original)
        {
            try
            {
                string ToReturn = "";
                string _key = "ay$a5%&jwrtmnh;lasjdf98787";
                string _iv = "abc@98797hjkas$&asd(*$%";
                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(original);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }                       
        }

        public static string Decrypt(string ciphertext)
        {
            try
            {
                string ToReturn = "";
                string _key = "ay$a5%&jwrtmnh;lasjdf98787";
                string _iv = "abc@98797hjkas$&asd(*$%";
                byte[] _ivByte = { };
                _ivByte = System.Text.Encoding.UTF8.GetBytes(_iv.Substring(0, 8));
                byte[] _keybyte = { };
                _keybyte = System.Text.Encoding.UTF8.GetBytes(_key.Substring(0, 8));
                MemoryStream ms = null; CryptoStream cs = null;
                byte[] inputbyteArray = new byte[ciphertext.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(ciphertext.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(_keybyte, _ivByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }        
    }
}
    