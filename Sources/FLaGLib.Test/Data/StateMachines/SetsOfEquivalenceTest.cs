using FLaGLib.Data;
using FLaGLib.Data.StateMachines;
using FLaGLib.Helpers;
using FLaGLib.Test.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FLaGLib.Test.Data.StateMachines
{
    [TestFixture]
    public class SetsOfEquivalenceTest
    {
        private Tuple<SetsOfEquivalence, SetsOfEquivalence, int>[] _Expectations = new Tuple<SetsOfEquivalence, SetsOfEquivalence, int>[]
        {
            new Tuple<SetsOfEquivalence,SetsOfEquivalence,int>(null, null, 0),
            new Tuple<SetsOfEquivalence,SetsOfEquivalence,int>(null, 
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('P')),
                                null,
                                new Label(new SingleLabel('D'))
                            )                            
                        ),
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('S')),
                                null,
                                new Label(new SingleLabel('M'))
                            )                            
                        )
                    )        
                ),
            -1),
            new Tuple<SetsOfEquivalence,SetsOfEquivalence,int>(
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('P')),
                                null,
                                new Label(new SingleLabel('D'))
                            )                            
                        ),
                        null,
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('S')),
                                null,
                                new Label(new SingleLabel('M'))
                            )
                        )                        
                    )     
                ), 
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('P')),
                                null,
                                new Label(new SingleLabel('D'))
                            )                            
                        ),
                        null,
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('S')),
                                null,
                                new Label(new SingleLabel('M'))
                            )
                        )   
                    )                    
                ),
            0),
            new Tuple<SetsOfEquivalence,SetsOfEquivalence,int>(
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('P')),
                                null,
                                new Label(new SingleLabel('D'))
                            )                            
                        ),
                        null,
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('S')),
                                null,
                                new Label(new SingleLabel('M'))
                            )
                        )                        
                    )                    
                ), 
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('K')),
                                null,
                                new Label(new SingleLabel('D'))
                            )                            
                        ),
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('S')),
                                null,
                                new Label(new SingleLabel('M'))
                            )
                        )
                    )
                ),
            -1),
            new Tuple<SetsOfEquivalence,SetsOfEquivalence,int>(
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('P')),
                                null,
                                new Label(new SingleLabel('D'))
                            )                            
                        ),
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('S')),
                                null,
                                new Label(new SingleLabel('M'))
                            )                            
                        )
                    )                    
                ), 
                new SetsOfEquivalence(
                    EnumerateHelper.Sequence(
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('K')),
                                null,
                                new Label(new SingleLabel('D'))
                            )                            
                        ),
                        new SetOfEquivalence(
                            EnumerateHelper.Sequence(
                                new Label(new SingleLabel('S')),
                                null,
                                new Label(new SingleLabel('M'))
                            )
                        )                        
                    )                    
                ),
            5)
        };

        [Test]
        public void CctorTest_Ok()
        {
            IEnumerable<SetOfEquivalence> setsOfEquivalenceExpected =
                EnumerateHelper.Sequence(
                   new SetOfEquivalence
                   (
                       new SortedSet<Label>(EnumerateHelper.Sequence(
                           new Label(new SingleLabel('P')),
                           null,
                           new Label(new SingleLabel('D'))
                           )
                       )
                   ),
                   null,
                   new SetOfEquivalence
                   (
                       new SortedSet<Label>(EnumerateHelper.Sequence(
                           new Label(new SingleLabel('S')),
                           new Label(new SingleLabel('M'))
                           )
                       )
                   )
                );

            SetsOfEquivalence setsOfEquivalence = new SetsOfEquivalence(setsOfEquivalenceExpected);

            CollectionAssert.AreEquivalent(setsOfEquivalenceExpected, setsOfEquivalence);
        }

        [Test]
        public void CctorTest_SetNull_Fail()
        {
            Assert.Throws<ArgumentNullException>(() => new SetsOfEquivalence(null));
        }

        [Test]
        public void EqualsTest()
        {
            ComparableEquatableHelper.TestEquals(_Expectations);
        }

        [Test]
        public void CompareTest()
        {
            ComparableEquatableHelper.TestCompare(_Expectations);
        }
    }
}
