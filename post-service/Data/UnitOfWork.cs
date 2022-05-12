using post_service.Interfaces;

namespace post_service.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext context;

    public IPostRepository Posts { get; }
    public IUserRepository Users { get; }


    public UnitOfWork(DatabaseContext context)
    {
        this.context = context;

        Posts = new PostRepository(context);
        Users = new UserRepository(context);   
    }

    

    public int Commit()
    {
      return context.SaveChanges();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
