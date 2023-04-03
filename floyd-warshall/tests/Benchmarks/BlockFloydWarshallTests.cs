using System;
using Code.Benchmarks;
using Code.Utilz;
using Xunit;

namespace Tests.Benchmarks
{
  public class BlockFloydWarshallTests
  {
    [Theory]
    [InlineData("18-14", 2)]
    [InlineData("18-14", 3)]
    [InlineData("18-14", 6)]
    public void Convertion(string input, int block_size)
    {
      // Arrange
      var (matrix, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");

      // Act: convert input matrix to block matrix
      var (block_matrix, block_count, _) = BlockMatrixHelpers.ConvertMatrixToBlockMatrix(matrix, size, block_size);

      // Act: now convert block matrix back to matrix
      var (result, _) = BlockMatrixHelpers.ConvertBlockMatrixToMatrix(block_matrix, block_count, block_size);

      // Assert
      Assert.Equal(matrix, result);
    }

    [Theory]
    [InlineData("18-14", "00", 2)]
    [InlineData("18-14", "00", 3)]
    [InlineData("18-14", "00", 6)]
    [InlineData("18-14", "01", 2)]
    [InlineData("18-14", "01", 3)]
    [InlineData("18-14", "01", 6)]
    [InlineData("18-14", "02", 2)]
    [InlineData("18-14", "02", 3)]
    [InlineData("18-14", "02", 6)]
    public void Variants(string input, string variant, int block_size)
    {
      // Arrange
      var (matrix, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");
        
      var (result, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result");

      // Arrange: convert both matrices to block matrices
      var (block_matrix, block_count, _) = BlockMatrixHelpers.ConvertMatrixToBlockMatrix(matrix, size, block_size);
      var (result_block_matrix, _, _) = BlockMatrixHelpers.ConvertMatrixToBlockMatrix(result, size, block_size);

      // Act
      switch (variant)
      {
        case "00": new BlockFloydWarshall().BlockFloydWarshall_00(block_matrix, block_count, block_size); break;
        case "01": new BlockFloydWarshall().BlockFloydWarshall_01(block_matrix, block_count, block_size); break;
        case "02": new BlockFloydWarshall().BlockFloydWarshall_02(block_matrix, block_count, block_size); break;
      }

      // Assert
      Assert.Equal(result_block_matrix, block_matrix);
    }
  }
}
