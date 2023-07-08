
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Code.Utilz;

public readonly struct Route
{
  public readonly int From;
  public readonly int To;
  public readonly IEnumerable<long> Path;

  public Route(
    int from,
    int to,
    IEnumerable<long>  path)
  {
    this.From = from;
    this.To = to;
    this.Path = path;
  }

  public static Route Read(
    Stream stream, int i, int j)
  {
    using var input = new StreamReader(stream, leaveOpen: true);

    string line = null;
    while ((line = input.ReadLine()) is not null)
    {
      var match = Regex.Match(line, @"(?<from>\d+):(?<to>\d+)=(?<route>[\d,]+)");
      if (!match.Success)
        throw new Exception($"Unexpected part of stream, expected <from>:<to>=<route> as the value but got: '{line}'");

      var from = int.Parse(match.Groups["from"].ValueSpan);
      var to = int.Parse(match.Groups["to"].ValueSpan);
      if (from == i && to == j)
      {
        var path = match.Groups["route"].Value
          .Split(',')
          .Select(x => long.Parse(x))
          .ToArray();

        return new Route(from, to, path);
      }
    }
    throw new Exception($"The route {i}:{j} isn't found in the input file.");
  }

  public static Route RebuildWithLinkedList(Matrix matrix, int from, int to)
    => new(from, to, RebuildWithLinkedList(matrix.Data, matrix.Size, from, to));

  public static Route RebuildWithArray(Matrix matrix, int from, int to)
    => new(from, to, RebuildWithArray(matrix.Data, matrix.Size, from, to));

  public static Route RebuildWithReverseYield(Matrix matrix, int from, int to)
    => new(from, to, RebuildWithReverseYield(matrix.Data, matrix.Size, from, to).Reverse());

  private static IEnumerable<long> RebuildWithLinkedList(
    long[] routes, int sz, int i, int j)
  {
    var x = new LinkedList<long>();

    var z = routes[i * sz + j];
    while (z != Matrix.NO_EDGE) 
    {
      x.AddFirst(z);
      z = routes[i * sz + z];
    }

    x.AddFirst(i);
    x.AddLast(j);

    return x;
  }

  private static IEnumerable<long> RebuildWithArray(
    long[] routes, int sz, int i, int j)
  {
    var x = new long[sz];
    var y = sz - 1;

    x[y--] = j;

    var z = routes[i * sz + j];
    while (z != Matrix.NO_EDGE) 
    {
      x[y--] = z;
      z = routes[i * sz + z];
    }

    x[y] = i;

    return new ArraySegment<long>(x, y, sz - y);
  }

  private static IEnumerable<long> RebuildWithReverseYield(
    long[] routes, int sz, int i, int j)
  {
    yield return j;

    var z = routes[i * sz + j];
    while (z != Matrix.NO_EDGE) 
    {
      yield return z;
      z = routes[i * sz + z];
    }

    yield return i;
  }
}