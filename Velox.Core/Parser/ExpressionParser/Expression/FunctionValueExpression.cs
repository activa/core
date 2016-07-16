using System;

namespace Velox.Core.Parser
{
    public class FunctionValueExpression : ValueExpression
    {
        public FunctionValueExpression(FunctionDefinitionExpression function) : base(function, typeof(FunctionDefinitionExpression))
        {
        }
    }
}