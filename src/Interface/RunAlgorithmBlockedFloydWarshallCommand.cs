using Code.Utilz;
using System;
using System.Collections.Generic;
using Code.Algorithms;
using Spectre.Console.Cli;
using System.IO;

namespace Code.Interface;

public class RunAlgorithmBlockedFloydWarshallCommand : Command<RunAlgorithmBlockedFloydWarshallSettings>
{
  private readonly static Dictionary<RunAlgorithmMethod, Action<Matrix.Blocks>> methods;

  static RunAlgorithmBlockedFloydWarshallCommand()
  {
    methods = new Dictionary<RunAlgorithmMethod, Action<Matrix.Blocks>>
    {
      {
        RunAlgorithmMethod.None, BlockedFloydWarshall.Baseline
      },
      {
        RunAlgorithmMethod.Parallel, BlockedFloydWarshall.ParallelOptimisation
      },
      {
        RunAlgorithmMethod.Parallel |
        RunAlgorithmMethod.Vector, BlockedFloydWarshall.ParallelVectorOptimisations
      },
      {
        RunAlgorithmMethod.Vector, BlockedFloydWarshall.VectorOptimisation
      }
    };
  }

  private static RunAlgorithmMethod GetMethod(
    RunAlgorithmBlockedFloydWarshallSettings settings)
  {
    var method = RunAlgorithmMethod.None;

    if (settings.Parallel) method |= RunAlgorithmMethod.Parallel;
    if (settings.Vector) method |= RunAlgorithmMethod.Vector;

    return method;
  }

  public override int Execute(
    CommandContext context,
    RunAlgorithmBlockedFloydWarshallSettings settings)
  {
    if (context is null)
    {
      throw new ArgumentNullException(nameof(context));
    }
    if (settings is null)
    {
      throw new ArgumentNullException(nameof(settings));
    }
    if (settings.BlockSize == 0)
    {
      throw new ArgumentException("The --block-size parameter is required");
    }

    var method = GetMethod(settings);
    if (!methods.TryGetValue(method, out var action))
    {
      throw new ArgumentException($"The method '{method}' is not found");
    }
    
    using var inputStream = new FileStream(settings.Input, FileMode.Open, FileAccess.Read, FileShare.Read);
    using var outputStream = new FileStream(settings.Output, FileMode.Create, FileAccess.Write, FileShare.None);

    var matrix = Matrix.Read(inputStream);
    var blocks = matrix.SplitInBlocks(settings.BlockSize);

    action(blocks);

    matrix.JoinFromBlocks(blocks);
    Matrix.Write(outputStream, matrix);

    return 0;
  }
}
