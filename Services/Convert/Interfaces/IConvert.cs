using System.Text.RegularExpressions;

namespace DataProcessing.Services.Convert.Interfaces
{
    public interface IConvert<T>
    {
        T Convert(GroupCollection data);
    }
}
