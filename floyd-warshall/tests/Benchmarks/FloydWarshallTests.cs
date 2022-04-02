using System;
using Code.Benchmarks;
using Code.Utilz;
using Xunit;

namespace Tests.Benchmarks
{
    public class FloydWarshallTests
    {
        [Theory]
        [InlineData("00")]
        [InlineData("01")]
        [InlineData("02")]
        [InlineData("03")]
        [InlineData("04")]
        public void Variants(string variant)
        {
            var inputs = new[]
            {
                "18-14"
            };
            foreach (var input in inputs)
            {
                var (matrix, size) = MatrixHelpers.FromInputFile($@"{Environment.CurrentDirectory}/Data/{input}.input");
                var (result, _) = MatrixHelpers.FromInputFile($@"{Environment.CurrentDirectory}/Data/{input}.input.result");

                Assert.Equal(matrix.Length, result.Length);

                switch (variant)
                {
                    case "00": new FloydWarshall().Variant_00(matrix, size); break;
                    case "01": new FloydWarshall().Variant_01(matrix, size); break;
                    case "02": new FloydWarshall().Variant_02(matrix, size); break;
                    case "03": new FloydWarshall().Variant_03(matrix, size); break;
                    case "04": new FloydWarshall().Variant_04(matrix, size); break;
                }

                for (var i = 0; i < matrix.Length; ++i)
                {
                    Assert.Equal(result[i], matrix[i]);
                }
            }
        }
    }
}
