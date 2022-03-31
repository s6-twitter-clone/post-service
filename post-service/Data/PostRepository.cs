using post_service.Interfaces;
using post_service.Models;

namespace post_service.Data
{
    public class PostRepository: GenericRepository<Post>, IPostRepository
    {
        public PostRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
