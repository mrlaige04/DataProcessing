namespace DataProcessing.Services.Validate.Interfaces
{
    public interface IValidationOption
    {
        public IValidationResult Validate(string input);
    }
}
