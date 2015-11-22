using FLaGLib.Data.Grammars;
using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
using FLaGLib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FLaGLib.Data.Helpers
{
    public class Matrix
    {
        private Expression[,] _Matrix;
        private NonTerminalSymbol[] _NonTerminals;

        public Expression this[int i,int j]
        {
            get
            {
                return _Matrix[i, j];
            }
        }

        public int RowCount
        {
            get
            {
                return _Matrix.GetLength(0);
            }
        }

        public int ColumnCount
        {
            get
            {
                return _Matrix.GetLength(1);
            }
        }

        public IReadOnlyList<NonTerminalSymbol> NonTerminals
        {
            get;
            private set;
        }

        public GrammarType GrammarType
        {
            get;
            private set;
        }

        public Matrix(Expression[,] matrix, IEnumerable<NonTerminalSymbol> nonTerminals, GrammarType grammarType)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (nonTerminals == null)
            {
                throw new ArgumentNullException(nameof(nonTerminals));
            }

            if (nonTerminals.AnyNull())
            {
                throw new ArgumentException("Collection contains null elements.", nameof(nonTerminals));
            }

            _NonTerminals = nonTerminals.ToArray();
            _Matrix = matrix;
            GrammarType = grammarType;

            if (_NonTerminals.Length != _Matrix.GetLength(0) || _NonTerminals.Length != _Matrix.GetLength(1) - 1)
            {
                throw new InvalidOperationException("Non terminals collection length is not equal either matrix rows count or matrix column count minus 1.");
            }

            for (int i = 0; i < _Matrix.GetLength(0); i++)
            {
                bool allNulls = true;

                for (int j = 0; j < _Matrix.GetLength(1); j++)
                {
                    allNulls &= _Matrix[i, j] == null;
                }

                if (allNulls)
                {
                    throw new InvalidOperationException("At least one equation has no elements.");
                }
            }

            NonTerminals = _NonTerminals.AsReadOnly();
        }

        internal Expression Solve(int row, Action<Matrix> onBegin = null, Action<Matrix> onIterate = null)
        {
            if (onBegin != null)
            {
                onBegin(this);
            }

            for (int i = 0; i < _Matrix.GetLength(0); i++)
            {
                if (SimpleIterate(i,true))
                {
                    if (onIterate != null)
                    {
                        onIterate(this);
                    }
                }

                if (AlphaBetaIterate(i))
                {
                    if (onIterate != null)
                    {
                        onIterate(this);
                    }
                }
            }

            for (int i = _Matrix.GetLength(0) - 1; i >= 0; i--)
            {
                if (SimpleIterate(i, false))
                {
                    if (onIterate != null)
                    {
                        onIterate(this);
                    }
                }

                if (AlphaBetaIterate(i))
                {
                    if (onIterate != null)
                    {
                        onIterate(this);
                    }
                }
            }

            return _Matrix[row, _Matrix.GetLength(1) - 1];
        }

        private bool AlphaBetaIterate(int row)
        {
            Expression expression = _Matrix[row, row];

            if (expression == null)
            {
                return false;
            }

            _Matrix[row, row] = null;

            for (int i = 0; i < _Matrix.GetLength(1); i++)
            {
                if (_Matrix[row, i] != null)
                {
                    _Matrix[row, i] = new Concat(Enumerate(_Matrix[row, i], new Iteration(expression, false))).Optimize();
                }
            }

            return true;
        }

        private IEnumerable<Expression> Enumerate(params Expression[] expressions)
        {
            switch (GrammarType)
            {
                case GrammarType.Left:
                    return EnumerateHelper.Sequence(expressions);
                case GrammarType.Right:
                    return EnumerateHelper.ReverseSequence(expressions);
                default:
                    throw new InvalidOperationException(Grammar.GrammarIsNotSupportedMessage);
            }
        }

        private bool SimpleIterate(int row, bool isDirect)
        {
            bool result = false;

            int first = isDirect ? 0 : row + 1;
            int last = isDirect ? row - 1 : _Matrix.GetLength(0) - 1;

            for (int i = first; i <= last; i++)
            {
                if (_Matrix[row, i] != null)
                {
                    result = true;

                    Expression expression = _Matrix[row, i];
                    _Matrix[row, i] = null;

                    for (int j = 0; j < _Matrix.GetLength(1); j++)
                    {
                        if (_Matrix[i, j] != null)
                        {
                            Concat concat = new Concat(Enumerate(_Matrix[i, j], expression));

                            if (_Matrix[row, j] != null)
                            {
                                Union union = new Union(EnumerateHelper.Sequence(_Matrix[row, j], concat));

                                _Matrix[row, j] = union.Optimize();
                            }
                            else
                            {
                                _Matrix[row, j] = concat.Optimize();
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
