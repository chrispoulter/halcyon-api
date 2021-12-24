using System.Threading.Tasks;

namespace Halcyon.Web.Services.Seed
{
    public interface ISeedService
    {
        void SeedData();

        Task SeedDataAsync();
    }
}
