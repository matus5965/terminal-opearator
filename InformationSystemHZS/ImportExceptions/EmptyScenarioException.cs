namespace InformationSystemHZS.ImportExceptions
{
    public class EmptyScenarioException : Exception
    {
        private const string OutputErrorMessage = "trying to load empty scenatio";
        public EmptyScenarioException(string message = OutputErrorMessage) : base(message) { }
    }
}
