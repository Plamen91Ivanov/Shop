using Shop.Data.Models;
using Shop.Services.Mapping;
using Shop.Web.ViewModels.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Web.ViewModels.Category
{
    public class CategoriesViewModel : IMapFrom<Shop.Data.Models.Category>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ProductInputModel> Products { get; set; }
    }
}
