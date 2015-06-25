using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Tiger.Tiger_Error;

namespace Tiger
{
    public class WrapperLexer : tigerLexer
    {
        public List<SyntacticError> Errors { get; set; }

        public WrapperLexer(ICharStream stream)
            : base(stream)
        {
            Errors = new List<SyntacticError>();
        }

        public WrapperLexer(ICharStream iCharStream, RecognizerSharedState state)
            : base(iCharStream, state)
        {
            Errors = new List<SyntacticError>();
        }

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            string msg = GetErrorMessage(e, tokenNames);

            Errors.Add(new SyntacticError() { Line = e.Line, Column = e.CharPositionInLine, Message = msg });
        }
    }
}
