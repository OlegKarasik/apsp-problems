using System;
using Code.Benchmarks;
using Code.Utilz;
using Xunit;

namespace Tests.Benchmarks
{
  public class FloydWarshallRoutesTests
  {
    [Theory]
    [InlineData("00")]
    public void Variants(string variant)
    {
      var inputs = new[]
      {
        "18-14"
      };

      foreach (var input in inputs)
      {
        var (matrix, size) = MatrixHelpers.FromInputFile(
          $@"{Environment.CurrentDirectory}/Data/{input}.input");

        var (routes, _) = MatrixHelpers.Initialize(size);

        var (result, _) = MatrixHelpers.FromInputFile(
          $@"{Environment.CurrentDirectory}/Data/{input}.input.result");

        var (result_routes, _) = MatrixHelpers.FromInputFile(
          $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route");

        Assert.Equal(matrix.Length, result.Length);

        switch (variant)
        {
          case "00": new FloydWarshallRoutes().FloydWarshallRoutes_00(matrix, routes, size); break;
          case "01": new FloydWarshallRoutes().FloydWarshallRoutes_01(matrix, routes, size); break;
        }

        Assert.Equal(result, matrix);
        Assert.Equal(result_routes, routes);
      }
    }
  
    [Theory]
    [InlineData("18-14", 0, 1)]
    public void Rebuild(string input, int i, int j)
    {
      var (routes, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route");


      Assert.Equal(result, matrix);
      Assert.Equal(result_routes, routes);
    }
  }
}