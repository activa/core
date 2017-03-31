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
        Whitespace,
        Comma,
        Colon,
        Integer,
        Float,
        String,
        EOF
    }
}