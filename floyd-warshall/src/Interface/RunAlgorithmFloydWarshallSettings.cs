using Spectre.Console.Cli;

namespace Code.Interface;

public class RunAlgorithmFloydWarshallSettings : RunAlgorithmSettings
{
  [CommandArgument(0, "<input>")]
  public string Input { get; init; }

  [CommandArgument(1, "<output>")]
  public string Output { get; init; }

  [CommandOption("-v|--vector")]
  public bool Vector { get; init; }

  [CommandOption("-p|--parallel")]
  public bool Parallel { get; init; }

  [CommandOption("-s|--spartial")]
  public bool Spartial { get; init; }
}
