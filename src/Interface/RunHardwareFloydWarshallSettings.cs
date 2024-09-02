using Spectre.Console.Cli;

namespace Code.Interface;

public class RunHardwareFloydWarshallSettings : RunSettings
{
  [CommandArgument(0, "<input>")]
  public string Input { get; init; }

  [CommandArgument(1, "<output>")]
  public string Output { get; init; }
}
