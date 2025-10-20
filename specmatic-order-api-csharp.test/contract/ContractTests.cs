using System.ComponentModel;
using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace specmatic_order_bff_csharp.test.contract;

public class ContractTests : IAsyncLifetime
{
    private DotNet.Testcontainers.Containers.IContainer _stubContainer, _testContainer;
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
        var logs = await _testContainer.GetLogsAsync();
        if (!logs.Stdout.Contains("Failures: 0"))
        {
            Assert.Fail("There are failing tests");
        }
    }

    public async Task InitializeAsync()
    {
        await TestcontainersSettings.ExposeHostPortsAsync(8090)
            .ConfigureAwait(false);

        StartOrderApiService();
        await StartDomainServiceStub();
    }

    private async Task RunContractTests()
    { 
        var localReportDirectory = Path.Combine(Pwd, "build", "reports");
        Directory.CreateDirectory(localReportDirectory);
        
        _testContainer = new ContainerBuilder()
            .WithImage("specmatic/specmatic").WithCommand("test")
            .WithCommand("--port=8090")
            .WithCommand("--host=host.testcontainers.internal")
            .WithCommand("--filter=PATH!='/internal/metrics'")
            .WithOutputConsumer(Consume.RedirectStdoutAndStderrToConsole())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Tests run:"))
            .WithBindMount(localReportDirectory, $"{TestContainerDirectory}/build/reports")
            .WithBindMount(
                $"{Pwd}/specmatic.yaml",
                $"{TestContainerDirectory}/specmatic.yaml").Build();
         
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

    private async Task StartDomainServiceStub()
    {
        _stubContainer = new ContainerBuilder()
            .WithImage("specmatic/specmatic").WithCommand("stub")
            .WithCommand("--examples=examples")
            .WithPortBinding(9000)
            .WithOutputConsumer(Consume.RedirectStdoutAndStderrToConsole())
            .WithExposedPort(9000)
            .WithReuse(true)
            .WithBindMount($"{Pwd}/examples/domain_service", $"{TestContainerDirectory}/examples")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9000))
            .WithBindMount(
                $"{Pwd}/specmatic.yaml",
                $"{TestContainerDirectory}/specmatic.yaml").Build();
        
        await _stubContainer.StartAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _stubContainer.DisposeAsync();
        await _testContainer.DisposeAsync();
        if (!_appProcess.HasExited)
        {
            _appProcess.Kill();
            await _appProcess.WaitForExitAsync();
            _appProcess.Dispose();
        }
    }
}