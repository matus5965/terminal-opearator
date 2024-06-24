namespace InformationSystemHZS.ImportExceptions
{
    public class InvalidVehicleTypeException : Exception
    {
        private const string OutputErrorMessage = "trying to load scenario with vehicle of unknown type";
        public InvalidVehicleTypeException(string message = OutputErrorMessage) : base(message) { }
    }
}