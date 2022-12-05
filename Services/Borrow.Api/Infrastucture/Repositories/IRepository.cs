namespace BookOnline.Borrowing.Api.Infrastucture.Repositories
{
    public interface IRepository<T> where T : IRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
