namespace DataProcessing.Services.Validate.Interfaces
{
    public interface IValidator
    {
        IValidationResult Validate(string input);
    }
}
