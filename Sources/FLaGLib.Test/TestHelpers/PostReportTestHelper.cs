using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Test.TestHelpers
{
    class PostReportTestHelper<B, I>
    {
        protected int _ActualIterationPostReportCount;
        protected bool _BeginPostReportInvoked;

        protected B _ExpectedBeginPostReport;
        protected IReadOnlyList<I> _ExpectedIterationPostReports;
        protected Action<B, B> _BeginPostReportChecker;
        protected Action<I, I> _IterationPostReportChecker;

        public virtual void OnBeginPostReport(B actualBeginPostReport)
        {
            Assert.IsFalse(_BeginPostReportInvoked);
            Assert.AreEqual(0, _ActualIterationPostReportCount);
            _BeginPostReportInvoked = true;
            BeginPostReportChecker(_ExpectedBeginPostReport, actualBeginPostReport);
        }

        public virtual void OnIterationPostReport(I actualIterationPostReport)
        {
            Assert.IsTrue(_BeginPostReportInvoked);
            Assert.IsTrue(_ActualIterationPostReportCount < _ExpectedIterationPostReports.Count);
            _ActualIterationPostReportCount++;
            IterationPostReportChecker(_ExpectedIterationPostReports[_ActualIterationPostReportCount-1], actualIterationPostReport);
        }

        private void IterationPostReportChecker(I expected, I actual)
        {
            CheckForNulls(expected, actual);
            _IterationPostReportChecker(expected, actual);
        }

        private void BeginPostReportChecker(B expected, B actual)
        {
            CheckForNulls(expected, actual);
            _BeginPostReportChecker(expected, actual);
        }

        protected void CheckForNulls<T>(T expected, T actual)
        {
            if (expected == null && actual == null)
            {
                return;
            }

            if (expected == null)
            {
                Assert.Fail("Expected null value, but non-null was returned.");
            }

            if (actual == null)
            {
                Assert.Fail("Expected non-null value, but null was returned.");
            }
        }

        public virtual void StartTest()
        {
            _ActualIterationPostReportCount = 0;
            _BeginPostReportInvoked = false;
        }

        public virtual void FinishTest()
        {
            Assert.IsTrue(_BeginPostReportInvoked);
            Assert.AreEqual(_ExpectedIterationPostReports.Count, _ActualIterationPostReportCount);
        }

        public PostReportTestHelper(
            B expectedBeginPostReport,
            IEnumerable<I> expectedIterationPostReports,
            Action<B, B> beginPostReportChecker,
            Action<I, I> iterationPostReportChecker)
        {
            if (expectedBeginPostReport == null)
            {
                throw new ArgumentNullException(nameof(expectedBeginPostReport));
            }

            if (expectedIterationPostReports == null)
            {
                throw new ArgumentNullException(nameof(expectedIterationPostReports));
            }

            if (beginPostReportChecker == null)
            {
                throw new ArgumentNullException(nameof(beginPostReportChecker));
            }

            if (iterationPostReportChecker == null)
            {
                throw new ArgumentNullException(nameof(iterationPostReportChecker));
            }

            _ExpectedBeginPostReport = expectedBeginPostReport;
            _ExpectedIterationPostReports = expectedIterationPostReports.ToList().AsReadOnly();
            _BeginPostReportChecker = beginPostReportChecker;
            _IterationPostReportChecker = iterationPostReportChecker;
        }
    }

    class PostReportTestHelper<B,I,E> : PostReportTestHelper<B,I>
    {
        protected bool _EndPostReportInvoked;
        protected E _ExpectedEndPostReport;
        protected Action<E, E> _EndPostReportChecker;

        public override void OnBeginPostReport(B actualBeginPostReport)
        {
            Assert.IsFalse(_EndPostReportInvoked);
            base.OnBeginPostReport(actualBeginPostReport);
        }

        public override void OnIterationPostReport(I actualIterationPostReport)
        {
            Assert.IsFalse(_EndPostReportInvoked);
            base.OnIterationPostReport(actualIterationPostReport);
        }

        public virtual void OnEndPostReport(E actualEndPostReport)
        {
            Assert.IsTrue(_BeginPostReportInvoked);
            Assert.IsFalse(_EndPostReportInvoked);
            Assert.AreEqual(_ExpectedIterationPostReports.Count, _ActualIterationPostReportCount);
            _EndPostReportInvoked = true;
            EndPostReportChecker(_ExpectedEndPostReport, actualEndPostReport);
        }

        private void EndPostReportChecker(E expected, E actual)
        {
            CheckForNulls(expected, actual);
            _EndPostReportChecker(expected, actual);
        }

        public override void StartTest()
        {
            _EndPostReportInvoked = false;
            base.StartTest();
        }

        public override void FinishTest()
        {
            Assert.IsTrue(_EndPostReportInvoked);
            base.FinishTest();
        }

        public PostReportTestHelper(
            B expectedBeginPostReport, 
            IEnumerable<I> expectedIterationPostReports, 
            E expectedEndPostReport,
            Action<B,B> beginPostReportChecker,
            Action<I,I> iterationPostReportChecker,
            Action<E,E> endPostReportChecker)
            : base(
                  expectedBeginPostReport,
                  expectedIterationPostReports,
                  beginPostReportChecker,
                  iterationPostReportChecker)
        {
            if (expectedEndPostReport == null)
            {
                throw new ArgumentNullException(nameof(expectedEndPostReport));
            }

            if (endPostReportChecker == null)
            {
                throw new ArgumentNullException(nameof(endPostReportChecker));
            }

            _ExpectedEndPostReport = expectedEndPostReport;
            _EndPostReportChecker = endPostReportChecker;
        }
    }
}
