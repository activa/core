namespace Iridium.Core
{
    public struct JsonToken
    {
        public JsonTokenType Type;
        public string Token;

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
    }
}