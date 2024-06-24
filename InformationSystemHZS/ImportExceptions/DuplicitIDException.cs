namespace InformationSystemHZS.ImportExceptions
{
    public class DuplicitIDException : Exception
    {
        private const string OutputErrorMessage = "trying to load scenario with duplicit IDs of ";
        public DuplicitIDException(string duplicitGroup = OutputErrorMessage) : base($"{OutputErrorMessage} {duplicitGroup}") { }
    }
}
