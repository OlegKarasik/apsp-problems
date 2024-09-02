using System;
using System.IO;
using System.Linq;
using Code.Benchmarks;
using Code.Utilz;
using Xunit;

namespace Tests.Utilz
{
  public class RouteTests
  {
    // Important: this test can be executed only against existing paths!
    //
    [Theory]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 1, 2)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 1, 3)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 1, 6)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 1, 4)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 3, 2)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 3, 4)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 3, 6)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 4, 2)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 4, 6)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 11, 12)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 11, 13)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 11, 16)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 11, 14)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 13, 12)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 13, 14)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 13, 16)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 14, 12)]
    [InlineData("18-14", "RebuildRouteWithLinkedList", 14, 16)]
    [InlineData("18-14", "RebuildRouteWithArray", 1, 2)]
    [InlineData("18-14", "RebuildRouteWithArray", 1, 3)]
    [InlineData("18-14", "RebuildRouteWithArray", 1, 6)]
    [InlineData("18-14", "RebuildRouteWithArray", 1, 4)]
    [InlineData("18-14", "RebuildRouteWithArray", 3, 2)]
    [InlineData("18-14", "RebuildRouteWithArray", 3, 4)]
    [InlineData("18-14", "RebuildRouteWithArray", 3, 6)]
    [InlineData("18-14", "RebuildRouteWithArray", 4, 2)]
    [InlineData("18-14", "RebuildRouteWithArray", 4, 6)]
    [InlineData("18-14", "RebuildRouteWithArray", 11, 12)]
    [InlineData("18-14", "RebuildRouteWithArray", 11, 13)]
    [InlineData("18-14", "RebuildRouteWithArray", 11, 16)]
    [InlineData("18-14", "RebuildRouteWithArray", 11, 14)]
    [InlineData("18-14", "RebuildRouteWithArray", 13, 12)]
    [InlineData("18-14", "RebuildRouteWithArray", 13, 14)]
    [InlineData("18-14", "RebuildRouteWithArray", 13, 16)]
    [InlineData("18-14", "RebuildRouteWithArray", 14, 12)]
    [InlineData("18-14", "RebuildRouteWithArray", 14, 16)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 1, 2)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 1, 3)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 1, 6)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 1, 4)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 3, 2)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 3, 4)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 3, 6)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 4, 2)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 4, 6)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 11, 12)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 11, 13)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 11, 16)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 11, 14)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 13, 12)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 13, 14)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 13, 16)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 14, 12)]
    [InlineData("18-14", "RebuildRouteWithReverseYield", 14, 16)]
    public void Rebuild(string input, string variant, int i, int j)
    {
      // Arrange
      using var inputStream = new FileStream(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route", FileMode.Open, FileAccess.Read, FileShare.Read);

      using var routeStream = new FileStream(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.routes", FileMode.Open, FileAccess.Read, FileShare.Read);

      var routes = Matrix.Read(inputStream);
      var route  = Route.Read(routeStream, i, j);

      // Act
      var result = variant switch
      {
        "RebuildRouteWithLinkedList" => Route.RebuildWithLinkedList(routes, i, j),
        "RebuildRouteWithArray" => Route.RebuildWithArray(routes, i, j),
        "RebuildRouteWithReverseYield" => Route.RebuildWithReverseYield(routes, i, j),
        _ => throw new NotImplementedException(),
      };

      // Assert
      Assert.Equal(result.Path, route.Path);
    }
  }
}