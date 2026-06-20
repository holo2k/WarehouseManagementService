using WarehouseManagementService.Api.Bootstrap;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddServices(builder.Configuration);

        var app = builder.Build();

        app.UseWebApi();

        await app.InitializeDatabaseAsync();
        await app.RunAsync();
    }
}