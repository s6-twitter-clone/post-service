namespace post_service.Interfaces;

public interface IUnitOfWork
{
    public IPostRepository Posts { get; }
    public IUserRepository Users { get; }
    public int Commit();
}
