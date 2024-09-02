using System;
using System.IO;
using Code.Algorithms;
using Code.Utilz;
using Xunit;

namespace Tests.Algorithms;

public class FloydWarshallTests
{
  [Theory]
  [InlineData("18-14", "Baseline")]
  [InlineData("18-14", "SpartialOptimisation")]
  [InlineData("18-14", "SpartialParallelOptimisations")]
  [InlineData("18-14", "SpartialVectorOptimisations")]
  [InlineData("18-14", "SpartialParallelVectorOptimisations")]
  [InlineData("18-14", "ParallelOptimisation")]
  [InlineData("18-14", "VectorOptimisation")]
  [InlineData("18-14", "ParallelVectorOptimisations")]
  public void Variants(string input, string variant)
  {
    // Arrange
    using var inputStream = new FileStream(
      $@"{Environment.CurrentDirectory}/Data/{input}.input", FileMode.Open, FileAccess.Read, FileShare.Read);

    using var resultStream = new FileStream(
      $@"{Environment.CurrentDirectory}/Data/{input}.input.result", FileMode.Open, FileAccess.Read, FileShare.Read);

    var matrix = Matrix.Read(inputStream);
    var result = Matrix.Read(resultStream);

    // Act
    switch (variant)
    {
      case "Baseline": FloydWarshall.Baseline(matrix); break;
      case "SpartialOptimisation": FloydWarshall.SpartialOptimisation(matrix); break;
      case "SpartialParallelOptimisations": FloydWarshall.SpartialParallelOptimisations(matrix); break;
      case "SpartialVectorOptimisations": FloydWarshall.SpartialVectorOptimisations(matrix); break;
      case "SpartialParallelVectorOptimisations": FloydWarshall.SpartialParallelVectorOptimisations(matrix); break;
      case "ParallelOptimisation": FloydWarshall.ParallelOptimisation(matrix); break;
      case "VectorOptimisation": FloydWarshall.VectorOptimisation(matrix); break;
      case "ParallelVectorOptimisations": FloydWarshall.ParallelVectorOptimisations(matrix); break;
      default:
        throw new NotImplementedException();
    }

    // Assert
    Assert.Equal(result.Data, matrix.Data);
  }

  [Theory]
  [InlineData("18-14", "BaselineWithRoutes")]
  [InlineData("18-14", "SpartialVectorOptimisationsWithRoutes")]
  public void VariantsWithRoutes(string input, string variant)
  {
    // Arrange
    using var inputStream = new FileStream(
      $@"{Environment.CurrentDirectory}/Data/{input}.input", FileMode.Open, FileAccess.Read, FileShare.Read);

    using var resultStream = new FileStream(
      $@"{Environment.CurrentDirectory}/Data/{input}.input.result", FileMode.Open, FileAccess.Read, FileShare.Read);

    using var resultRoutesStream = new FileStream(
      $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route", FileMode.Open, FileAccess.Read, FileShare.Read);

    var matrix = Matrix.Read(inputStream);
    var matrixRoutes = Matrix.Default(matrix.Size);

    var result = Matrix.Read(resultStream);
    var resultRoutes = Matrix.Read(resultRoutesStream); 

    // Act
    switch (variant)
    {
      case "BaselineWithRoutes": FloydWarshall.BaselineWithRoutes(matrix, matrixRoutes); break;
      case "SpartialVectorOptimisationsWithRoutes": FloydWarshall.SpartialVectorOptimisationsWithRoutes(matrix, matrixRoutes); break;
      default:
        throw new NotImplementedException();
    }

    // Assert
    Assert.Equal(result.Data, matrix.Data);
    Assert.Equal(resultRoutes.Data, matrixRoutes.Data);
  }
}
