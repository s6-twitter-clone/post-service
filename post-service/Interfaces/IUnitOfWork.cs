namespace post_service.Interfaces;

public interface IUnitOfWork
{
    public IPostRepository Posts { get; }
    public int Commit();
}
