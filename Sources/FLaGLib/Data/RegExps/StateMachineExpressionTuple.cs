using FLaGLib.Data.StateMachines;
using System;

namespace FLaGLib.Data.RegExps
{
    public class StateMachineExpressionTuple
    {
        public Expression Expression
        {
            get;
            private set;
        }

        public StateMachine StateMachine
        {
            get;
            private set;
        }

        public int Number
        {
            get;
            private set;
        }

        public StateMachineExpressionTuple(Expression expression, StateMachine stateMachine, int number)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (stateMachine == null)
            {
                throw new ArgumentNullException(nameof(stateMachine));
            }

            Expression = expression;
            StateMachine = stateMachine;
            Number = number;
        }
    }
}
