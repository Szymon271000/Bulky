using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository productRepository { get; }

        ICategoryRepository categoryRepository { get; }

        ICompanyRepository companyRepository { get; }

        IShoppingCartRepository shoppingCartRepository { get; }
        void Save();
    }
}
