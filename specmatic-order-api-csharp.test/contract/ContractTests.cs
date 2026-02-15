using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace specmatic_order_bff_csharp.test.contract;

public class ContractTests : IAsyncLifetime
{
    private DotNet.Testcontainers.Containers.IContainer _testContainer;
    private Process _appProcess;
    private static readonly string Pwd =
        Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? string.Empty;
    private readonly string _projectPath = Directory.GetParent(Pwd)?.FullName ?? string.Empty;
    private const string ProjectName = "specmatic-order-api-csharp";
    private const string TestContainerDirectory = "/usr/src/app";

    [Fact]
    public async Task ContractTestsAsync()
    {
        await RunContractTests();
        var exit = await _testContainer.GetExitCodeAsync();
        var logs = await _testContainer.GetLogsAsync();

        if (exit != 0 || !logs.Stdout.Contains("Failures: 0"))
        {
            throw new Exception("Contract tests failed with exit code: " + exit);
        }
    }

    public async Task InitializeAsync()
    {
        StartOrderApiService();
    }

    private async Task RunContractTests()
    {
        var localReportDirectory = Path.Combine(Pwd, "build", "reports");
        Directory.CreateDirectory(localReportDirectory);

        _testContainer = new ContainerBuilder()
            .WithImage("specmatic/specmatic").WithCommand("test")
            .WithOutputConsumer(Consume.RedirectStdoutAndStderrToConsole())
            .WithEnvironment("APP_URL", "http://host.docker.internal:8090")
            .WithBindMount($"{Pwd}",$"{TestContainerDirectory}")
            .WithExtraHost("host.docker.internal", "host-gateway")
            .Build();

        await _testContainer.StartAsync().ConfigureAwait(true);
    }

    private void StartOrderApiService()
    {
        _appProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run",
                WorkingDirectory = Path.Combine(_projectPath, ProjectName),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        _appProcess.Start();
        Console.WriteLine($"Order BFF service started on id: {_appProcess.Id}");
    }

    public async Task DisposeAsync()
    {
        await _testContainer.DisposeAsync();
        if (!_appProcess.HasExited)
        {
            _appProcess.Kill();
            await _appProcess.WaitForExitAsync();
            _appProcess.Dispose();
        }
    }
}
