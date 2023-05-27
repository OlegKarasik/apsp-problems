
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Code.Utilz
{
  public static class RoutesHelpers
  {
    public static long[] FromInputFile(
      string file, int i, int j)
    {
      if (string.IsNullOrWhiteSpace(file))
      {
        throw new ArgumentException(
          $"'{nameof(file)}' cannot be null or whitespace.",
          nameof(file));
      }

      using var stream = new StreamReader(file);

      string line = null;
      while ((line = stream.ReadLine()) is not null)
      {
        var match = Regex.Match(line, @"(?<from>\d+):(?<to>\d+)=(?<route>[\d,]+)");
        if (!match.Success)
          throw new Exception($"Can't read <from>:<to>=<route> from '{line}'.");

        var from = long.Parse(match.Groups["from"].ValueSpan);
        var to = long.Parse(match.Groups["to"].ValueSpan);
        if (from == i && to == j)
        {
          return match.Groups["route"].Value
            .Split(',')
            .Select(x => long.Parse(x))
            .ToArray();
        }
      }
      throw new Exception($"The route {i}:{j} isn't found in the input file.");
    }

    public static IEnumerable<long> RebuildRouteWithLinkedList(
      long[] routes, int sz, int i, int j)
    {
      var x = new LinkedList<long>();

      var z = routes[i * sz + j];
      while (z != Constants.NO_EDGE) 
      {
        x.AddFirst(z);
        z = routes[i * sz + z];
      }

      x.AddFirst(i);
      x.AddLast(j);

      return x;
    }

    public static IEnumerable<long> RebuildRouteWithArray(
      long[] routes, int sz, int i, int j)
    {
      var x = new long[sz];
      var y = sz - 1;

      x[y--] = j;

      var z = routes[i * sz + j];
      while (z != Constants.NO_EDGE) 
      {
        x[y--] = z;
        z = routes[i * sz + z];
      }

      x[y] = i;

      return new ArraySegment<long>(x, y, sz - y);
    }

    public static IEnumerable<long> RebuildRouteWithReverseYield(
      long[] routes, int sz, int i, int j)
    {
      yield return j;

      var z = routes[i * sz + j];
      while (z != Constants.NO_EDGE) 
      {
        yield return z;
        z = routes[i * sz + z];
      }

      yield return i;
    }
  }
}