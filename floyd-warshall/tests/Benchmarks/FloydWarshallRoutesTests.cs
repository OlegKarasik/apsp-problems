using System;
using Code.Benchmarks;
using Code.Utilz;
using Xunit;

namespace Tests.Benchmarks
{
  public class FloydWarshallRoutesTests
  {
    [Theory]
    [InlineData("18-14", "00")]
    [InlineData("18-14", "01")]
    public void Variants(string input, string variant)
    {
      // Arrange
      var (matrix, size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input");

      var (routes, _) = MatrixHelpers.Initialize(size);

      var (result, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result");

      var (result_routes, _) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route");

      // Act
      switch (variant)
      {
        case "00": new FloydWarshallRoutes().FloydWarshallRoutes_00(matrix, routes, size); break;
        case "01": new FloydWarshallRoutes().FloydWarshallRoutes_01(matrix, routes, size); break;
      }

      // Assert
      Assert.Equal(result, matrix);
      Assert.Equal(result_routes, routes);
    }
  
    // Important: this test can be executed only against existing paths!
    //
    [Theory]
    [InlineData("18-14", "00", 1, 2)]
    [InlineData("18-14", "00", 1, 3)]
    [InlineData("18-14", "00", 1, 6)]
    [InlineData("18-14", "00", 1, 4)]
    [InlineData("18-14", "00", 3, 2)]
    [InlineData("18-14", "00", 3, 4)]
    [InlineData("18-14", "00", 3, 6)]
    [InlineData("18-14", "00", 4, 2)]
    [InlineData("18-14", "00", 4, 6)]
    [InlineData("18-14", "00", 11, 12)]
    [InlineData("18-14", "00", 11, 13)]
    [InlineData("18-14", "00", 11, 16)]
    [InlineData("18-14", "00", 11, 14)]
    [InlineData("18-14", "00", 13, 12)]
    [InlineData("18-14", "00", 13, 14)]
    [InlineData("18-14", "00", 13, 16)]
    [InlineData("18-14", "00", 14, 12)]
    [InlineData("18-14", "00", 14, 16)]
    [InlineData("18-14", "01", 1, 2)]
    [InlineData("18-14", "01", 1, 3)]
    [InlineData("18-14", "01", 1, 6)]
    [InlineData("18-14", "01", 1, 4)]
    [InlineData("18-14", "01", 3, 2)]
    [InlineData("18-14", "01", 3, 4)]
    [InlineData("18-14", "01", 3, 6)]
    [InlineData("18-14", "01", 4, 2)]
    [InlineData("18-14", "01", 4, 6)]
    [InlineData("18-14", "01", 11, 12)]
    [InlineData("18-14", "01", 11, 13)]
    [InlineData("18-14", "01", 11, 16)]
    [InlineData("18-14", "01", 11, 14)]
    [InlineData("18-14", "01", 13, 12)]
    [InlineData("18-14", "01", 13, 14)]
    [InlineData("18-14", "01", 13, 16)]
    [InlineData("18-14", "01", 14, 12)]
    [InlineData("18-14", "01", 14, 16)]
    public void Rebuild(string input, string variant, int i, int j)
    {
      // Arrange
      var (routes, sz) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route");

      var route = RoutesHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.routes", i, j);

      // Act
      var result = variant switch
      {
        "00" => FloydWarshallRoutes.RebuildRoute_00(routes, sz, i, j),
        "01" => FloydWarshallRoutes.RebuildRoute_01(routes, sz, i, j),
      };

      // Assert
      Assert.Equal(result, route);
    }
  }
}