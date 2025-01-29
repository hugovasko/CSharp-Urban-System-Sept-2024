using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UrbanSystem.Data.Models;
using UrbanSystem.Data.Repository;
using UrbanSystem.Data.Repository.Contracts;

namespace UrbanSystem.Web.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepositories(this IServiceCollection services, Assembly modelsAssembly)
        {
            List<Type> typesToExclude = new List<Type>()
            {
                typeof(ApplicationUser)
            };

            List<Type> modelTypes = modelsAssembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                            !t.Name.ToLower().EndsWith("attribute") &&
                            !typesToExclude.Contains(t))
                .ToList();

            foreach (Type type in modelTypes)
            {
                Type repositoryInterface = typeof(IRepository<,>);
                Type repositoryInstanceType = typeof(BaseRepository<,>);

                PropertyInfo idPropInfo = type
                    .GetProperties()
                    .FirstOrDefault(p => p.Name.ToLower() == "id");

                Type[] constructArgs = new Type[2];
                constructArgs[0] = type;

                if (idPropInfo == null)
                {
                    constructArgs[1] = typeof(object);
                }
                else
                {
                    constructArgs[1] = idPropInfo.PropertyType;
                }

                repositoryInterface = repositoryInterface.MakeGenericType(constructArgs);
                repositoryInstanceType = repositoryInstanceType.MakeGenericType(constructArgs);

                services.AddScoped(repositoryInterface, repositoryInstanceType);
            }
        }

        public static void RegisterUserDefinedServices(this IServiceCollection serviceCollection, Assembly serviceAssembly)
        {
            if (serviceAssembly == null)
            {
                throw new ArgumentNullException(nameof(serviceAssembly));
            }

            var serviceTypes = serviceAssembly.GetTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract && t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var serviceType in serviceTypes)
            {
                var serviceInterface = serviceType.GetInterfaces()
                    .FirstOrDefault(i => i.Name.Equals("I" + serviceType.Name, StringComparison.OrdinalIgnoreCase));

                if (serviceInterface != null)
                {
                    serviceCollection.AddScoped(serviceInterface, serviceType);
                }
                else
                {
                    Console.WriteLine($"Warning: No matching interface found for {serviceType.Name}");
                }
            }
        }
    }
}
