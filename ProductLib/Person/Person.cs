using System;
using System.Collections.Generic;
using ProductLib.UserRole;

namespace ProductLib.Person
{
    public abstract class Person: IEquatable<Person>
    {
        public long Id { get; set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string AccName { get; private set; }

        public bool Equals(Person? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Email == other.Email && AccName == other.AccName;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Person) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Email, AccName);
        }
    }
    public class User:Person{
        public RoleCollection Roles { get; private set; }
    }
}