namespace Iridium.Core
{
    internal class JsonToken
    {
        public JsonTokenType Type { get; }
        public string Token { get; }

        public JsonToken(JsonTokenType type, string token)
        {
            Type = type;
            Token = token;
        }

        public JsonToken(JsonTokenType type, char token)
        {
            Type = type;
            Token = token.ToString();
        }

        public JsonToken(JsonTokenType type)
        {
            Type = type;
            Token = null;
        }

        public static JsonToken Colon = new JsonToken(JsonTokenType.Colon);
        public static JsonToken Comma = new JsonToken(JsonTokenType.Comma);
        public static JsonToken ObjectStart = new JsonToken(JsonTokenType.ObjectStart);
        public static JsonToken ObjectEnd = new JsonToken(JsonTokenType.ObjectEnd);
        public static JsonToken ArrayStart = new JsonToken(JsonTokenType.ArrayStart);
        public static JsonToken ArrayEnd = new JsonToken(JsonTokenType.ArrayEnd);
        public static JsonToken Eof = new JsonToken(JsonTokenType.EOF);
        public static JsonToken True = new JsonToken(JsonTokenType.True);
        public static JsonToken False = new JsonToken(JsonTokenType.False);
        public static JsonToken Null = new JsonToken(JsonTokenType.Null);
    }
}