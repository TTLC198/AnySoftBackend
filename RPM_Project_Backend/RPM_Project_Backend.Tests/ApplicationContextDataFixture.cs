using Microsoft.EntityFrameworkCore;

namespace RPM_Project_Backend.Tests;

public class ApplicationContextDataFixture : IDisposable
{
    public ApplicationContext ApplicationContext { get; private set; }

    public ApplicationContextDataFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase("ApplicationContextDatabase")
            .EnableSensitiveDataLogging()
            .Options;

        ApplicationContext = new ApplicationContext(options);

        ApplicationContext.Users.AddRange(TestValues.Users);
        ApplicationContext.Categories.AddRange(TestValues.Categories);
        ApplicationContext.Products.AddRange(TestValues.Products);
        
        ApplicationContext.SaveChanges();
    }

    public void Dispose()
    {
        ApplicationContext.Dispose();
        ApplicationContext = null!;
    }
}