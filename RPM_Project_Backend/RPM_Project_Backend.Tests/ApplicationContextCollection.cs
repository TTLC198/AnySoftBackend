using Xunit;

namespace RPM_Project_Backend.Tests;

[CollectionDefinition("ApplicationContext Collection")]
public class ApplicationContextCollection : ICollectionFixture<ApplicationContextDataFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}