using System;
using System.IO;
using Code.Algorithms;
using Code.Utilz;
using Xunit;

namespace Tests.Algorithms;

public class BlockedFloydWarshallTests
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
    using var inputStream = new FileStream(
      $@"{Environment.CurrentDirectory}/Data/{input}.input", FileMode.Open, FileAccess.Read, FileShare.Read);

    using var resultStream = new FileStream(
      $@"{Environment.CurrentDirectory}/Data/{input}.input.result", FileMode.Open, FileAccess.Read, FileShare.Read);

    var blocks = Matrix.Read(inputStream).SplitInBlocks(block_size);
    var result = Matrix.Read(resultStream).SplitInBlocks(block_size); 

    // Act
    switch (variant)
    {
      case "Baseline": BlockedFloydWarshall.Baseline(blocks); break;
      case "ParallelOptimisation": BlockedFloydWarshall.ParallelOptimisation(blocks); break;
      case "VectorOptimisation": BlockedFloydWarshall.VectorOptimisation(blocks); break;
      case "ParallelVectorOptimisations": BlockedFloydWarshall.ParallelVectorOptimisations(blocks); break;
      default:
        throw new NotImplementedException();
    }

    // Assert
    Assert.Equal(result.Data, blocks.Data);
  }
}
