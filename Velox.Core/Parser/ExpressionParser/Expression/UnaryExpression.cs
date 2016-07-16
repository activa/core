namespace Velox.Core.Parser
{
    public abstract class UnaryExpression : Expression
    {
        public Expression Value { get; private set; }

        protected UnaryExpression(Expression value)
        {
            Value = value;
        }
    }
}