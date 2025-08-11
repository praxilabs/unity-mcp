namespace JsonConverterBase
{
    public interface IConvertFromJson<T>
    {
        T FromJson(string objectJson);
    }
}