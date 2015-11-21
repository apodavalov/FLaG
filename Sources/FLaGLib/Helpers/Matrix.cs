using FLaGLib.Data.Grammars;
using FLaGLib.Data.RegExps;
using FLaGLib.Extensions;
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
            throw new NotImplementedException();
        }
    }
}
