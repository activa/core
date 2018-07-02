namespace Iridium.Core
{
    internal enum JsonTokenType
    {
        ObjectStart,
        ObjectEnd,
        ArrayStart,
        ArrayEnd,
        Null,
        True,
        False,
        Comma,
        Colon,
        Integer,
        Float,
        String,
        EOF
    }
}