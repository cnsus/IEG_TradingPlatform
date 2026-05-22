namespace FtpProductCatalogService.Services
{
    public interface IProductCatalogRepository
    {
        Task<IEnumerable<string>> GetAllAsync();
    }
}
