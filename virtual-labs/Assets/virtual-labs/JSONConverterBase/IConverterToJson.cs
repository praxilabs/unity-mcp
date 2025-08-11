namespace JsonConverterBase
{
    public interface IConvertToJson<T>
    {
        string ToJson(T objectToConvert, bool prettyPrint);
    }
}