using DataRepoCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Entity {
    public class EmployeeEntity {

        /// <summary>
        /// The id of the user.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The personnelNumber used to identify the user.
        /// </summary>
        public short PersonnelNumber { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The role enume encoded as byte
        /// </summary>
        public byte Role { get; set; }

        /// <summary>
        /// The hashed password.
        /// </summary>
        private string _passwordHash;

        /// <summary>
        /// The salt used to hash the password.
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// Check if the provided passwort matches using the password hasher.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <param name="passwordHasher">The hasher to use to check.</param>
        /// <returns>True if the provided password matches, otherwise false.</returns>
        public bool Authenticate(string password, IPasswordHasher<EmployeeEntity> passwordHasher) {
            return passwordHasher.VerifyHashedPassword(this, _passwordHash, password) == PasswordVerificationResult.Success;
        }

        /// <summary>
        /// Set the password of the user.
        /// </summary>
        /// <param name="password">The password to set.</param>
        /// <param name="passwordHasher">The passwordHasher to use to hash the password.</param>
        public void setPassword(string password, IPasswordHasher<EmployeeEntity> passwordHasher) {
            _passwordHash = passwordHasher.HashPassword(this, password);
        }

        public EmployeeEntity(int id, short personnelNumber, string firstName, string lastName,byte role, string _passwordHash, byte[] passwordSalt) {
            Id = id;
            PersonnelNumber = personnelNumber;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            this._passwordHash = _passwordHash;
            PasswordSalt = passwordSalt;
        }

        public EmployeeEntity(Employee employee) {
            Id = employee.Id;
            PersonnelNumber = employee.PersonnelNumber;
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            Role = (byte)(employee.Role);
            this._passwordHash = string.Empty;
            PasswordSalt = new byte[0];
        }

        public Employee MapToDataModel() {
            return new Employee() { FirstName = FirstName, LastName = LastName, Id = Id, PersonnelNumber = PersonnelNumber, Role = (Role)Role };
        }
    }
}
