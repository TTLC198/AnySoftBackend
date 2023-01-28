using Microsoft.EntityFrameworkCore;
using RPM_PR_LIB;
using RPM_Project_Backend.Repositories;
using RPM_Project_Backend.Services.Database;

namespace RPM_Project_Backend;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "MyPolicy", policy => policy.WithOrigins("http://localhost:3000").AllowCredentials());
        });
        
        var connection = Configuration.GetConnectionString("DefaultConnection")!;
        services.AddMvc();
        services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
        services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
        services.AddScoped<IBaseRepository<Product>, BaseRepository<Product>>();
        services.AddControllers();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors();
        
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}