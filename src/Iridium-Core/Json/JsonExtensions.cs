namespace Iridium.Core
{
    public static class JsonExtensions
    {
        public static string Serialize(this JsonObject json, JsonDateFormat dateFormat = JsonDateFormat.SlashDate)
        {
            return JsonSerializer.ToJson(json, dateFormat);
        }

        public static JsonObject ParseJson(this string s)
        {
            return JsonParser.Parse(s);
        }

        public static T ParseJson<T>(this string s) where T:class,new()
        {
            return JsonParser.Parse<T>(s);
        }
    }
}