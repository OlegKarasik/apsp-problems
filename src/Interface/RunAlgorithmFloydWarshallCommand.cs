using Code.Utilz;
using System;
using Spectre.Console;
using System.Collections.Generic;
using Code.Algorithms;
using Spectre.Console.Cli;
using System.IO;

namespace Code.Interface;

public class RunAlgorithmFloydWarshallCommand : Command<RunAlgorithmFloydWarshallSettings>
{
  private readonly static Dictionary<RunAlgorithmMethod, Action<Matrix>> methods;

  static RunAlgorithmFloydWarshallCommand()
  {
    methods = new Dictionary<RunAlgorithmMethod, Action<Matrix>>
    {
      {
        RunAlgorithmMethod.None, FloydWarshall.Baseline
      },
      {
        RunAlgorithmMethod.Spartial, FloydWarshall.SpartialOptimisation
      },
      {
        RunAlgorithmMethod.Spartial |
        RunAlgorithmMethod.Parallel, FloydWarshall.SpartialParallelOptimisations },
      {
        RunAlgorithmMethod.Spartial |
        RunAlgorithmMethod.Parallel |
        RunAlgorithmMethod.Vector, FloydWarshall.SpartialParallelVectorOptimisations
      },
      {
        RunAlgorithmMethod.Parallel, FloydWarshall.ParallelOptimisation
      },
      {
        RunAlgorithmMethod.Parallel |
        RunAlgorithmMethod.Vector, FloydWarshall.ParallelVectorOptimisations
      },
      {
        RunAlgorithmMethod.Vector, FloydWarshall.VectorOptimisation
      }
    };
  }

  private static RunAlgorithmMethod GetMethod(
    RunAlgorithmFloydWarshallSettings settings)
  {
    var method = RunAlgorithmMethod.None;

    if (settings.Spartial) method |= RunAlgorithmMethod.Spartial;
    if (settings.Parallel) method |= RunAlgorithmMethod.Parallel;
    if (settings.Vector) method |= RunAlgorithmMethod.Vector;

    return method;
  }

  public override int Execute(
    CommandContext context,
    RunAlgorithmFloydWarshallSettings settings)
  {
    if (context is null)
    {
      throw new ArgumentNullException(nameof(context));
    }
    if (settings is null)
    {
      throw new ArgumentNullException(nameof(settings));
    }

    var method = GetMethod(settings);
    if (!methods.TryGetValue(method, out var action))
    {
      throw new ArgumentException($"The method '{method}' is not found");
    }
    
    using var inputStream = new FileStream(settings.Input, FileMode.Open, FileAccess.Read, FileShare.Read);
    using var outputStream = new FileStream(settings.Output, FileMode.Create, FileAccess.Write, FileShare.None);

    var matrix = Matrix.Read(inputStream);

    action(matrix);

    Matrix.Write(outputStream, matrix);

    return 0;
  }
}
