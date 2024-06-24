namespace InformationSystemHZS.ImportExceptions
{
    public class InvalidMembersCountException : Exception
    {
        private const string OutputErrorMessage = "trying to load unit with invalid number of members";
        public InvalidMembersCountException(string message = OutputErrorMessage) : base(message) { }
    }
}
