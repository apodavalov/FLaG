using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using NUnit.Framework;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class StateMachineTest
    {
        [Test]
        public void IsDeterministicTest_Deterministic()
        {
            Label kState = new Label(new SingleLabel('K'));
            Label pState = new Label(new SingleLabel('P'));
            Label sState = new Label(new SingleLabel('S'));
            Label hState = new Label(new SingleLabel('H'));

            Label[] states = new Label[] 
            {
                kState,
                sState,
                hState,
                pState
            };

            Label initialState = kState;

            Transition[] transitions = new Transition[] 
            {
                new Transition(kState, 'a', sState),
                new Transition(kState, 'b', pState),
                new Transition(kState, 'c', hState)
            };

            Label[] finalStates = new Label[]
            {
                hState
            };

            StateMachine stateMachine = new StateMachine(kState,new HashSet<Label>(finalStates),new HashSet<Transition>(transitions));

            Assert.AreEqual(true, stateMachine.IsDeterministic());
        }

        [Test]
        public void IsDeterministicTest_NonDeterministic()
        {
            Label kState = new Label(new SingleLabel('K'));
            Label pState = new Label(new SingleLabel('P'));
            Label sState = new Label(new SingleLabel('S'));
            Label hState = new Label(new SingleLabel('H'));

            Label[] states = new Label[] 
            {
                kState,
                sState,
                hState,
                pState
            };

            Label initialState = kState;

            Transition[] transitions = new Transition[] 
            {
                new Transition(kState, 'a', sState),
                new Transition(kState, 'a', pState),
                new Transition(kState, 'c', hState)
            };

            Label[] finalStates = new Label[]
            {
                hState
            };

            StateMachine stateMachine = new StateMachine(kState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            Assert.AreEqual(false, stateMachine.IsDeterministic());
        }
    }
}
