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

        [Test]
        public void ConvertToDeterministicIfNotTest()
        {
            Label s11State = new Label(new SingleLabel('S',subIndex: 11));
            Label s8State = new Label(new SingleLabel('S', subIndex: 8));
            Label s7State = new Label(new SingleLabel('S', subIndex: 7));
            Label s2State = new Label(new SingleLabel('S', subIndex: 2));
            Label s16State = new Label(new SingleLabel('S', subIndex: 16));
            Label s15State = new Label(new SingleLabel('S', subIndex: 15));
            Label s13State = new Label(new SingleLabel('S', subIndex: 13));
            Label hState = new Label(new SingleLabel('H', subIndex: 11));

            Label[] states = new Label[] 
            {
                s11State,
                s8State,
                s7State,
                s2State,
                s16State,
                s15State,
                s13State,
                hState,
            };

            Label initialState = s11State;

            Transition[] transitions = new Transition[] 
            {
                new Transition(s11State, 'a', s8State),
                new Transition(s11State, 'c', s8State),
                new Transition(s11State, 'a', s16State),
                new Transition(s11State, 'c', s16State),
                new Transition(s11State, 'a', s2State),
                new Transition(s8State, 'a', s8State),
                new Transition(s8State, 'c', s8State),
                new Transition(s8State, 'a', s16State),
                new Transition(s8State, 'c', s16State),
                new Transition(s7State, 'a', s2State),
                new Transition(s2State, 'c', s7State),
                new Transition(s2State, 'c', s13State),
                new Transition(s16State, 'a', hState),
                new Transition(s16State, 'c', hState),
                new Transition(s16State, 'a', s16State),
                new Transition(s16State, 'c', s16State),
                new Transition(s15State, 'c', hState),
                new Transition(s15State, 'c', s13State),
                new Transition(s13State, 'a', s15State)
            };

            Label[] finalStates = new Label[]
            {
                hState
            };

            StateMachine stateMachine = new StateMachine(initialState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            StateMachine stateMachine1 = stateMachine.ConvertToDeterministicIfNot();
        }
    }
}
