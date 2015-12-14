using System;

namespace FLaGLib.Data.RegExps
{
    public class StateMachineExpressionWithOriginal
    {
        public StateMachineExpressionTuple StateMachineExpression
        {
            get;
            private set;
        }

        public StateMachineExpressionTuple OriginalStateMachineExpression
        {
            get;
            private set;
        }

        public StateMachineExpressionWithOriginal(StateMachineExpressionTuple stateMachineExpression, StateMachineExpressionTuple originalStateMachineExpression = null)
        {
            if (stateMachineExpression == null)
            {
                throw new ArgumentNullException(nameof(stateMachineExpression));
            }

            StateMachineExpression = stateMachineExpression;
            OriginalStateMachineExpression = originalStateMachineExpression;
        }
    }
}
