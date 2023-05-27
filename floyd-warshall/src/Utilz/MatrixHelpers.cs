
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Code.Utilz
{
  public static class MatrixHelpers
  {
    public static (long[] matrix, int size) Initialize(
      int size)
    {
      var matrix = new long[size * size];
      Array.Fill(matrix, Constants.NO_EDGE);

      return (matrix, size);
    }
    
    public static (long[] matrix, int size) FromInputFile(
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

      var matrix = new long[size * size];

      // Fill the whole matrix with NO_EDGE values
      // because we are reading only existing edges
      //
      Array.Fill(matrix, Constants.NO_EDGE);

      while ((line = stream.ReadLine()) is not null)
      {
        var match = Regex.Match(line, @"(?<from>\d+)\s(?<to>\d+)\s(?<distance>\d+)");
        if (!match.Success)
          throw new Exception($"Can't read <from> <to> <distance> from '{line}'.");

        var from = long.Parse(match.Groups["from"].ValueSpan);
        var to = long.Parse(match.Groups["to"].ValueSpan);
        var distance = long.Parse(match.Groups["distance"].ValueSpan);

        matrix[from * size + to] = distance;
      }
      return (matrix, size);
    }
  
    public static void SplitInPlace(
      long[] matrix, int size, int block_size, out int block_count)
    {
      block_count = size / block_size;
      if (size % block_size != 0)
      {
        throw new InvalidOperationException("Can't split matrix into uneven blocks");
      }

      var lineral_block_size = block_size * block_size;
      var lineral_block_row_size = block_count * lineral_block_size;

      var temp = new long[size * size];
      for (var i = 0; i < size; i++)
      {
        for (var j = 0; j < size; j++)
        {
          var b_i = i / block_size; var b_j = j / block_size;
          var b_x = i % block_size; var b_y = j % block_size;

          temp[b_i * lineral_block_row_size + b_j * lineral_block_size + b_x * block_size + b_y] = matrix[i * size + j];
        }
      }
      Array.Copy(temp, matrix, size * size);
    }

    public static void JoinInPlace(
      long[] matrix, int block_count, int block_size, out int size)
    {
      size = block_count * block_size;

      var lineral_block_size = block_size * block_size;
      var lineral_block_row_size = block_count * lineral_block_size;

      var temp = new long[size * size];
      for (var i = 0; i < size; i++)
      {
        for (var j = 0; j < size; j++)
        {
          var b_i = i / block_size; var b_j = j / block_size;
          var b_x = i % block_size; var b_y = j % block_size;

          temp[i * size + j] = matrix[b_i * lineral_block_row_size + b_j * lineral_block_size + b_x * block_size + b_y];
        }
      }
      Array.Copy(temp, matrix, size * size);
    }
  }
}