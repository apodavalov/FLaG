using FLaGLib.Collections;
using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

            IEnumerable<Label> states = EnumerateHelper.Sequence(            
                kState,
                sState,
                hState,
                pState
            );

            Label initialState = kState;

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                new Transition(kState, 'a', sState),
                new Transition(kState, 'b', pState),
                new Transition(kState, 'c', hState)
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(            
                hState
            );

            StateMachine stateMachine = new StateMachine(kState,finalStates,transitions);

            Assert.AreEqual(true, stateMachine.IsDeterministic());
        }

        [Test]
        public void IsDeterministicTest_NonDeterministic()
        {
            Label kState = new Label(new SingleLabel('K'));
            Label pState = new Label(new SingleLabel('P'));
            Label sState = new Label(new SingleLabel('S'));
            Label hState = new Label(new SingleLabel('H'));

            IEnumerable<Label> states = EnumerateHelper.Sequence(                
                kState,
                sState,
                hState,
                pState
            );

            Label initialState = kState;

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(            
                new Transition(kState, 'a', sState),
                new Transition(kState, 'a', pState),
                new Transition(kState, 'c', hState)
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(              
                hState
            );

            StateMachine stateMachine = new StateMachine(kState, finalStates, transitions);

            Assert.AreEqual(false, stateMachine.IsDeterministic());
        }

        [Test]
        public void ReorganizeTest()
        {
            Label s11State = new Label(new SingleLabel('S', 11));
            Label s8State = new Label(new SingleLabel('S', 8));
            Label s7State = new Label(new SingleLabel('S', 7));
            Label s2State = new Label(new SingleLabel('S', 2));
            Label s16State = new Label(new SingleLabel('S', 16));
            Label s15State = new Label(new SingleLabel('S', 15));
            Label s13State = new Label(new SingleLabel('S', 13));
            Label hState = new Label(new SingleLabel('H', 11));

            IEnumerable<Label> states = EnumerateHelper.Sequence( 
                hState,
                s2State,
                s7State,
                s8State,
                s11State,
                s13State,
                s15State,
                s16State
            );

            Label initialState = s11State;

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence( 
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
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence( 
                hState
            );

            StateMachine stateMachine = new StateMachine(initialState, finalStates, transitions);

            Label s1NewState = new Label(new SingleLabel('S', 1));
            Label s2NewState = new Label(new SingleLabel('S', 2));
            Label s3NewState = new Label(new SingleLabel('S', 3));
            Label s4NewState = new Label(new SingleLabel('S', 4));
            Label s5NewState = new Label(new SingleLabel('S', 5));
            Label s6NewState = new Label(new SingleLabel('S', 6));
            Label s7NewState = new Label(new SingleLabel('S', 7));
            Label s8NewState = new Label(new SingleLabel('S', 8));

            IReadOnlyDictionary<Label, Label> expectedDictionary = EnumerateHelper.Sequence(
                new KeyValuePair<Label, Label>(hState, s1NewState),
                new KeyValuePair<Label, Label>(s2State, s2NewState),
                new KeyValuePair<Label, Label>(s7State, s3NewState),
                new KeyValuePair<Label, Label>(s8State, s4NewState),
                new KeyValuePair<Label, Label>(s11State, s5NewState),
                new KeyValuePair<Label, Label>(s13State, s6NewState),
                new KeyValuePair<Label, Label>(s15State, s7NewState),
                new KeyValuePair<Label, Label>(s16State, s8NewState)
            ).ToDictionary().AsReadOnly();

            IEnumerable<Label> expectedStates = EnumerateHelper.Sequence(
                s1NewState,
                s2NewState,
                s3NewState,
                s4NewState,
                s5NewState,
                s6NewState,
                s7NewState,
                s8NewState
            );

            IEnumerable<Label> expectedFinalStates = EnumerateHelper.Sequence(
                s1NewState
            );

            IEnumerable<Transition> expectedTransitions = EnumerateHelper.Sequence( 
                new Transition(s2NewState, 'c', s3NewState),
                new Transition(s2NewState, 'c', s6NewState),
                new Transition(s3NewState, 'a', s2NewState),
                new Transition(s4NewState, 'a', s4NewState),
                new Transition(s4NewState, 'a', s8NewState),
                new Transition(s4NewState, 'c', s4NewState),
                new Transition(s4NewState, 'c', s8NewState),
                new Transition(s5NewState, 'a', s2NewState),
                new Transition(s5NewState, 'a', s4NewState),
                new Transition(s5NewState, 'a', s8NewState),
                new Transition(s5NewState, 'c', s4NewState),
                new Transition(s5NewState, 'c', s8NewState),
                new Transition(s6NewState, 'a', s7NewState),
                new Transition(s7NewState, 'c', s1NewState),
                new Transition(s7NewState, 'c', s6NewState),
                new Transition(s8NewState, 'a', s1NewState),
                new Transition(s8NewState, 'a', s8NewState),
                new Transition(s8NewState, 'c', s1NewState),
                new Transition(s8NewState, 'c', s8NewState)
            );

            PostReportTestHelper<IReadOnlyDictionary<Label, Label>> helper =
                new PostReportTestHelper<IReadOnlyDictionary<Label, Label>>(expectedDictionary.AsSequence(), OnTuple);

            helper.StartTest();

            StateMachine actualStateMachine = stateMachine.Reorganize('S', helper.OnIterationPostReport);

            helper.FinishTest();

            CollectionAssert.AreEquivalent(stateMachine.Alphabet, actualStateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEquivalent(expectedTransitions, actualStateMachine.Transitions);
            CollectionAssert.AreEquivalent(expectedFinalStates, actualStateMachine.FinalStates);
        }

        private void OnTuple(IReadOnlyDictionary<Label, Label> expected, IReadOnlyDictionary<Label, Label> actual)
        {
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void RemoveUnreachableStatesTest()
        {
            Label s1State = new Label(new SingleLabel('S', 1));
            Label s2State = new Label(new SingleLabel('S', 2));
            Label s3State = new Label(new SingleLabel('S', 3));
            Label s4State = new Label(new SingleLabel('S', 4));
            Label s5State = new Label(new SingleLabel('S', 5));
            Label s6State = new Label(new SingleLabel('S', 6));
            Label s7State = new Label(new SingleLabel('S', 7));
            Label s8State = new Label(new SingleLabel('S', 8));
            Label s9State = new Label(new SingleLabel('S', 9));
            Label s10State = new Label(new SingleLabel('S', 10));
            Label s11State = new Label(new SingleLabel('S', 11));
            Label s12State = new Label(new SingleLabel('S', 12));
            Label s13State = new Label(new SingleLabel('S', 13));
            Label s14State = new Label(new SingleLabel('S', 14));
            Label s15State = new Label(new SingleLabel('S', 15));
            Label s16State = new Label(new SingleLabel('S', 16));

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                new Transition(s1State, 'a',s2State),
                new Transition(s2State, 'a',s4State),
                new Transition(s3State, 'a',s4State),
                new Transition(s4State, 'a',s2State),
                new Transition(s5State, 'a',s2State),
                new Transition(s6State, 'b',s7State),
                new Transition(s7State, 'b',s9State),
                new Transition(s8State, 'b',s9State),
                new Transition(s9State, 'b',s7State),
                new Transition(s9State, 'c',s12State),
                new Transition(s10State,'b',s7State),
                new Transition(s10State,'c',s12State),
                new Transition(s11State,'c',s12State),
                new Transition(s12State,'c',s14State),
                new Transition(s13State,'c',s14State),
                new Transition(s14State,'c',s12State),
                new Transition(s15State,'c',s12State),
                new Transition(s16State,'a',s2State),
                new Transition(s16State,'b',s7State),
                new Transition(s16State,'c',s12State)
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(
                s4State,s5State,s9State,s10State,s14State,s15State,s16State
            );

            Label expectedInitialState = s16State;

            IEnumerable<Transition> expectedTransitions = EnumerateHelper.Sequence(
                new Transition(s2State, 'a',s4State),
                new Transition(s4State, 'a',s2State),
                new Transition(s7State, 'b',s9State),
                new Transition(s9State, 'b',s7State),
                new Transition(s9State, 'c',s12State),
                new Transition(s12State,'c',s14State),
                new Transition(s14State,'c',s12State),
                new Transition(s16State,'a',s2State),
                new Transition(s16State,'b',s7State),
                new Transition(s16State,'c',s12State)
            );

            IEnumerable<Label> expectedFinalStates = EnumerateHelper.Sequence(            
                s4State,s9State,s14State,s16State
            );

            IEnumerable<Label> expectedStates = EnumerateHelper.Sequence(  
                s2State,s4State,s7State,s9State,s12State,s14State,s16State
            );

            IEnumerable<char> expectedAlphabet = EnumerateHelper.Sequence('a', 'b', 'c');

            StateMachine stateMachine = new StateMachine(expectedInitialState, finalStates, transitions);

            RemovingUnreachableStatesBeginPostReport expectedBegin = 
                new RemovingUnreachableStatesBeginPostReport(
                    EnumerateHelper.Sequence(s16State),
                    EnumerateHelper.Sequence(s16State),
                    0);

            IReadOnlyList<RemovingUnreachableStatesIterationPostReport> expectedSequence = EnumerateHelper.Sequence(
                new RemovingUnreachableStatesIterationPostReport(
                    EnumerateHelper.Sequence(s16State),
                    EnumerateHelper.Sequence(s2State,s7State,s12State,s16State),
                    EnumerateHelper.Sequence(s2State,s7State,s12State),
                    EnumerateHelper.Sequence(s2State,s7State,s12State),
                    1,
                    false),
                new RemovingUnreachableStatesIterationPostReport(
                    EnumerateHelper.Sequence(s2State,s7State,s12State,s16State),
                    EnumerateHelper.Sequence(s2State,s4State,s7State,s9State,s12State,s14State,s16State),
                    EnumerateHelper.Sequence(s4State,s9State,s14State),
                    EnumerateHelper.Sequence(s4State,s9State,s14State),
                    2,
                    false),
                new RemovingUnreachableStatesIterationPostReport(
                    EnumerateHelper.Sequence(s2State, s4State, s7State, s9State, s12State, s14State, s16State),
                    EnumerateHelper.Sequence(s2State, s4State, s7State, s9State, s12State, s14State, s16State),
                    EnumerateHelper.Sequence(s2State, s7State, s12State),
                    Enumerable.Empty<Label>(),
                    3,
                    true)
            ).ToList().AsReadOnly();

            PostReportTestHelper<RemovingUnreachableStatesBeginPostReport, RemovingUnreachableStatesIterationPostReport> helper =
                new PostReportTestHelper<RemovingUnreachableStatesBeginPostReport, RemovingUnreachableStatesIterationPostReport>(
                    expectedBegin,    
                    expectedSequence,
                    OnTuple,
                    OnTuple);

            helper.StartTest();

            StateMachine actualStateMachine = stateMachine.RemoveUnreachableStates(helper.OnBeginPostReport, helper.OnIterationPostReport);

            helper.FinishTest();

            CollectionAssert.AreEquivalent(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEquivalent(expectedAlphabet, actualStateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedFinalStates, actualStateMachine.FinalStates);
            CollectionAssert.AreEquivalent(expectedTransitions, actualStateMachine.Transitions);
            Assert.AreEqual(expectedInitialState, actualStateMachine.InitialState);
        }

        private void OnTuple(RemovingUnreachableStatesBeginPostReport current, RemovingUnreachableStatesBeginPostReport tuple)
        {
            Assert.AreEqual(current.Iteration, tuple.Iteration);
            CollectionAssert.AreEquivalent(current.ApproachedStates, tuple.ApproachedStates);
            CollectionAssert.AreEquivalent(current.ReachableStates, tuple.ReachableStates);
        }

        private void OnTuple(RemovingUnreachableStatesIterationPostReport current, RemovingUnreachableStatesIterationPostReport tuple)
        {
            Assert.AreEqual(current.IsLastIteration, tuple.IsLastIteration);
            Assert.AreEqual(current.Iteration, tuple.Iteration);
            CollectionAssert.AreEquivalent(current.CurrentApproachedStates, tuple.CurrentApproachedStates);
            CollectionAssert.AreEquivalent(current.CurrentApproachedMinusCurrentReachableStates, tuple.CurrentApproachedMinusCurrentReachableStates);
            CollectionAssert.AreEquivalent(current.CurrentReachableStates, tuple.CurrentReachableStates);
            CollectionAssert.AreEquivalent(current.NextReachableStates, tuple.NextReachableStates);
        }

        [Test]
        public void GetMetaTransitionsTest_AnyNonSimpleLabel_Fail()
        {
            Label s11State = new Label(new SingleLabel('S', 11));
            Label s8s16State = new Label(
                EnumerateHelper.Sequence(
                    new SingleLabel('S', 8),
                    new SingleLabel('S', 16)
                )                
            );

            IEnumerable<Label> states = EnumerateHelper.Sequence(
                s11State,
                s8s16State 
            );

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                new Transition(s11State, 'a', s8s16State)
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(
                s8s16State
            );

            Label initialState = s11State;

            StateMachine stateMachine = new StateMachine(
                initialState,
                finalStates,
                transitions
            );

            Assert.Throws<InvalidOperationException>(() => stateMachine.GetMetaTransitions());
        }

        [Test]
        public void GetMetaStateTest_Ok()
        {
            Label s1State = new Label(new SingleLabel('S', 1));
            Label s2State = new Label(new SingleLabel('S', 2));
            Label s3State = new Label(new SingleLabel('S', 3));

            Label initialState = s1State;

            IEnumerable<Label> states = EnumerateHelper.Sequence( 
                s1State,
                s2State,
                s3State
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(
                s3State
            );

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                new Transition(s1State, 'a', s2State),
                new Transition(s2State, 'c', s3State),
                new Transition(s1State, 'c', s3State)
            );

            StateMachine stateMachine = new StateMachine(
                initialState,
                finalStates,
                transitions
            );

            IReadOnlySet<Label> metaState = stateMachine.GetMetaStates();

            CollectionAssert.AreEquivalent(states, metaState);
        }

        [Test]
        public void GetMetaStateTest_AnyNonSimpleLabel_Fail()
        {
            Label s11State = new Label(new SingleLabel('S', 11));
            Label s8s16State = new Label(
                EnumerateHelper.Sequence(
                    new SingleLabel('S', 8),
                    new SingleLabel('S', 16)
                )                
            );

            IEnumerable<Label> states = EnumerateHelper.Sequence(
                s11State,
                s8s16State
            );

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                new Transition(s11State, 'a', s8s16State)
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(
                s8s16State
            );

            Label initialState = s11State;

            StateMachine stateMachine = new StateMachine(
                initialState,
                finalStates,
                transitions
            );

            Assert.Throws<InvalidOperationException>(() => stateMachine.GetMetaStates());
        }

        [Test]
        public void GetMetaFinalStateTest_AnyNonSimpleLabel_Fail()
        {
            Label s11State = new Label(new SingleLabel('S', 11));
            Label s8s16State = new Label(
                EnumerateHelper.Sequence(
                    new SingleLabel('S', 8),
                    new SingleLabel('S', 16)
                )                
            );

            IEnumerable<Label> states = EnumerateHelper.Sequence(
                s11State,
                s8s16State 
            );

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                new Transition(s11State, 'a', s8s16State)
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(
                s8s16State
            );

            Label initialState = s11State;

            StateMachine stateMachine = new StateMachine(
                initialState,
                finalStates,
                transitions
            );

            Assert.Throws<InvalidOperationException>(() => stateMachine.GetMetaFinalStates());
        }

        [Test]
        public void GetMetaFinalStateTest_Ok()
        {
            Label s1State = new Label(new SingleLabel('S', 1));
            Label s2State = new Label(new SingleLabel('S', 2));
            Label s3State = new Label(new SingleLabel('S', 3));

            Label initialState = s1State;

            IEnumerable<Label> states = EnumerateHelper.Sequence(
                s1State,
                s2State,
                s3State
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(
                s3State
            );

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                new Transition(s1State, 'a', s2State),
                new Transition(s2State, 'c', s3State),
                new Transition(s1State, 'c', s3State)
            );

            StateMachine stateMachine = new StateMachine(
                initialState, 
                finalStates, 
                transitions
            );

            IEnumerable<Label> expectedOptionalStates = EnumerateHelper.Sequence(
                s1State, s2State
            );

            IEnumerable<Label> expectedRequiredStates = EnumerateHelper.Sequence(
                s3State
            );

            MetaFinalState metaFinalState = stateMachine.GetMetaFinalStates();

            CollectionAssert.AreEquivalent(expectedOptionalStates, metaFinalState.OptionalStates);
            CollectionAssert.AreEquivalent(expectedRequiredStates, metaFinalState.RequiredStates);
        }

        [Test]
        public void GetMetaTransitionsTest_Ok()
        {
            Label s11State = new Label(new SingleLabel('S',11));
            Label s8State = new Label(new SingleLabel('S', 8));
            Label s7State = new Label(new SingleLabel('S', 7));
            Label s2State = new Label(new SingleLabel('S', 2));
            Label s16State = new Label(new SingleLabel('S', 16));
            Label s15State = new Label(new SingleLabel('S', 15));
            Label s13State = new Label(new SingleLabel('S', 13));
            Label hState = new Label(new SingleLabel('H'));

            IEnumerable<Label> states = EnumerateHelper.Sequence( 
                s11State,
                s8State,
                s7State,
                s2State,
                s16State,
                s15State,
                s13State,
                hState
            );

            Label initialState = s11State;

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence( 
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
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence( 
                hState
            );

            StateMachine stateMachine = new StateMachine(initialState, finalStates, transitions);

            IEnumerable<MetaTransition> expectedMetaTransitions = EnumerateHelper.Sequence(             
                new MetaTransition(
                    EnumerateHelper.Sequence(s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s11State, s13State, s15State),
                    'a',
                    EnumerateHelper.Sequence(hState, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s13State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s11State, s15State ),
                    'a',
                    EnumerateHelper.Sequence(hState, s15State, s16State )
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s13State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s11State, s15State, s16State ),
                    'a',
                    EnumerateHelper.Sequence(s15State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s11State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s13State, s15State),
                    'a',
                    EnumerateHelper.Sequence(hState, s2State, s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s11State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s13State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s2State, s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s11State, s13State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s15State),
                    'a',
                    EnumerateHelper.Sequence(hState, s2State, s8State, s15State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s11State, s13State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s2State, s8State, s15State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s8State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s11State, s13State, s15State ),
                    'a',
                    EnumerateHelper.Sequence(hState, s8State, s16State)
                 ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s8State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s11State, s13State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s8State, s13State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s11State, s15State),
                    'a',
                    EnumerateHelper.Sequence(hState, s8State, s15State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s8State, s13State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s11State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s8State, s15State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s8State, s11State, s13State, s15State ),
                    'a',
                    EnumerateHelper.Sequence(hState, s2State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State),
                    EnumerateHelper.Sequence(hState, s2State, s8State, s11State, s13State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s2State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State, s13State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s8State, s11State, s15State),
                    'a',
                    EnumerateHelper.Sequence(hState, s2State, s15State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State, s13State),
                    EnumerateHelper.Sequence(hState, s2State, s8State, s11State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s2State, s15State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State, s8State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s11State, s13State, s15State),
                    'a',
                    EnumerateHelper.Sequence(hState, s2State, s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State, s8State),
                    EnumerateHelper.Sequence(hState, s2State, s11State, s13State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s2State, s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State, s8State, s13State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s11State, s15State),
                    'a',
                    EnumerateHelper.Sequence(hState, s2State, s8State, s15State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s7State, s8State, s13State),
                    EnumerateHelper.Sequence(hState, s2State, s11State, s15State, s16State),
                    'a',
                    EnumerateHelper.Sequence(s2State, s8State, s15State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s15State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s11State, s13State),
                    'c',
                    EnumerateHelper.Sequence(hState, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s11State, s13State, s15State),
                    'c',
                    EnumerateHelper.Sequence(hState, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s15State
                    ),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s11State, s13State, s16State),
                    'c',
                    EnumerateHelper.Sequence(hState, s13State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s11State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s13State, s15State),
                    'c',
                    EnumerateHelper.Sequence(hState, s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s11State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s13State, s15State, s16State),
                    'c',
                    EnumerateHelper.Sequence(s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s11State, s15State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s8State, s13State, s16State),
                    'c',
                    EnumerateHelper.Sequence(hState, s8State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s8State, s16State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s11State, s13State, s15State),
                    'c',
                    EnumerateHelper.Sequence(hState, s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s8State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s11State, s13State, s15State, s16State),
                    'c',
                    EnumerateHelper.Sequence(s8State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s8State, s15State),
                    EnumerateHelper.Sequence(hState, s2State, s7State, s11State, s13State, s16State),
                    'c',
                    EnumerateHelper.Sequence(hState, s8State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s16State),
                    EnumerateHelper.Sequence(hState, s7State, s8State, s11State, s13State, s15State),
                    'c',
                    EnumerateHelper.Sequence(hState, s7State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State),
                    EnumerateHelper.Sequence(hState, s7State, s8State, s11State, s13State, s15State, s16State),
                    'c',
                    EnumerateHelper.Sequence(s7State, s13State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s15State),
                    EnumerateHelper.Sequence(hState, s7State, s8State, s11State, s13State, s16State),
                    'c',
                    EnumerateHelper.Sequence(hState, s7State, s13State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s11State, s16State),
                    EnumerateHelper.Sequence(hState, s7State, s8State, s13State, s15State),
                    'c',
                    EnumerateHelper.Sequence(hState, s7State, s8State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s11State),
                    EnumerateHelper.Sequence(hState, s7State, s8State, s13State, s15State, s16State),
                    'c',
                    EnumerateHelper.Sequence(s7State, s8State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s11State, s15State),
                    EnumerateHelper.Sequence(hState, s7State, s8State, s13State, s16State),
                    'c',
                    EnumerateHelper.Sequence(hState, s7State, s8State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s8State, s16State),
                    EnumerateHelper.Sequence(hState, s7State, s11State, s13State, s15State),
                    'c',
                    EnumerateHelper.Sequence(hState, s7State, s8State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s8State),
                    EnumerateHelper.Sequence(hState, s7State, s11State, s13State, s15State, s16State),
                    'c',
                    EnumerateHelper.Sequence(s7State, s8State, s13State, s16State)
                ),
                new MetaTransition(
                    EnumerateHelper.Sequence(s2State, s8State, s15State),
                    EnumerateHelper.Sequence(hState, s7State, s11State, s13State, s16State),
                    'c',
                    EnumerateHelper.Sequence(hState, s7State, s8State, s13State, s16State)
                )
            );

            IReadOnlySet<MetaTransition> actualMetaTransitions = stateMachine.GetMetaTransitions();

            CollectionAssert.AreEquivalent(expectedMetaTransitions, actualMetaTransitions);
        }

        [Test]
        public void ConvertToDeterministicIfNotTest_Deterministic()
        {
            Label s11State = new Label(new SingleLabel('S', 11));
            Label s8State = new Label(new SingleLabel('S', 8));
            Label s7State = new Label(new SingleLabel('S', 7));

            Label[] states = new Label[] 
            {
                s11State,
                s8State,
                s7State
            };

            Transition[] transitions = new Transition[] 
            {
                new Transition(s11State, 'a', s8State),
                new Transition(s11State, 'c', s8State),
                new Transition(s8State, 'a', s8State),
                new Transition(s8State, 'c', s8State),
                new Transition(s8State, 'b', s7State),
            };

            Label initialState = s11State;

            Label[] finalStates = new Label[]
            {
                s7State
            };

            StateMachine stateMachine = new StateMachine(initialState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            Assert.IsTrue(object.ReferenceEquals(stateMachine, stateMachine.ConvertToDeterministicIfNot()));
        }

        [Test]
        public void ConvertToDeterministicIfNotTest_NonDeterministic()
        {
            Label s11State = new Label(new SingleLabel('S',11));
            Label s8State = new Label(new SingleLabel('S', 8));
            Label s7State = new Label(new SingleLabel('S', 7));
            Label s2State = new Label(new SingleLabel('S', 2));
            Label s16State = new Label(new SingleLabel('S', 16));
            Label s15State = new Label(new SingleLabel('S', 15));
            Label s13State = new Label(new SingleLabel('S', 13));
            Label hState = new Label(new SingleLabel('H', 11));

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

            Label _s11_State = new Label(new HashSet<SingleLabel>(new SingleLabel[]
            {
                s11State.ExtractSingleLabel()
            }));
            Label _s8_s16_State = new Label(new HashSet<SingleLabel>(new SingleLabel[]
            {
                s8State.ExtractSingleLabel(),
                s16State.ExtractSingleLabel(),
            }));
            Label _h_s8_s16_State = new Label(new HashSet<SingleLabel>(new SingleLabel[]
            {
                hState.ExtractSingleLabel(),
                s8State.ExtractSingleLabel(),
                s16State.ExtractSingleLabel()
            }));
            Label _s2_s8_s16_State = new Label(new HashSet<SingleLabel>(new SingleLabel[]
            {
                s2State.ExtractSingleLabel(),
                s8State.ExtractSingleLabel(),
                s16State.ExtractSingleLabel()
            }));
            Label _h_s2_s8_s15_s16_State = new Label(new HashSet<SingleLabel>(new SingleLabel[]
            {
                hState.ExtractSingleLabel(),
                s2State.ExtractSingleLabel(),
                s8State.ExtractSingleLabel(),
                s15State.ExtractSingleLabel(),
                s16State.ExtractSingleLabel()
            }));
            Label _h_s7_s8_s13_s16_State = new Label(new HashSet<SingleLabel>(new SingleLabel[]
            {
                hState.ExtractSingleLabel(),
                s7State.ExtractSingleLabel(),
                s8State.ExtractSingleLabel(),
                s13State.ExtractSingleLabel(),
                s16State.ExtractSingleLabel()
            }));

            Label[] expectedStates = new Label[]
            {
                _s11_State,
                _s8_s16_State,
                _h_s8_s16_State,
                _s2_s8_s16_State,
                _h_s2_s8_s15_s16_State,
                _h_s7_s8_s13_s16_State
            };

            Label expectedInitialState = _s11_State;

            Label[] expectedFinalStates = new Label[]
            {
                _h_s8_s16_State,
                _h_s2_s8_s15_s16_State,
                _h_s7_s8_s13_s16_State
            };

            Transition[] expectedTransitions = new Transition[]
            {
                new Transition(_s11_State,'a',_s2_s8_s16_State),
                new Transition(_s11_State,'c',_s8_s16_State),
                new Transition(_s8_s16_State,'a',_h_s8_s16_State),
                new Transition(_s8_s16_State,'c',_h_s8_s16_State),
                new Transition(_h_s8_s16_State,'a',_h_s8_s16_State),
                new Transition(_h_s8_s16_State,'c',_h_s8_s16_State),
                new Transition(_s2_s8_s16_State,'a',_h_s8_s16_State),
                new Transition(_s2_s8_s16_State,'c',_h_s7_s8_s13_s16_State),
                new Transition(_h_s2_s8_s15_s16_State,'a',_h_s8_s16_State),
                new Transition(_h_s2_s8_s15_s16_State,'c',_h_s7_s8_s13_s16_State),
                new Transition(_h_s7_s8_s13_s16_State,'a',_h_s2_s8_s15_s16_State),
                new Transition(_h_s7_s8_s13_s16_State,'c',_h_s8_s16_State)
            };

            char[] expectedAlphabet = new char[] { 'a', 'c' };

            StateMachine stateMachine = new StateMachine(initialState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            StateMachine actualStateMachine = stateMachine.ConvertToDeterministicIfNot();

            CollectionAssert.AreEquivalent(expectedAlphabet, actualStateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEquivalent(expectedFinalStates, actualStateMachine.FinalStates);
            CollectionAssert.AreEquivalent(expectedTransitions, actualStateMachine.Transitions);
            Assert.AreEqual(expectedInitialState, actualStateMachine.InitialState);
        }

        [Test]
        public void MinimizeTest_AlreadyMinimized()
        {
            Label s11State = new Label(new SingleLabel('S', 11));
            Label s8State = new Label(new SingleLabel('S', 8));
            Label s7State = new Label(new SingleLabel('S', 7));

            Label[] states = new Label[] 
            {
                s11State,
                s8State,
                s7State
            };

            Transition[] transitions = new Transition[] 
            {
                new Transition(s11State, 'a', s8State),
                new Transition(s11State, 'c', s8State),
                new Transition(s8State, 'a', s8State),
                new Transition(s8State, 'c', s8State),
                new Transition(s8State, 'b', s7State),
            };

            Label initialState = s11State;

            Label[] finalStates = new Label[]
            {
                s7State
            };

            StateMachine stateMachine = new StateMachine(initialState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            Assert.IsTrue(object.ReferenceEquals(stateMachine, stateMachine.Minimize()));
        }

        [Test]
        public void MinimizeTest_NotMinimized()
        {
            Label s1State = new Label(new SingleLabel('S', 1));
            Label s2State = new Label(new SingleLabel('S', 2));
            Label s3State = new Label(new SingleLabel('S', 3));
            Label s4State = new Label(new SingleLabel('S', 4));
            Label s5State = new Label(new SingleLabel('S', 5));
            Label s6State = new Label(new SingleLabel('S', 6));

            IEnumerable<Label> states = EnumerateHelper.Sequence( 
                s1State,
                s2State,
                s3State,
                s4State,
                s5State,
                s6State
            );

            Label initialState = s1State;

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence( 
                new Transition(s1State, 'a', s4State),
                new Transition(s1State, 'c', s2State),
                new Transition(s2State, 'a', s3State),
                new Transition(s2State, 'c', s3State),
                new Transition(s3State, 'a', s3State),
                new Transition(s3State, 'c', s3State),
                new Transition(s4State, 'a', s3State),
                new Transition(s4State, 'c', s6State),
                new Transition(s5State, 'a', s3State),
                new Transition(s5State, 'c', s6State),
                new Transition(s6State, 'a', s5State),
                new Transition(s6State, 'c', s3State)
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence( 
                s3State,
                s5State,
                s6State
            );

            IEnumerable<Label> expectedStates = EnumerateHelper.Sequence( 
                s1State,
                s2State,
                s3State
            );

            Label expectedInitialState = s1State;

            IEnumerable<Label> expectedFinalStates = EnumerateHelper.Sequence( 
                s3State
            );

            IEnumerable<Transition> expectedTransitions = EnumerateHelper.Sequence( 
                new Transition(s1State, 'a', s2State),
                new Transition(s1State, 'c', s2State),
                new Transition(s2State, 'a', s3State),
                new Transition(s2State, 'c', s3State),
                new Transition(s3State, 'a', s3State),
                new Transition(s3State, 'c', s3State)
            );

            IEnumerable<char> expectedAlphabet = EnumerateHelper.Sequence('a', 'c');

            StateMachine stateMachine = new StateMachine(initialState, finalStates, transitions);

            IReadOnlyList<IEnumerable<Label>> setsOfEquivalence = EnumerateHelper.Sequence(
                EnumerateHelper.Sequence(
                    s1State,
                    s2State,
                    s4State
                ),
                EnumerateHelper.Sequence(
                    s3State,
                    s5State,
                    s6State
                ),                
                    EnumerateHelper.Sequence(
                        s1State
                ),
                EnumerateHelper.Sequence(
                    s2State,
                    s4State
                )                
            ).ToList().AsReadOnly();

            IReadOnlyList<IEnumerable<char>> charsSet = EnumerateHelper.Sequence(
                EnumerateHelper.Sequence('a','c')
            ).ToList().AsReadOnly();

            MinimizingBeginPostReport expectedBeginPostReport = new MinimizingBeginPostReport(
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(setsOfEquivalence[0]),
                        new SetOfEquivalence(setsOfEquivalence[1])
                    )
                ), 0
             );
            
            IReadOnlyList<MinimizingIterationPostReport> expectedIterationPostReports = EnumerateHelper.Sequence<MinimizingIterationPostReport>(
                new MinimizingIterationPostReport(
                    new SetsOfEquivalence(
                        EnumerateHelper.Sequence(
                            new SetOfEquivalence(setsOfEquivalence[2], new SetOfEquivalenceTransition(charsSet[0], 0).AsSequence()),
                            new SetOfEquivalence(setsOfEquivalence[3], new SetOfEquivalenceTransition(charsSet[0], 1).AsSequence()),
                            new SetOfEquivalence(setsOfEquivalence[1], new SetOfEquivalenceTransition(charsSet[0], 1).AsSequence())
                        )
                    ), 1, false
                 ),
                new MinimizingIterationPostReport(
                    new SetsOfEquivalence(
                        EnumerateHelper.Sequence(
                            new SetOfEquivalence(setsOfEquivalence[2], new SetOfEquivalenceTransition(charsSet[0], 1).AsSequence()),
                            new SetOfEquivalence(setsOfEquivalence[3], new SetOfEquivalenceTransition(charsSet[0], 2).AsSequence()),
                            new SetOfEquivalence(setsOfEquivalence[1], new SetOfEquivalenceTransition(charsSet[0], 2).AsSequence())
                        )
                    ), 2, true
                 )
            ).ToList().AsReadOnly();

            bool expectedEndPostReport = true;

            PostReportTestHelper<MinimizingBeginPostReport, MinimizingIterationPostReport, bool> helper =
                new PostReportTestHelper<MinimizingBeginPostReport, MinimizingIterationPostReport, bool>
                (expectedBeginPostReport, expectedIterationPostReports, expectedEndPostReport, OnTuple, OnTuple, OnTuple);

            helper.StartTest();

            StateMachine actualStateMachine = stateMachine.Minimize(helper.OnBeginPostReport, helper.OnIterationPostReport, helper.OnEndPostReport);

            helper.FinishTest();

            CollectionAssert.AreEquivalent(expectedAlphabet, actualStateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEquivalent(expectedFinalStates, actualStateMachine.FinalStates);
            CollectionAssert.AreEquivalent(expectedTransitions, actualStateMachine.Transitions);
            Assert.AreEqual(expectedInitialState, actualStateMachine.InitialState);
        }

        private void OnTuple(bool actual, bool expected)
        {
            Assert.AreEqual(expected, actual);
        }

        private void OnTuple(MinimizingBeginPostReport actual, MinimizingBeginPostReport expected)
        {
            Assert.AreEqual(expected.Iteration, actual.Iteration);
            CheckSetsOfEquivalence(expected.SetsOfEquivalence, actual.SetsOfEquivalence);
        }

        private void OnTuple(MinimizingIterationPostReport actual, MinimizingIterationPostReport expected)
        {
            Assert.AreEqual(expected.Iteration, actual.Iteration);
            Assert.AreEqual(expected.IsLastIteration, actual.IsLastIteration);
            CheckSetsOfEquivalence(expected.SetsOfEquivalence, actual.SetsOfEquivalence);
        }

        private void CheckSetsOfEquivalence(SetsOfEquivalence expected, SetsOfEquivalence actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            using (IEnumerator<SetOfEquivalence> expectedSetOfEquivalenceEnumerator = expected.GetEnumerator())
            {
                using (IEnumerator<SetOfEquivalence> actualSetOfEquivalenceEnumerator = actual.GetEnumerator())
                {
                    while (expectedSetOfEquivalenceEnumerator.MoveNext() && actualSetOfEquivalenceEnumerator.MoveNext())
                    {
                        Assert.AreEqual(expectedSetOfEquivalenceEnumerator.Current, actualSetOfEquivalenceEnumerator.Current);

                        CheckSetOfEquivalenceTransitions(expectedSetOfEquivalenceEnumerator.Current.Transitions, actualSetOfEquivalenceEnumerator.Current.Transitions);
                    }
                }
            }
        }

        private void CheckSetOfEquivalenceTransitions(IReadOnlyList<SetOfEquivalenceTransition> expected, IReadOnlyList<SetOfEquivalenceTransition> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            using (IEnumerator<SetOfEquivalenceTransition> expectedSetOfEquivalenceTransitionsEnumerator = expected.GetEnumerator())
            {
                using (IEnumerator<SetOfEquivalenceTransition> actualSetOfEquivalenceTransitionsEnumerator = actual.GetEnumerator())
                {
                    while (expectedSetOfEquivalenceTransitionsEnumerator.MoveNext() && actualSetOfEquivalenceTransitionsEnumerator.MoveNext())
                    {
                        Assert.AreEqual(expectedSetOfEquivalenceTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence,
                            actualSetOfEquivalenceTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence);

                        CollectionAssert.AreEquivalent(
                            expectedSetOfEquivalenceTransitionsEnumerator.Current.Symbols,
                            actualSetOfEquivalenceTransitionsEnumerator.Current.Symbols);
                    }
                }
            }
        }

        [Test]
        public void CctorTest_InitialStateNull_Fail()
        {
            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))));
            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(new Label(new SingleLabel('b')));

            Assert.Throws<ArgumentNullException>(() => new StateMachine(null, finalStates, transitions));
        }

        [Test]
        public void CctorTest_FinalStatesNull_Fail()
        {
            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))));
            Label initialState = new Label(new SingleLabel('c'));

            Assert.Throws<ArgumentNullException>(() => new StateMachine(initialState, null, transitions));
        }

        [Test]
        public void CctorTest_TransitionsNull_Fail()
        {
            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(new Label(new SingleLabel('b')));
            Label initialState = new Label(new SingleLabel('c'));

            Assert.Throws<ArgumentNullException>(() => new StateMachine(initialState, finalStates, null));
        }

        [Test]
        public void CctorTest_OneFinalStateNull_Fail()
        {
            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))));
            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(new Label(new SingleLabel('b')), null);
            Label initialState = new Label(new SingleLabel('c'));

            Assert.Throws<ArgumentException>(() => new StateMachine(initialState, finalStates, transitions));
        }

        [Test]
        public void CctorTest_OneTransitionNull_Fail()
        {
            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))), null);
            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(new Label(new SingleLabel('b')));
            Label initialState = new Label(new SingleLabel('c'));

            Assert.Throws<ArgumentException>(() => new StateMachine(initialState, finalStates, transitions));
        }

        [Test]
        public void CctorTest_NoTransitions_Ok()
        {
            IEnumerable<Transition> transitions = Enumerable.Empty<Transition>();
            Label expectedInitialState = new Label(new SingleLabel('b'));
            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(expectedInitialState);

            StateMachine stateMachine = new StateMachine(expectedInitialState, finalStates, transitions);

            IEnumerable<Label> expectedStates = EnumerateHelper.Sequence(expectedInitialState);

            IEnumerable<char> expectedAlphabet = Enumerable.Empty<char>();

            IEnumerable<Transition> expectedTransitions = Enumerable.Empty<Transition>();

            IEnumerable<Label> expectedFinalStates = EnumerateHelper.Sequence(expectedInitialState);

            CollectionAssert.AreEquivalent(expectedAlphabet, stateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedStates, stateMachine.States);
            CollectionAssert.AreEquivalent(expectedTransitions, stateMachine.Transitions);
            CollectionAssert.AreEquivalent(expectedFinalStates, stateMachine.FinalStates);
            Assert.AreEqual(expectedInitialState, stateMachine.InitialState);
        }

        [Test]
        public void CctorTest_NoFinalStates_Fail()
        {
            IEnumerable<Transition> transitions = Enumerable.Empty<Transition>();
            Label initialState = new Label(new SingleLabel('b'));
            IEnumerable<Label> finalStates = Enumerable.Empty<Label>();

            Assert.Throws<ArgumentException>(() => new StateMachine(initialState, finalStates, transitions));
        }

        [Test]
        public void CctorTest_Ok()
        {
            Label sState = new Label(new SingleLabel('S'));
            Label dState = new Label(new SingleLabel('D'));
            Label pState = new Label(new SingleLabel('P'));
            Label kState = new Label(new SingleLabel('K'));
            Label aState = new Label(new SingleLabel('A'));

            IEnumerable<Label> expectedStates = EnumerateHelper.Sequence(
                aState,
                dState,
                kState,
                pState,
                sState
            );

            IEnumerable<char> expectedAlphabet = EnumerateHelper.Sequence(
                'a',
                'b',
                'c',
                'd'
            );

            Label expectedInitialState = kState;

            Transition s_a_kTranstition = new Transition(sState, 'a', kState);
            Transition k_b_pTranstition = new Transition(kState, 'b', pState);
            Transition p_c_aTranstition = new Transition(pState, 'c', aState);
            Transition a_d_dTranstition = new Transition(aState, 'd', dState);

            IEnumerable<Transition> expectedTransitions = EnumerateHelper.Sequence(
                a_d_dTranstition,
                k_b_pTranstition,
                p_c_aTranstition,
                s_a_kTranstition
            );

            IEnumerable<Label> expectedfinalStates = EnumerateHelper.Sequence(
                kState,
                pState
            );

            IEnumerable<Transition> transitions = EnumerateHelper.Sequence(
                s_a_kTranstition,
                k_b_pTranstition,
                p_c_aTranstition,
                a_d_dTranstition
            );

            IEnumerable<Label> finalStates = EnumerateHelper.Sequence(
                pState,
                kState
            );

            StateMachine stateMachine = new StateMachine(expectedInitialState, finalStates, transitions);

            CollectionAssert.AreEquivalent(expectedAlphabet, stateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedStates, stateMachine.States);
            CollectionAssert.AreEquivalent(expectedTransitions, stateMachine.Transitions);
            CollectionAssert.AreEquivalent(expectedfinalStates, stateMachine.FinalStates);
            Assert.AreEqual(expectedInitialState, stateMachine.InitialState);
        }
    }
}
