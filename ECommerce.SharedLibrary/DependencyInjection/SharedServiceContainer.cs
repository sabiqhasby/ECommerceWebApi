using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.SharedLibrary.DependencyInjection
{
   public static class SharedServiceContainer
   {
      public static IServiceCollection AddSharedServices<TContext>
         (this IServiceCollection services, IConfiguration config, string fileName)
         where TContext : DbContext
      {
         // Add Generic Database Context
         services.AddDbContext<TContext>
            (options => options.UseSqlServer(config.GetConnectionString("ECommerceConnection"), sqlserverOption =>
         sqlserverOption.EnableRetryOnFailure()));

         //Configure serilog logging
         return services;
      }
   }
}
