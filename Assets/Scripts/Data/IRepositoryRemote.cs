using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepositoryRemote
{
    Task<List<ItemRemote>> GetProductsRemoteAsync();
}
