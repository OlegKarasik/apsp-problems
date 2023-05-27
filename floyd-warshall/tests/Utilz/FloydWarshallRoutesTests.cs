using System;
using System.Linq;
using Code.Benchmarks;
using Code.Utilz;
using Xunit;

namespace Tests.Utilz
{
  public class RoutesHelpersTests
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
      var (routes, sz) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.route");

      var route = RoutesHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{input}.input.result.routes", i, j);

      // Act
      var result = variant switch
      {
        "RebuildRouteWithLinkedList" => RoutesHelpers.RebuildRouteWithLinkedList(routes, sz, i, j),
        "RebuildRouteWithArray" => RoutesHelpers.RebuildRouteWithArray(routes, sz, i, j),
        "RebuildRouteWithReverseYield" => RoutesHelpers.RebuildRouteWithReverseYield(routes, sz, i, j).Reverse(),
        _ => throw new NotImplementedException(),
      };

      // Assert
      Assert.Equal(result, route);
    }
  }
}