using System;
using Code.Benchmarks;
using Code.Utilz;
using Xunit;

namespace Tests.Benchmarks
{
  public class FloydWarshallTests
  {
    [Theory]
    [InlineData("18-14", "00")]
    [InlineData("18-14", "01")]
    [InlineData("18-14", "02")]
    [InlineData("18-14", "03")]
    [InlineData("18-14", "04")]
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
        case "00": new FloydWarshall().FloydWarshall_00(matrix, size); break;
        case "01": new FloydWarshall().FloydWarshall_01(matrix, size); break;
        case "02": new FloydWarshall().FloydWarshall_02(matrix, size); break;
        case "03": new FloydWarshall().FloydWarshall_03(matrix, size); break;
        case "04": new FloydWarshall().FloydWarshall_04(matrix, size); break;
      }

      // Assert
      Assert.Equal(result, matrix);
    }

    [Theory]
    [InlineData("18-14", "00")]
    [InlineData("18-14", "01")]
    [InlineData("18-14", "02")]
    [InlineData("18-14", "03")]
    [InlineData("18-14", "04")]
    public void VariantsUnsafe(string input, string variant)
    {
      // Arrange
      var (matrix, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");
        
      var (result, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result");

      // Act
      switch (variant)
      {
        case "00": new FloydWarshallUnsafe().FloydWarshall_00(matrix, size); break;
        case "01": new FloydWarshallUnsafe().FloydWarshall_01(matrix, size); break;
        case "02": new FloydWarshallUnsafe().FloydWarshall_02(matrix, size); break;
        case "03": new FloydWarshallUnsafe().FloydWarshall_03(matrix, size); break;
        case "04": new FloydWarshallUnsafe().FloydWarshall_04(matrix, size); break;
      }

      // Assert
      Assert.Equal(result, matrix);
    }
  }
}
