using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Tiger_Error;

namespace Tiger
{
    public class WrapperParser : tigerParser
    {
        public List<SyntacticError> Errors { get; set; }

        public WrapperParser(ITokenStream stream)
            : base(stream)
        {
            Errors = new List<SyntacticError>();
        }

        public WrapperParser(ITokenStream input, RecognizerSharedState state)
            : base(input, state)
        {
            Errors = new List<SyntacticError>();
        }

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            string msg = GetErrorMessage(e, tokenNames);

            Errors.Add(new SyntacticError() { Message = msg, Line = e.Line, Column = e.CharPositionInLine });
        }
    }
}
