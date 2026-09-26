using Afrimine.Migrations;
using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public class SendEmailRepository : RepositoryBase<SendEmail> , ISendEmailRepository
    {
        public SendEmailRepository(AppDbContext appDbContext) : base(appDbContext)
        {}

        public async Task CreateAsync(SendEmail entity) => await Create(entity);
    }
}
