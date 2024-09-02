using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Code.Hardware;
using Code.Utilz;
using Hast.Layer;
using Hast.Transformer.Vhdl.Configuration;
using Hast.Xilinx.Drivers;
using Lombiq.HelpfulLibraries.Common.Utilities;
using Spectre.Console.Cli;

namespace Code.Interface;

public class RunHardwareFloydWarshallCommand : AsyncCommand<RunHardwareFloydWarshallSettings>
{
  public async override Task<int> ExecuteAsync(
    [NotNull] CommandContext context,
    [NotNull] RunHardwareFloydWarshallSettings settings)
  {
    using var hastlayer = Hastlayer.Create();

    var configuration = new HardwareGenerationConfiguration(NexysA7Driver.NexysA7);

    configuration.AddHardwareEntryPointType<APSP04>();

    configuration.VhdlTransformerConfiguration().VhdlGenerationConfiguration = VhdlGenerationConfiguration.Debug;

    hastlayer.ExecutedOnHardware += (_, e) =>
        Console.WriteLine(
            StringHelper.ConcatenateConvertiblesInvariant(
                "Executing on hardware took ",
                e.Arguments.HardwareExecutionInformation.HardwareExecutionTimeMilliseconds,
                " milliseconds (net) ",
                e.Arguments.HardwareExecutionInformation.FullExecutionTimeMilliseconds,
                " milliseconds (all together)."));

    Console.WriteLine("Hardware generation starts.");
    var hardwareRepresentation = await hastlayer.GenerateHardwareAsync(
        new[]
        {
            typeof(APSP04).Assembly,
        },
        configuration);

    Console.WriteLine("Hardware generated, starting software execution.");
    Console.WriteLine();

    using var inputStream = new FileStream(settings.Input, FileMode.Open, FileAccess.Read, FileShare.Read);
    using var outputStream = new FileStream(settings.Output, FileMode.Create, FileAccess.Write, FileShare.None);

    var matrix = Matrix.Read(inputStream);

    var sw = System.Diagnostics.Stopwatch.StartNew();
    new APSP04().Run(matrix.Data, matrix.Size);
    sw.Stop();

    Matrix.Write(outputStream, matrix);

    Console.WriteLine(StringHelper.ConcatenateConvertiblesInvariant("On CPU it took ", sw.ElapsedMilliseconds, " milliseconds."));

    Console.WriteLine();
    Console.WriteLine("Starting hardware execution.");

    var parallelAlgorithm = await hastlayer.GenerateProxyAsync(hardwareRepresentation, new APSP04());

    var memoryConfig = hastlayer.CreateMemoryConfiguration(hardwareRepresentation);
    parallelAlgorithm.Run(matrix.Data, matrix.Size, hastlayer, hardwareRepresentation.HardwareGenerationConfiguration);
    parallelAlgorithm.Run(matrix.Data, matrix.Size, hastlayer, hardwareRepresentation.HardwareGenerationConfiguration);
    parallelAlgorithm.Run(matrix.Data, matrix.Size, hastlayer, hardwareRepresentation.HardwareGenerationConfiguration);

    return 0;
  }
}
