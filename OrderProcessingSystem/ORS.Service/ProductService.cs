using ORS.Data.Models;
using ORS.Data.Repositories;
using ORS.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORS.Service
{

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Product? GetProductById(int id)
        {
            return _productRepository.GetById(id);
        }
    }
}
