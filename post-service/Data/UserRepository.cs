using post_service.Interfaces;
using post_service.Models;

namespace post_service.Data
{
    public class UserRepository: GenericRepository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context): base(context)
        {
        }
    }
}
