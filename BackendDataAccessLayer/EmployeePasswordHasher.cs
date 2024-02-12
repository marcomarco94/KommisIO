using BackendDataAccessLayer.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


namespace BackendDataAccessLayer {
    //Inspired by https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-8.0
    public class EmployeePasswordHasher : IPasswordHasher<EmployeeEntity> {
        public string HashPassword(EmployeeEntity user, string password) {
            user.PasswordSalt = RandomNumberGenerator.GetBytes(16);
            string hashed = Convert.ToBase64String(HashPasswordBytes(user, password));
            return hashed;
        }

        public PasswordVerificationResult VerifyHashedPassword(EmployeeEntity user, string hashedPassword, string providedPassword) {
            string hashed = Convert.ToBase64String(HashPasswordBytes(user, providedPassword));
            return hashed.Equals(hashedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        protected byte[] HashPasswordBytes(EmployeeEntity user, string password) {
            return KeyDerivation.Pbkdf2(password, user.PasswordSalt, KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000, numBytesRequested: 32);
        }
    }
}
