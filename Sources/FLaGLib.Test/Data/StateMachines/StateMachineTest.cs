using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Collections;
using FLaGLib.Extensions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using FLaGLib.Helpers;

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
        public void ReorganizeTest()
        {
            Label s11State = new Label(new SingleLabel('S', subIndex: 11));
            Label s8State = new Label(new SingleLabel('S', subIndex: 8));
            Label s7State = new Label(new SingleLabel('S', subIndex: 7));
            Label s2State = new Label(new SingleLabel('S', subIndex: 2));
            Label s16State = new Label(new SingleLabel('S', subIndex: 16));
            Label s15State = new Label(new SingleLabel('S', subIndex: 15));
            Label s13State = new Label(new SingleLabel('S', subIndex: 13));
            Label hState = new Label(new SingleLabel('H', subIndex: 11));

            Label[] states = new Label[] 
            {
                hState,
                s2State,
                s7State,
                s8State,
                s11State,
                s13State,
                s15State,
                s16State
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

            Label s1NewState = new Label(new SingleLabel('S', subIndex: 1));
            Label s2NewState = new Label(new SingleLabel('S', subIndex: 2));
            Label s3NewState = new Label(new SingleLabel('S', subIndex: 3));
            Label s4NewState = new Label(new SingleLabel('S', subIndex: 4));
            Label s5NewState = new Label(new SingleLabel('S', subIndex: 5));
            Label s6NewState = new Label(new SingleLabel('S', subIndex: 6));
            Label s7NewState = new Label(new SingleLabel('S', subIndex: 7));
            Label s8NewState = new Label(new SingleLabel('S', subIndex: 8));

            KeyValuePair<Label, Label>[] expectedDictionary = new KeyValuePair<Label, Label>[]
            {
                new KeyValuePair<Label,Label>(hState,s1NewState),
                new KeyValuePair<Label,Label>(s2State,s2NewState),
                new KeyValuePair<Label,Label>(s7State,s3NewState),
                new KeyValuePair<Label,Label>(s8State,s4NewState),
                new KeyValuePair<Label,Label>(s11State,s5NewState),
                new KeyValuePair<Label,Label>(s13State,s6NewState),
                new KeyValuePair<Label,Label>(s15State,s7NewState),
                new KeyValuePair<Label,Label>(s16State,s8NewState)
            };

            Label[] expectedStates = new Label[]
            {
                s1NewState,
                s2NewState,
                s3NewState,
                s4NewState,
                s5NewState,
                s6NewState,
                s7NewState,
                s8NewState
            };

            Label[] expectedFinalStates = new Label[]
            {
                s1NewState
            };

            Transition[] expectedTransitions = new Transition[] 
            {
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
            };

            bool onStateMapInvoked = false;

            StateMachine actualStateMachine = stateMachine.Reorganize('S', 
                stateMap => 
                {
                    Assert.IsFalse(onStateMapInvoked);
                    CollectionAssert.AreEquivalent(expectedDictionary, stateMap);
                    onStateMapInvoked = true;
                }
                );

            Assert.IsTrue(onStateMapInvoked);

            CollectionAssert.AreEqual(stateMachine.Alphabet, actualStateMachine.Alphabet);
            CollectionAssert.AreEqual(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEqual(expectedTransitions, actualStateMachine.Transitions);
            CollectionAssert.AreEqual(expectedFinalStates, actualStateMachine.FinalStates);            
        }

        [Test]
        public void RemoveUnreachableStatesTest()
        {
            Label s1State = new Label(new SingleLabel('S', subIndex: 1));
            Label s2State = new Label(new SingleLabel('S', subIndex: 2));
            Label s3State = new Label(new SingleLabel('S', subIndex: 3));
            Label s4State = new Label(new SingleLabel('S', subIndex: 4));
            Label s5State = new Label(new SingleLabel('S', subIndex: 5));
            Label s6State = new Label(new SingleLabel('S', subIndex: 6));
            Label s7State = new Label(new SingleLabel('S', subIndex: 7));
            Label s8State = new Label(new SingleLabel('S', subIndex: 8));
            Label s9State = new Label(new SingleLabel('S', subIndex: 9));
            Label s10State = new Label(new SingleLabel('S', subIndex: 10));
            Label s11State = new Label(new SingleLabel('S', subIndex: 11));
            Label s12State = new Label(new SingleLabel('S', subIndex: 12));
            Label s13State = new Label(new SingleLabel('S', subIndex: 13));
            Label s14State = new Label(new SingleLabel('S', subIndex: 14));
            Label s15State = new Label(new SingleLabel('S', subIndex: 15));
            Label s16State = new Label(new SingleLabel('S', subIndex: 16));

            Transition[] transitions = new Transition[]
            {
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
            };

            Label[] finalStates = new Label[] 
            {
                s4State,s5State,s9State,s10State,s14State,s15State,s16State
            };

            Label expectedInitialState = s16State;

            Transition[] expectedTransitions = new Transition[]
            {
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
            };

            Label[] expectedFinalStates = new Label[]
            {
                s4State,s9State,s14State,s16State
            };

            Label[] expectedStates = new Label[]
            {
                s2State,s4State,s7State,s9State,s12State,s14State,s16State
            };

            char[] expectedAlphabet = new char[] { 'a', 'b', 'c' };

            StateMachine stateMachine = new StateMachine(expectedInitialState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            RemovingUnreachableStatesPostReport[] expectedSequence = new RemovingUnreachableStatesPostReport[]
            {
                new RemovingUnreachableStatesPostReport(
                    new SortedSet<Label>(new Label[] 
                    {
                        s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s16State
                    }).AsReadOnly(),
                    0),
                new RemovingUnreachableStatesPostReport(
                    new SortedSet<Label>(new Label[] 
                    {
                        s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s7State,s12State,s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s7State,s12State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s7State,s12State
                    }).AsReadOnly(),
                    1),
                new RemovingUnreachableStatesPostReport(
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s7State,s12State,s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s4State,s7State,s9State,s12State,s14State,s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s4State,s9State,s14State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s4State,s9State,s14State
                    }).AsReadOnly(),
                    2),
                new RemovingUnreachableStatesPostReport(
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s4State,s7State,s9State,s12State,s14State,s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s4State,s7State,s9State,s12State,s14State,s16State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] 
                    {
                        s2State,s7State,s12State
                    }).AsReadOnly(),
                    new SortedSet<Label>(new Label[] { }).AsReadOnly(),
                    3)
            };

            int actualPostReportCount = 0;
            bool onBeginInvoked = false;
            bool onEndInvoked = false;

            StateMachine actualStateMachine = stateMachine.RemoveUnreachableStates(
                tuple =>
                {   
                    Assert.IsFalse(onBeginInvoked);
                    Assert.AreEqual(0, actualPostReportCount);
                    onBeginInvoked = true;
                    actualPostReportCount = OnTuple(tuple, expectedSequence, actualPostReportCount);    
                },
                tuple =>
                {
                    actualPostReportCount = OnTuple(tuple, expectedSequence, actualPostReportCount);    
                },
                tuple =>
                {
                    Assert.IsFalse(onEndInvoked);
                    onEndInvoked = true;
                    actualPostReportCount = OnTuple(tuple, expectedSequence, actualPostReportCount);
                    Assert.AreEqual(expectedSequence.Length, actualPostReportCount);
                }
            );

            Assert.AreEqual(expectedSequence.Length, actualPostReportCount);
            Assert.IsTrue(onBeginInvoked);
            Assert.IsTrue(onEndInvoked);
            CollectionAssert.AreEquivalent(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEquivalent(expectedAlphabet, actualStateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedFinalStates, actualStateMachine.FinalStates);
            CollectionAssert.AreEquivalent(expectedTransitions, actualStateMachine.Transitions);
            Assert.AreEqual(expectedInitialState, actualStateMachine.InitialState);
        }

        private int OnTuple(RemovingUnreachableStatesPostReport tuple, RemovingUnreachableStatesPostReport[] expectedSequence, int actualPostReportProcessedCount)
        {
            Assert.IsTrue(actualPostReportProcessedCount < expectedSequence.Length);

            RemovingUnreachableStatesPostReport current = expectedSequence[actualPostReportProcessedCount];

            Assert.AreEqual(current.Iteration, tuple.Iteration);

            CollectionAssert.AreEqual(current.CurrentApproachedStates, tuple.CurrentApproachedStates);
            CollectionAssert.AreEqual(current.NextApproachedStates, tuple.NextApproachedStates);
            CollectionAssert.AreEqual(current.CurrentReachableStates, tuple.CurrentReachableStates);
            CollectionAssert.AreEqual(current.NextReachableStates, tuple.NextReachableStates);

            return actualPostReportProcessedCount + 1;
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
        public void MinimizeTest()
        {
            Label s1State = new Label(new SingleLabel('S', subIndex: 1));
            Label s2State = new Label(new SingleLabel('S', subIndex: 2));
            Label s3State = new Label(new SingleLabel('S', subIndex: 3));
            Label s4State = new Label(new SingleLabel('S', subIndex: 4));
            Label s5State = new Label(new SingleLabel('S', subIndex: 5));
            Label s6State = new Label(new SingleLabel('S', subIndex: 6));

            Label[] states = new Label[] 
            {
                s1State,
                s2State,
                s3State,
                s4State,
                s5State,
                s6State
            };

            Label initialState = s1State;

            Transition[] transitions = new Transition[] 
            {
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
            };

            Label[] finalStates = new Label[]
            {
                s3State,
                s5State,
                s6State
            };


            Label[] expectedStates = new Label[]
            {
                s1State,
                s2State,
                s3State
            };

            Label expectedInitialState = s1State;

            Label[] expectedFinalStates = new Label[]
            {
                s3State
            };

            Transition[] expectedTransitions = new Transition[]
            {
                new Transition(s1State, 'a', s2State),
                new Transition(s1State, 'c', s2State),
                new Transition(s2State, 'a', s3State),
                new Transition(s2State, 'c', s3State),
                new Transition(s3State, 'a', s3State),
                new Transition(s3State, 'c', s3State)
            };

            char[] expectedAlphabet = new char[] { 'a', 'c' };

            StateMachine stateMachine = new StateMachine(initialState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            SetOfEquivalence[] setsOfEquivalence = new SetOfEquivalence[]
            {
                new SetOfEquivalence(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            s1State,
                            s2State,
                            s4State
                        )
                    )
                ),
                new SetOfEquivalence(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            s3State,
                            s5State,
                            s6State
                        )
                    )
                ),
                new SetOfEquivalence(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            s1State
                        )
                    )
                ),
                new SetOfEquivalence(
                    new SortedSet<Label>(
                        EnumerateHelper.Sequence(
                            s2State,
                            s4State
                        )
                    )
                )
            };

            IReadOnlySet<char>[] charsSet = new IReadOnlySet<char>[]
            {
                new SortedSet<char>(EnumerateHelper.Sequence('a','c')).AsReadOnly()
            };

            SetsOfEquivalencePostReport[] expectedSetsOfEquivalencePostReports = new SetsOfEquivalencePostReport[] 
            {
                new SetsOfEquivalencePostReport(
                    new SetsOfEquivalence(
                        new SortedSet<SetOfEquivalence>(
                            EnumerateHelper.Sequence(
                                setsOfEquivalence[0],
                                setsOfEquivalence[1]
                            )
                        )
                    ),0
                 ),
                new SetsOfEquivalencePostReport(
                    new SetsOfEquivalence(
                        new SortedSet<SetOfEquivalence>(
                            EnumerateHelper.Sequence(
                                setsOfEquivalence[2],
                                setsOfEquivalence[3],
                                setsOfEquivalence[1]                               
                            )
                        )
                    ),1
                 ),
                new SetsOfEquivalencePostReport(
                    new SetsOfEquivalence(
                        new SortedSet<SetOfEquivalence>(
                            EnumerateHelper.Sequence(
                                setsOfEquivalence[2],
                                setsOfEquivalence[3],
                                setsOfEquivalence[1]    
                            )
                        )
                    ),2
                 )
            };

            SetOfEquivalenceTransitionsPostReport[] expectedSetOfEquivalenceTransitionsPostReports = new SetOfEquivalenceTransitionsPostReport[] 
            {
               new SetOfEquivalenceTransitionsPostReport(
                   new List<SetOfEquivalenceTransition>(
                       EnumerateHelper.Sequence(
                            new SetOfEquivalenceTransition(setsOfEquivalence[2],charsSet[0],setsOfEquivalence[0],0),
                            new SetOfEquivalenceTransition(setsOfEquivalence[3],charsSet[0],setsOfEquivalence[1],1),
                            new SetOfEquivalenceTransition(setsOfEquivalence[1],charsSet[0],setsOfEquivalence[1],1)
                       )
                   ).AsReadOnly(),1
               ),
               new SetOfEquivalenceTransitionsPostReport(
                   new List<SetOfEquivalenceTransition>(
                       EnumerateHelper.Sequence(
                            new SetOfEquivalenceTransition(setsOfEquivalence[2],charsSet[0],setsOfEquivalence[3],1),
                            new SetOfEquivalenceTransition(setsOfEquivalence[3],charsSet[0],setsOfEquivalence[1],2),
                            new SetOfEquivalenceTransition(setsOfEquivalence[1],charsSet[0],setsOfEquivalence[1],2)
                       )
                   ).AsReadOnly(),2
               )
            };

            bool onResultInvoked = false;
            int actualSetsOfEquivalencePostReportCount = 0;
            int actualSetOfEquivalenceTransitionsPostReportCount = 0;

            StateMachine actualStateMachine = stateMachine.Minimize(
                setsOfEquivalenceReport =>
                {
                    actualSetsOfEquivalencePostReportCount = 
                        OnSetsOfEquivalnceReport(
                        setsOfEquivalenceReport, 
                        expectedSetsOfEquivalencePostReports, 
                        actualSetsOfEquivalencePostReportCount, 
                        actualSetOfEquivalenceTransitionsPostReportCount);
                },
                setOfEquivalenceTransitionsReport =>
                {
                    actualSetOfEquivalenceTransitionsPostReportCount = 
                        OnSetOfEquivalnceTransitionsReport(
                        setOfEquivalenceTransitionsReport,
                        expectedSetOfEquivalenceTransitionsPostReports, 
                        actualSetsOfEquivalencePostReportCount, 
                        actualSetOfEquivalenceTransitionsPostReportCount);
                },
                setOfEquivalenceResult =>
                {
                    Assert.IsFalse(onResultInvoked);
                    onResultInvoked = true;
                    Assert.AreEqual(expectedSetsOfEquivalencePostReports.Length, actualSetsOfEquivalencePostReportCount);
                    Assert.AreEqual(expectedSetOfEquivalenceTransitionsPostReports.Length, actualSetOfEquivalenceTransitionsPostReportCount);
                    Assert.IsTrue(setOfEquivalenceResult.IsStatesCombined);
                    Assert.AreEqual(expectedSetOfEquivalenceTransitionsPostReports.Length, setOfEquivalenceResult.LastIteration);
                });

            Assert.IsTrue(onResultInvoked);
            Assert.AreEqual(expectedSetsOfEquivalencePostReports.Length, actualSetsOfEquivalencePostReportCount);
            Assert.AreEqual(expectedSetOfEquivalenceTransitionsPostReports.Length, actualSetOfEquivalenceTransitionsPostReportCount);

            CollectionAssert.AreEquivalent(expectedAlphabet, actualStateMachine.Alphabet);
            CollectionAssert.AreEquivalent(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEquivalent(expectedFinalStates, actualStateMachine.FinalStates);
            CollectionAssert.AreEquivalent(expectedTransitions, actualStateMachine.Transitions);
            Assert.AreEqual(expectedInitialState, actualStateMachine.InitialState);
        }

        private int OnSetsOfEquivalnceReport(
            SetsOfEquivalencePostReport actualReport, SetsOfEquivalencePostReport[] expectedSequence, 
            int actualSetsOfEquivalencePostReportCount, int actualSetOfEquivalenceTransitionsPostReportCount)
        {
            Assert.IsTrue(actualSetsOfEquivalencePostReportCount < expectedSequence.Length);

            SetsOfEquivalencePostReport expected = expectedSequence[actualSetsOfEquivalencePostReportCount];

            Assert.AreEqual(expected.Iteration, actualReport.Iteration);

            Assert.AreEqual(expected.SetsOfEquivalence, actualReport.SetsOfEquivalence);

            Assert.AreEqual(actualSetsOfEquivalencePostReportCount, actualSetOfEquivalenceTransitionsPostReportCount);
            return actualSetsOfEquivalencePostReportCount + 1;
        }

        private int OnSetOfEquivalnceTransitionsReport(SetOfEquivalenceTransitionsPostReport actualReport, SetOfEquivalenceTransitionsPostReport[] expectedSequence,
            int actualSetsOfEquivalencePostReportCount, int actualSetOfEquivalenceTransitionsPostReportCount)
        {
            Assert.IsTrue(actualSetOfEquivalenceTransitionsPostReportCount < expectedSequence.Length);

            SetOfEquivalenceTransitionsPostReport expected = expectedSequence[actualSetOfEquivalenceTransitionsPostReportCount];

            Assert.AreEqual(expected.Iteration, actualReport.Iteration);

            Assert.AreEqual(expected.SetOfEquivalenceTransitions.Count, actualReport.SetOfEquivalenceTransitions.Count);

            IEnumerator<SetOfEquivalenceTransition> currentSetOfEquivalenceTransitionsEnumerator = expected.SetOfEquivalenceTransitions.GetEnumerator();
            IEnumerator<SetOfEquivalenceTransition> actualSetOfEquivalenceTransitionsEnumerator = actualReport.SetOfEquivalenceTransitions.GetEnumerator();

            while (currentSetOfEquivalenceTransitionsEnumerator.MoveNext() && actualSetOfEquivalenceTransitionsEnumerator.MoveNext())
            {
                Assert.AreEqual(currentSetOfEquivalenceTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence,
                    actualSetOfEquivalenceTransitionsEnumerator.Current.IndexOfCurrentSetOfEquivalence);

                CollectionAssert.AreEqual(
                    currentSetOfEquivalenceTransitionsEnumerator.Current.Symbols,
                    actualSetOfEquivalenceTransitionsEnumerator.Current.Symbols);

                Assert.AreEqual(currentSetOfEquivalenceTransitionsEnumerator.Current.CurrentSetOfEquivalence,
                    actualSetOfEquivalenceTransitionsEnumerator.Current.CurrentSetOfEquivalence);

                Assert.AreEqual(currentSetOfEquivalenceTransitionsEnumerator.Current.NextSetOfEquivalence,
                    actualSetOfEquivalenceTransitionsEnumerator.Current.NextSetOfEquivalence);
            }

            Assert.AreEqual(actualSetsOfEquivalencePostReportCount, actualSetOfEquivalenceTransitionsPostReportCount + 1);
            return actualSetOfEquivalenceTransitionsPostReportCount + 1;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_InitialStateNull_Fail()
        {
            ISet<Transition> transitions = new HashSet<Transition>(new Transition[] { new Transition(new Label(new SingleLabel('c')),'a', new Label(new SingleLabel('b'))) });
            ISet<Label> finalStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('b')) });

            new StateMachine(null, finalStates, transitions);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_FinalStatesNull_Fail()
        {
            ISet<Transition> transitions = new HashSet<Transition>(new Transition[] { new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))) });
            Label initialState = new Label(new SingleLabel('c'));

            new StateMachine(initialState, null, transitions);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CctorTest_TransitionsNull_Fail()
        {
            ISet<Label> finalStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('b')) });
            Label initialState = new Label(new SingleLabel('c'));

            new StateMachine(initialState, finalStates, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_OneFinalStateNull_Fail()
        {
            ISet<Transition> transitions = new HashSet<Transition>(new Transition[] { new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))) });
            ISet<Label> finalStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('b')), null });
            Label initialState = new Label(new SingleLabel('c'));

            new StateMachine(initialState, finalStates, transitions);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_OneTransitionNull_Fail()
        {
            ISet<Transition> transitions = new HashSet<Transition>(new Transition[] { new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))), null });
            ISet<Label> finalStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('b'))});
            Label initialState = new Label(new SingleLabel('c'));

            new StateMachine(initialState, finalStates, transitions);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_FinalStatesSetIsNotSupersetOfStates_Fail()
        {
            ISet<Transition> transitions = new HashSet<Transition>(new Transition[] { new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))) });
            ISet<Label> finalStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('b')), new Label(new SingleLabel('d')) });
            Label initialState = new Label(new SingleLabel('c'));

            new StateMachine(initialState, finalStates, transitions);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CctorTest_InitialStateDoesNotBelongToStates_Fail()
        {
            ISet<Transition> transitions = new HashSet<Transition>(new Transition[] { new Transition(new Label(new SingleLabel('c')), 'a', new Label(new SingleLabel('b'))) });
            ISet<Label> finalStates = new HashSet<Label>(new Label[] { new Label(new SingleLabel('b')) });
            Label initialState = new Label(new SingleLabel('d'));

            new StateMachine(initialState, finalStates, transitions);
        }

        [Test]
        public void CctorTest_Ok()
        {
            Label sState = new Label(new SingleLabel('S'));
            Label dState = new Label(new SingleLabel('D'));
            Label pState = new Label(new SingleLabel('P'));
            Label kState = new Label(new SingleLabel('K'));
            Label aState = new Label(new SingleLabel('A'));

            Label[] expectedStates = new Label[]
            {
                aState,
                dState,
                kState,
                pState,
                sState
            };

            char[] expectedAlphabet = new char[]
            {
                'a',
                'b',
                'c',
                'd'
            };

            Label expectedInitialState = kState;

            Transition s_a_kTranstition = new Transition(sState, 'a', kState);
            Transition k_b_pTranstition = new Transition(kState, 'b', pState);
            Transition p_c_aTranstition = new Transition(pState, 'c', aState);
            Transition a_d_dTranstition = new Transition(aState, 'd', dState);

            Transition[] expectedTransitions = new Transition[]
            {
                a_d_dTranstition,
                k_b_pTranstition,
                p_c_aTranstition,
                s_a_kTranstition
            };

            Label[] expectedfinalStates = new Label[]
            {
                kState,
                pState
            };

            ISet<Transition> transitions = new HashSet<Transition>(new Transition[]
            {
                s_a_kTranstition,
                k_b_pTranstition,
                p_c_aTranstition,
                a_d_dTranstition
            });

            ISet<Label> finalStates = new HashSet<Label>(new Label[]
            {
                pState,
                kState
            });

            StateMachine stateMachine = new StateMachine(expectedInitialState, finalStates, transitions);

            CollectionAssert.AreEqual(expectedAlphabet, stateMachine.Alphabet);
            CollectionAssert.AreEqual(expectedStates, stateMachine.States);
            CollectionAssert.AreEqual(expectedTransitions, stateMachine.Transitions);
            CollectionAssert.AreEqual(expectedfinalStates, stateMachine.FinalStates);
            Assert.AreEqual(expectedInitialState, stateMachine.InitialState);
        }
    }
}
