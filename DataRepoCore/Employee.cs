using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public struct Employee {
        /// <summary>
        /// The first name of the employee
        /// </summary>
        public required string FirstName { get; init; }

        /// <summary>
        /// The last name of the employee.
        /// </summary>
        public required string LastName { get; init; }

        /// <summary>
        /// A individual number to identify the employee (unique Key), it is Unique but not the tech. primary key. (Seperation of tech. and buis. logic)
        /// </summary>
        public required short PersonnelNumber { get; init; }

        /// <summary>
        /// The assigned roles, an enum because the product is intended as demo, so no further customization is needed. 
        /// This strips away the not necessary complexity of the authentication system and database. 
        /// </summary>
        public required Role Role { get; init; }

        public override int GetHashCode() {
            //Using business
            return PersonnelNumber.GetHashCode();
        }

        public override bool Equals(object? obj) {
            if(obj is  Employee e) {
                //Using business logic to compare the objects.
                return e.PersonnelNumber == PersonnelNumber;
            }
            return false;
        }
    }
}
