namespace Assignment3_Backend.Models
{
    public interface IRepository
    {
        Task<bool> SaveChangesAsync();
        
        void Add<T>(T entity) where T : class;
    }
}
