﻿using Microsoft.EntityFrameworkCore;

namespace RPM_Project_Backend.Tests;

public class ApplicationContextDataFixture : IDisposable
{
    public ApplicationContext ApplicationContext { get; private set; }

    public ApplicationContextDataFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase("ApplicationContextDatabase")
            .Options;

        ApplicationContext = new ApplicationContext(options);

        ApplicationContext.Users.AddRange(TestValues.Users);
        ApplicationContext.SaveChanges();
    }

    public void Dispose()
    {
        ApplicationContext.Dispose();
    }
}