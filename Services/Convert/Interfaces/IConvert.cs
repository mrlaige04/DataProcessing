namespace DataProcessing.Services.Convert.Interfaces
{
    public interface IConvert<T, K>
    {
        T Convert(K data, CancellationToken token = default);
    }
}
