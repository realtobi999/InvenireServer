namespace InvenireServer.Presentation;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
        }
        var app = builder.Build();
        {
            app.Run();
        }
    }
}