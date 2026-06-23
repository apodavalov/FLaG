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

            GrammarDispatcher.Dispatch(
                GrammarType,
                () =>
                {
                    AscendingIteration(onIterate);
                    DescendingIteration(onIterate);
                },
                () =>
                {
                    DescendingIteration(onIterate);
                    AscendingIteration(onIterate);
                }
            );

            return _Matrix[row, _Matrix.GetLength(1) - 1]
                ?? throw new InvalidOperationException("Expected non null expression.");
        }

        private void AscendingIteration(Action<Matrix>? onIterate)
        {
            for (int i = 0; i < _Matrix.GetLength(0); ++i)
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
        }

        private void DescendingIteration(Action<Matrix>? onIterate)
        {
            for (int i = _Matrix.GetLength(0) - 1; i >= 0; --i)
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
                expressionConsumed |= AlphaBeta(row, i, expression);
            }

            if (!expressionConsumed)
            {
                _Matrix[row, _Matrix.GetLength(1) - 1] = new Iteration(expression, false);
            }

            return true;
        }

        private bool AlphaBeta(int row, int column, Expression expression)
        {
            Expression? rowColumn = _Matrix[row, column];
            if (rowColumn is null)
            {
                return false;
            }
            _Matrix[row, column] = new Concat(
                Enumerate(rowColumn, new Iteration(expression, false))
            ).Optimize();
            return true;
        }

        private IEnumerable<Expression> Enumerate(params Expression[] expressions) =>
            GrammarDispatcher.Dispatch(GrammarType, () => expressions, () => expressions.Reverse());

        private bool SimpleIterate(int row, bool reverse)
        {
            bool result = false;

            if (reverse)
            {
                for (int i = _Matrix.GetLength(1) - 2; i > row; --i)
                {
                    result |= Simple(row, i);
                }
            }
            else
            {
                for (int i = 0; i < row; ++i)
                {
                    result |= Simple(row, i);
                }
            }

            return result;
        }

        private bool Simple(int row, int column)
        {
            Expression? rowColumn = _Matrix[row, column];

            if (rowColumn is null)
            {
                return false;
            }

            Expression expression = rowColumn;
            _Matrix[row, column] = null;
            bool expressionConsumed = false;

            for (int j = 0; j < _Matrix.GetLength(1); ++j)
            {
                Expression? iJ = _Matrix[column, j];
                if (iJ is null)
                {
                    continue;
                }

                Concat concat = new(Enumerate(iJ, expression));
                Expression? rowJ = _Matrix[row, j];

                if (rowJ is null)
                {
                    _Matrix[row, j] = concat.Optimize();
                }
                else
                {
                    Union union = new([rowJ, concat]);
                    _Matrix[row, j] = union.Optimize();
                }
                expressionConsumed = true;
            }

            if (!expressionConsumed)
            {
                throw new InvalidOperationException("The expression was not consumed.");
            }

            return true;
        }
    }
}
