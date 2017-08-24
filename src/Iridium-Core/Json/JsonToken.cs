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
    }
}