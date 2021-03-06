﻿using System;

namespace FLaG.IO.Input
{
    public class AuthorDescription
    {
        public string FirstNameInitial
        {
            get
            {
                return GetInitial(FirstName);
            }
        }

        public string SecondNameInitial
        {
            get
            {
                return GetInitial(SecondName);
            }
        }

        private static string GetInitial(string value)
        {
            if (value.Length > 0)
            {
                return value[0].ToString();
            }

            return string.Empty;
        }

        public string FirstName
        {
            get;
            private set;
        }

        public string SecondName
        {
            get;
            private set;
        }

        public string LastName
        {
            get;
            private set;
        }

        public string Group
        {
            get;
            private set;
        }

        public AuthorDescription(string firstName, string secondName, string lastName, string group)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (secondName == null)
            {
                throw new ArgumentNullException(nameof(secondName));
            }

            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            FirstName = firstName;
            SecondName = secondName;
            LastName = lastName;
            Group = group;
        }
    }
}
