using System;
using Code.Utilz;
using Xunit;

namespace Tests.Utilz
{
  public class MatrixHelpersTests
  {
    [Theory]
    [InlineData("18-14", 2)]
    [InlineData("18-14", 3)]
    [InlineData("18-14", 6)]
    public void SplitJoin(string input, int block_size)
    {
      // Arrange
      var (original, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");

      var matrix = new long[size * size];
      Array.Copy(original, matrix, size * size);

      // Act: convert input matrix to block matrix
      MatrixHelpers.SplitInPlace(matrix, size, block_size, out var block_count);

      // Act: now convert block matrix back to matrix
      MatrixHelpers.JoinInPlace(matrix, block_count, block_size, out _);

      // Assert
      Assert.Equal(original, matrix);
    }
  }
}
