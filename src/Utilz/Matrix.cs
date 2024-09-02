using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Code.Utilz;

public readonly partial struct Matrix
{
  public readonly struct Blocks
  {
    public const int NO_EDGE = Matrix.NO_EDGE;

    public readonly int[] Data;
    public readonly int Count;
    public readonly int Size;

    public Blocks(
      int[] data,
      int count,
      int size)
    {
      this.Data = data;
      this.Count = count;
      this.Size = size; 
    }
  }

  public const int NO_EDGE = int.MaxValue / 2 - 1;

  public readonly int[] Data;
  public readonly int Size;

  public Matrix(
    int[] data,
    int size)
  {
    this.Data = data;
    this.Size = size;
  }

  [GeneratedRegex("(?<from>\\d+)\\s(?<to>\\d+)\\s(?<distance>\\d+)")]
  private static partial Regex ReadRegex();

  public Blocks SplitInBlocks(
    int size)
  {
    var count = this.Size / size;
    if (this.Size % size != 0)
    {
      throw new InvalidOperationException("Can't split matrix into uneven blocks");
    }

    var lineralBlockSize = size * size;
    var lineralBlockRowSize = count * lineralBlockSize;

    var blocks = new int[this.Size * this.Size];
    for (var i = 0; i < this.Size; i++)
    {
      for (var j = 0; j < this.Size; j++)
      {
        var bI = i / size; var bJ = j / size;
        var bX = i % size; var bY = j % size;

        blocks[bI * lineralBlockRowSize + bJ * lineralBlockSize + bX * size + bY] = this.Data[i * this.Size + j];
      }
    }
    Array.Copy(blocks, this.Data, this.Size * this.Size);

    return new Blocks(this.Data, count, size);
  }

  public void JoinFromBlocks(
    Blocks blocks)
  {
    if (blocks.Data != this.Data)
    {
      throw new ArgumentException("Unable to assemble matrix from blocks of different matrix");
    }

    var lineralBlockSize = blocks.Size * blocks.Size;
    var lineralBlockRowSize = blocks.Count * lineralBlockSize;

    var data = new int[this.Data.Length];
    for (var i = 0; i < this.Size; i++)
    {
      for (var j = 0; j < this.Size; j++)
      {
        var bI = i / blocks.Size; var bJ = j / blocks.Size;
        var bX = i % blocks.Size; var bY = j % blocks.Size;

        data[i * this.Size + j] = blocks.Data[bI * lineralBlockRowSize + bJ * lineralBlockSize + bX * blocks.Size + bY];
      }
    }
    Array.Copy(data, blocks.Data, data.Length);
  }

  public static Matrix Default(
    int size)
  {
    var data = new int[size * size];
    Array.Fill(data, NO_EDGE);

    return new Matrix(data, size);
  }

  public static Matrix Copy(
    Matrix matrix)
  {
    var data = new int[matrix.Size * matrix.Size];

    Array.Copy(matrix.Data, data, data.Length);

    return new Matrix(data, matrix.Size);
  }

  public static Matrix Read(
    Stream stream)
  {
    if (stream is null)
    {
      throw new ArgumentNullException(nameof(stream));
    }

    using var input = new StreamReader(stream, leaveOpen: true);

    var line = input.ReadLine();
    if (!int.TryParse(line, out var size))
      throw new Exception($"Unexpected start of stream, expected <number> as the first value from stream but got: '{line}'");

    var data = new int[size * size];

    // Fill the whole data with NO_EDGE values
    // because we are reading only existing edges
    //
    Array.Fill(data, NO_EDGE);

    while ((line = input.ReadLine()) is not null)
    {
      var match = ReadRegex().Match(line);
      if (!match.Success)
        throw new Exception($"Unexpected value, expected value in <from> <to> <distance> format but got: '{line}'");

      var from = int.Parse(match.Groups["from"].ValueSpan);
      var to = int.Parse(match.Groups["to"].ValueSpan);
      var distance = int.Parse(match.Groups["distance"].ValueSpan);

      data[from * size + to] = distance;
    }
    return new Matrix(data, size);
  }

  public static void Write(
    Stream stream,
    Matrix matrix)
  {
    if (stream is null)
    {
      throw new ArgumentNullException(nameof(stream));
    }

    using var output = new StreamWriter(stream, leaveOpen: true);

    output.WriteLine(matrix.Size);

    for (int i = 0; i < matrix.Size; i++)
    {
      for (int j = 0; j < matrix.Size; j++)
      {
        if (matrix.Data[i * matrix.Size + j] != NO_EDGE)
        {
          output.WriteLine($"{i} {j} {matrix.Data[i * matrix.Size + j]}");
        }
      }
    }
  }
}