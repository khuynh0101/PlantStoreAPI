using PlantStoreAPI.DataModel;
using PlantStoreAPI.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PlantStoreAPI.Repositories
{
    public class SecurityRepository
    {
        private readonly string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private PlantStoreDBContext _context = null;
        public SecurityRepository() {
            _context = new PlantStoreDBContext();
        }

        public bool SignIn(HttpRequestMessage request)
        {
            if (request.Headers.Authorization != null)
            {
                string authenticationToken = request.Headers.Authorization.Parameter;
                string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));

                string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                string username = "";
                string password = "";
                if (usernamePasswordArray.Length > 1)
                {
                    username = usernamePasswordArray[0];
                    password = usernamePasswordArray[1];
                }
                return new SecurityRepository().ValidateUser(username, password);
            }
            return false;
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                string hashedPassword = HashPassword(password);
                using (_context)
                {
                    var user = from u in _context.Users
                               where u.UserName.Trim().ToLower() == username.ToLower() && u.Password == hashedPassword
                               select u;
                    return user.Any();
                }
            }
            catch (Exception ex)
            {
                CustomLogging.LogMessage(TracingLevel.ERROR, new Exception("Error validating user", ex));
                throw ex;
            }
        }

        public string AddUser(User user)
        {
            user.UserName = user.UserName.Trim();
            user.Password = HashPassword(user.Password).Trim();
            using (_context)
            {
                if (!UserExists(user.UserName))
                {
                    //_context.Users.Add(user);
                    _context.sp_add_user(user.UserName, user.Password);
                    _context.SaveChanges();
                    return "Registered";
                }
                else
                    return "Exists";
            }
        }

        public string ResetPassword(User user)
        {
            try
            {
                string decryptedUser = Decrypt(user.UserName);
                string[] decryptedUserAndTime = decryptedUser.Split('|');
                if (decryptedUserAndTime.Length > 1)
                {
                    if (DateTime.Now.Subtract(Convert.ToDateTime(decryptedUserAndTime[1])).TotalMinutes <= 5)
                        {
                        user.UserName = decryptedUserAndTime[0].Trim();
                        user.Password = HashPassword(user.Password).Trim();
                        using (_context)
                        {
                            _context.SPUpdatePassword(user.UserName, user.Password);
                            _context.SaveChanges();
                            return "Reset";
                        }
                    }
                    else return "Invalid Token";
                }
                return "Invalid Token";
            }
            catch (Exception)
            {
                return "Failed";
            }
        }

        public string GenerateToken(User user)
        {
            string encryptedString = Encrypt(user.UserName);
            return encryptedString;
        }

        private string Encrypt(string encryptString)
        {
            encryptString = string.Format("{0}|{1}", encryptString, DateTime.Now.ToShortTimeString());
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        private string HashPassword(string password)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hashedBytes = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(password));
                mySHA256.Clear();
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool UserExists(string username)
        {
            var user = from u in _context.Users
                       where u.UserName.ToLower() == username
                       select u;
            return user.Any();
        }
    }
}