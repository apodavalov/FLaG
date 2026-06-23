using System.Collections.Immutable;
using FLaG.Core.Data.Grammars;
using FLaG.Core.Data.RegExps;

namespace FLaG.Core.Helpers
{
    public class Matrix
    {
        private Expression?[,] _Matrix;
        private NonTerminalSymbol[] _NonTerminals;

        public Expression? this[int i, int j] => _Matrix[i, j];

        public int RowCount => _Matrix.GetLength(0);

        public int ColumnCount => _Matrix.GetLength(1);

        public IImmutableList<NonTerminalSymbol> NonTerminals { get; }

        public GrammarType GrammarType { get; }

        public Matrix(
            Expression?[,] matrix,
            IEnumerable<NonTerminalSymbol> nonTerminals,
            GrammarType grammarType
        )
        {
            _NonTerminals = nonTerminals.ToArray();
            _Matrix = matrix;
            GrammarType = grammarType;

            if (
                _NonTerminals.Length != _Matrix.GetLength(0)
                || _NonTerminals.Length != _Matrix.GetLength(1) - 1
            )
            {
                throw new InvalidOperationException(
                    "Non terminals collection length is not equal either matrix rows count or matrix column count minus 1."
                );
            }

            for (int i = 0; i < _Matrix.GetLength(0); ++i)
            {
                bool allNulls = true;

                for (int j = 0; j < _Matrix.GetLength(1); ++j)
                {
                    allNulls &= _Matrix[i, j] is null;
                }

                if (allNulls)
                {
                    throw new InvalidOperationException("At least one equation has no elements.");
                }
            }

            NonTerminals = _NonTerminals.ToImmutableList();
        }

        internal Expression Solve(
            int row,
            Action<Matrix>? onBegin = null,
            Action<Matrix>? onIterate = null
        )
        {
            onBegin?.Invoke(this);

            for (int i = 0; i < _Matrix.GetLength(0); ++i)
            {
                if (SimpleIterate(i, true))
                {
                    onIterate?.Invoke(this);
                }

                if (AlphaBetaIterate(i))
                {
                    onIterate?.Invoke(this);
                }
            }

            for (int i = _Matrix.GetLength(0) - 1; i >= 0; --i)
            {
                if (SimpleIterate(i, false))
                {
                    onIterate?.Invoke(this);
                }

                if (AlphaBetaIterate(i))
                {
                    onIterate?.Invoke(this);
                }
            }

            return _Matrix[row, _Matrix.GetLength(1) - 1]
                ?? throw new InvalidOperationException("Expected non null expression.");
        }

        private bool AlphaBetaIterate(int row)
        {
            Expression? expression = _Matrix[row, row];

            if (expression is null)
            {
                return false;
            }

            _Matrix[row, row] = null;
            bool expressionConsumed = false;

            for (int i = 0; i < _Matrix.GetLength(1); ++i)
            {
                Expression? rowI = _Matrix[row, i];
                if (rowI is not null)
                {
                    _Matrix[row, i] = new Concat(
                        Enumerate(rowI, new Iteration(expression, false))
                    ).Optimize();
                    expressionConsumed = true;
                }
            }

            if (!expressionConsumed)
            {
                _Matrix[row, _Matrix.GetLength(1) - 1] = new Iteration(expression, true);
            }

            return true;
        }

        private IEnumerable<Expression> Enumerate(params Expression[] expressions) =>
            GrammarDispatcher.Dispatch(GrammarType, () => expressions, () => expressions.Reverse());

        private bool SimpleIterate(int row, bool isDirect)
        {
            bool result = false;

            int first = isDirect ? 0 : row + 1;
            int last = isDirect ? row - 1 : _Matrix.GetLength(0) - 1;

            for (int i = first; i <= last; ++i)
            {
                Expression? rowI = _Matrix[row, i];
                if (rowI is not null)
                {
                    result = true;
                    Expression expression = rowI;
                    _Matrix[row, i] = null;
                    bool expressionConsumed = false;

                    for (int j = 0; j < _Matrix.GetLength(1); ++j)
                    {
                        Expression? iJ = _Matrix[i, j];
                        if (iJ is not null)
                        {
                            Concat concat = new(Enumerate(iJ, expression));
                            Expression? rowJ = _Matrix[row, j];
                            if (rowJ is not null)
                            {
                                Union union = new([rowJ, concat]);
                                _Matrix[row, j] = union.Optimize();
                            }
                            else
                            {
                                _Matrix[row, j] = concat.Optimize();
                            }
                            expressionConsumed = true;
                        }
                    }
                    if (!expressionConsumed)
                    {
                        throw new InvalidOperationException("The expression was not consumed.");
                    }
                }
            }

            return result;
        }
    }
}
