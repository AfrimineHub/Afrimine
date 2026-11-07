using Afrimine.Migrations;
using Afrimine.Model.Entities;

namespace Afrimine.Repository
{
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context) { }
        public new async Task Create(Message entity) => await base.Create(entity);
        public new void Update(Message entity) => base.Update(entity);
    }
}
