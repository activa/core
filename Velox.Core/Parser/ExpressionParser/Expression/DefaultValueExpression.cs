using System;
using System.Linq;

namespace Velox.Core.Parser
{
    public class DefaultValueExpression : BinaryExpression
    {
        public DefaultValueExpression(Expression value, Expression defaultValue) : base(value, defaultValue)
        {
        }

        public Expression Value
        {
            get { return Left; }
        }

        public Expression DefaultValue
        {
            get { return Right; }
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            ValueExpression result = Value.Evaluate(context);


            if (context.ToBoolean(result.Value))
                return result;
            else
                return DefaultValue.Evaluate(context);
        }

#if DEBUG
        public override string ToString()
        {
            return "(" + Value + " ?: " + DefaultValue + ")";
        }
#endif
    }
}