
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Code.Utilz
{
  public static class MatrixHelpers
  {
    public static (int[] matrix, int size) FromInputFile(
        string file)
    {
      if (string.IsNullOrWhiteSpace(file))
      {
        throw new ArgumentException(
          $"'{nameof(file)}' cannot be null or whitespace.",
          nameof(file));
      }

      using var stream = new StreamReader(file);

      var line = stream.ReadLine();
      if (!int.TryParse(line, out var size))
        throw new Exception($"Can't read matrix size from '{line}'.");

      var matrix = new int[size * size];

      // Fill the whole matrix with NO_EDGE values
      // because we are reading only existing edges
      //
      Array.Fill(matrix, Constants.NO_EDGE);

      while ((line = stream.ReadLine()) is not null)
      {
        var match = Regex.Match(line, @"(?<from>\d+)\s(?<to>\d+)\s(?<distance>\d+)");
        if (!match.Success)
          throw new Exception($"Can't read <from> <to> <distance> from '{line}'.");

        var from = int.Parse(match.Groups["from"].ValueSpan);
        var to = int.Parse(match.Groups["to"].ValueSpan);
        var distance = int.Parse(match.Groups["distance"].ValueSpan);

        matrix[from * size + to] = distance;
      }
      return (matrix, size);
    }
  }
}