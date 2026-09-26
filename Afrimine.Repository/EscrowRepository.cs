using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class EscrowRepository : RepositoryBase<Escrow>, IEscrowRepository
    {
        public EscrowRepository(AppDbContext context) : base(context) { }
        public new async Task Create(Escrow entity) => await base.Create(entity);
        public new void Update(Escrow entity) => base.Update(entity);

        public async Task<Escrow?> GetByOrderIdAsync(Guid orderId) =>
            await FindByCondition(x => x.OrderId == orderId && !x.IsDeleted, true)
                .FirstOrDefaultAsync();
    }
}
