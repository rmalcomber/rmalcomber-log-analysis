using LivenerTechTest.interfaces;
using LivenerTechTest.providers;

namespace LivenerTechTest;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddTransient<IDataStore, FileDataStoreProvider>();
        builder.Services.AddControllers();

        var app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}