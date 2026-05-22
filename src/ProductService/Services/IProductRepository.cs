namespace ProductService.Services
{
    public interface IProductRepository
    {
        IEnumerable<string> GetAll();
    }
}
