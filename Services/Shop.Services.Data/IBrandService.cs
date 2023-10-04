﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Data
{
    public interface IBrandService
    {
        Task<int> Create(string name, string description, string userId, IFormFile logo);

        T BrandById<T>(int id);

        IEnumerable<T> AllBrands<T>();

        IEnumerable<T> GetBestBrands<T>();

        IEnumerable<T> AllProduct<T>(int id);

        IEnumerable<T> BrandsByUserId<T>(string id);
    }
}
