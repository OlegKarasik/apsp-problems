using System;
using Code.Algorithms;
using Code.Utilz;
using Xunit;

namespace Tests.Algorithms
{
  public class FloydWarshallTests
  {
    [Theory]
    [InlineData("18-14", "Baseline")]
    [InlineData("18-14", "SpartialOptimisation")]
    [InlineData("18-14", "ParallelOptimisation")]
    [InlineData("18-14", "VectorOptimisation")]
    [InlineData("18-14", "ParallelVectorOptimisations")]
    public void Variants(string input, string variant)
    {
      // Arrange
      var (matrix, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");
        
      var (result, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result");

      // Act
      switch (variant)
      {
        case "Baseline": FloydWarshall.Baseline(matrix, size); break;
        case "SpartialOptimisation": FloydWarshall.SpartialOptimisation(matrix, size); break;
        case "ParallelOptimisation": FloydWarshall.ParallelOptimisation(matrix, size); break;
        case "VectorOptimisation": FloydWarshall.VectorOptimisation(matrix, size); break;
        case "ParallelVectorOptimisations": FloydWarshall.ParallelVectorOptimisations(matrix, size); break;
        default:
          throw new NotImplementedException();
      }

      // Assert
      Assert.Equal(result, matrix);
    }

    [Theory]
    [InlineData("18-14", "BaselineWithRoutes")]
    [InlineData("18-14", "SpartialVectorOptimisationsWithRoutes")]
    public void VariantsWithRoutes(string input, string variant)
    {
      // Arrange
      var (matrix, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");

      var (matrix_routes, _) = MatrixHelpers.Initialize(size);

      var (result, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result");

      var (result_routes, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route");

      // Act
      switch (variant)
      {
        case "BaselineWithRoutes": FloydWarshall.BaselineWithRoutes(matrix, matrix_routes, size); break;
        case "SpartialVectorOptimisationsWithRoutes": FloydWarshall.SpartialVectorOptimisationsWithRoutes(matrix, matrix_routes, size); break;
        default:
          throw new NotImplementedException();
      }

      // Assert
      Assert.Equal(result, matrix);
      Assert.Equal(result_routes, matrix_routes);
    }
  }
}
