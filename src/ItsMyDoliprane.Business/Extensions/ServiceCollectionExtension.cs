using ItsMyDoliprane.Business.Medications;
using Microsoft.Extensions.DependencyInjection;

namespace ItsMyDoliprane.Business.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddDolipraneBusiness(this IServiceCollection serviceCollection) {
        serviceCollection.AddTransient<MedicationDoliprane>();
        serviceCollection.AddTransient<MedicationHumex>();
    }
}
