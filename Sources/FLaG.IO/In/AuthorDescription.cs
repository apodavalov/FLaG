namespace FLaG.IO.In
{
    public sealed record AuthorDescription(
        string FirstName,
        string SecondName,
        string LastName,
        string Group
    )
    {
        public string FirstNameInitial => GetInitial(FirstName);

        public string SecondNameInitial => GetInitial(SecondName);

        private static string GetInitial(string value)
        {
            if (value.Length > 0)
            {
                return value[0].ToString();
            }

            return string.Empty;
        }
    }
}
