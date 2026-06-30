namespace FLaG.IO.Out
{
    public sealed class ReadOnlyGraphMatrix(GraphMatrix graphMatrix)
    {
        private GraphMatrix _GraphMatrix = graphMatrix;

        public bool Get(int i, int j) => _GraphMatrix.Get(i, j);

        public int VertexCount => _GraphMatrix.VertexCount;
    }

    public sealed class GraphMatrix
    {
        private bool[][] _Matrix;

        private int _VertexCount;

        public int VertexCount => _VertexCount;

        public GraphMatrix(int vertexCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(vertexCount, nameof(vertexCount));
            _VertexCount = vertexCount;
            if (_VertexCount == 0)
            {
                _Matrix = [];
                return;
            }

            _Matrix = new bool[_VertexCount - 1][];
            for (int i = 0; i < _VertexCount - 1; ++i)
            {
                _Matrix[i] = new bool[i + 1];
                for (int j = 0; j < _Matrix[i].Length; ++j)
                {
                    _Matrix[i][j] = false;
                }
            }
        }

        public void Set(int i, int j, bool value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(i, nameof(i));
            ArgumentOutOfRangeException.ThrowIfNegative(j, nameof(j));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(i, _VertexCount, nameof(i));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(j, _VertexCount, nameof(j));
            if (i == j)
            {
                return;
            }
            if (i < j)
            {
                (i, j) = (j, i);
            }
            _Matrix[i - 1][j] = value;
        }

        public bool Get(int i, int j)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(i, nameof(i));
            ArgumentOutOfRangeException.ThrowIfNegative(j, nameof(j));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(i, _VertexCount, nameof(i));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(j, _VertexCount, nameof(j));
            if (i == j)
            {
                return false;
            }
            if (i < j)
            {
                (i, j) = (j, i);
            }
            return _Matrix[i - 1][j];
        }
    }
}
