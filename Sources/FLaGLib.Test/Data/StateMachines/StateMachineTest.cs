using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using NUnit.Framework;
using System;
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

            Tuple<StateMachine, IReadOnlyDictionary<Label, Label>> actualTuple = stateMachine.Reorganize();

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

            CollectionAssert.AreEqual(stateMachine.Alphabet, actualTuple.Item1.Alphabet);
            CollectionAssert.AreEqual(expectedStates, actualTuple.Item1.States);
            CollectionAssert.AreEqual(expectedTransitions, actualTuple.Item1.Transitions);
            CollectionAssert.AreEqual(expectedFinalStates, actualTuple.Item1.FinalStates);
            CollectionAssert.AreEquivalent(expectedDictionary, actualTuple.Item2);
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

            StateMachine stateMachine = new StateMachine(initialState, new HashSet<Label>(finalStates), new HashSet<Transition>(transitions));

            StateMachine actualStateMachine = stateMachine.ConvertToDeterministicIfNot();

            CollectionAssert.AreEquivalent(expectedStates, actualStateMachine.States);
            CollectionAssert.AreEquivalent(expectedFinalStates, actualStateMachine.FinalStates);
            CollectionAssert.AreEquivalent(expectedTransitions, actualStateMachine.Transitions);
            Assert.AreEqual(expectedInitialState, actualStateMachine.InitialState);
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
        public void Cctor_Ok()
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
