using System;

using Xunit;

namespace Problems.Tests
{
    public class AlgorithmsTests
    {
        private const int NO_EDGE = (int.MaxValue / 2) - 1;
        private const int MATRIX_SZ = 18;

        private (int f, int t, int v)[] input = new []
        {
            (1, 2, 9),
            (1, 3, 2),
            (1, 6, 5),
            (3, 4, 3),
            (3, 6, 6),
            (4, 2, 1),
            (4, 6, 4),
            (11, 12, 9),
            (11, 13, 2),
            (11, 16, 5),
            (13, 14, 3),
            (13, 16, 6),
            (14, 12, 1),
            (14, 16, 4)
        };
        private (int f, int t, int v)[] result = new []
        {
            (1, 2, 6),
            (1, 3, 2),
            (1, 4, 5),
            (1, 6, 5),
            (3, 2, 4),
            (3, 4, 3),
            (3, 6, 6),
            (4, 2, 1),
            (4, 6, 4),
            (11, 12, 6),
            (11, 13, 2),
            (11, 14, 5),
            (11, 16, 5),
            (13, 12, 4),
            (13, 14, 3),
            (13, 16, 6),
            (14, 12, 1),
            (14, 16, 4)
        };

        [Theory]
        [InlineData("00")]
        [InlineData("01")]
        [InlineData("02")]
        [InlineData("03")]
        [InlineData("04")]
        public void Algorithms(string method)
        {
            var sz     = MATRIX_SZ;
            var matrix = new int[sz * sz];
            var result = new int[sz * sz];

            Array.Fill(matrix, NO_EDGE);
            Array.Fill(result, NO_EDGE);

            foreach (var (f, t, v) in this.input)
            {
                matrix[f * sz + t] = v;
            }
            foreach (var (f, t, v) in this.result)
            {
                result[f * sz + t] = v;
            }

            switch (method)
            {
                case "00": new Algorithms().FloydWarshall_00(matrix, sz); break;
                case "01": new Algorithms().FloydWarshall_01(matrix, sz); break;
                case "02": new Algorithms().FloydWarshall_02(matrix, sz); break;
                case "03": new Algorithms().FloydWarshall_03(matrix, sz); break;
                case "04": new Algorithms().FloydWarshall_04(matrix, sz); break;
            }

            for (var i = 0; i < sz * sz; ++i)
            {
                Assert.Equal(result[i], matrix[i]);
            }
        }
    }
}
