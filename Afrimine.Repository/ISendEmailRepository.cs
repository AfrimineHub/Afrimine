using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public interface ISendEmailRepository
    {
        Task CreateAsync(SendEmail entity);
    }
}
