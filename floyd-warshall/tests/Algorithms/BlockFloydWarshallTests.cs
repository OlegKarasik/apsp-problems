using System;
using Code.Algorithms;
using Code.Utilz;
using Xunit;

namespace Tests.Algorithms
{
  public class BlockFloydWarshallTests
  {
    [Theory]
    [InlineData("18-14", "Baseline", 2)]
    [InlineData("18-14", "Baseline", 3)]
    [InlineData("18-14", "Baseline", 6)]
    [InlineData("18-14", "ParallelOptimisation", 2)]
    [InlineData("18-14", "ParallelOptimisation", 3)]
    [InlineData("18-14", "ParallelOptimisation", 6)]
    [InlineData("18-14", "VectorOptimisation", 2)]
    [InlineData("18-14", "VectorOptimisation", 3)]
    [InlineData("18-14", "VectorOptimisation", 6)]
    [InlineData("18-14", "ParallelVectorOptimisations", 2)]
    [InlineData("18-14", "ParallelVectorOptimisations", 3)]
    [InlineData("18-14", "ParallelVectorOptimisations", 6)]
    public void Variants(string input, string variant, int block_size)
    {
      // Arrange
      var (matrix, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");
        
      var (result, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result");

      // Arrange: convert both matrices to block matrices
      MatrixHelpers.SplitInPlace(matrix, size, block_size, out var block_count);
      MatrixHelpers.SplitInPlace(result, size, block_size, out _);

      // Act
      switch (variant)
      {
        case "Baseline": BlockFloydWarshall.Baseline(matrix, block_count, block_size); break;
        case "ParallelOptimisation": BlockFloydWarshall.ParallelOptimisation(matrix, block_count, block_size); break;
        case "VectorOptimisation": BlockFloydWarshall.VectorOptimisation(matrix, block_count, block_size); break;
        case "ParallelVectorOptimisations": BlockFloydWarshall.ParallelVectorOptimisations(matrix, block_count, block_size); break;
        default:
          throw new NotImplementedException();
      }

      // Assert
      Assert.Equal(result, matrix);
    }
  }
}
