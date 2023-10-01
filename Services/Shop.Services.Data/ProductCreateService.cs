﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shop.Data.Common.Repositories;
using Shop.Data.Models;
using Shop.Services.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Services.Data
{
    public class ProductCreateService : IProductCreateService
    {
        private const int InitialPosts = 6;
        private readonly IDeletableEntityRepository<Product> product;
        private readonly IDeletableEntityRepository<ProductImage> productImage;
        private readonly IDeletableEntityRepository<ImageProduct> imageProduct;
        private readonly Cloudinary cloudinary;

        public ProductCreateService(IDeletableEntityRepository<Product> product,
                                    IDeletableEntityRepository<ProductImage> productImage,
                                    IDeletableEntityRepository<ImageProduct> imageProduct,
                                    Cloudinary cloudinary)
        {
            this.product = product;
            this.productImage = productImage;
            this.imageProduct = imageProduct;
            this.cloudinary = cloudinary;
        }

        public IEnumerable<T> GetAllPromotedProducts<T>()
        {
            //get N number products 
            //var promotedProduct = this.product.All().Take(InitialPosts).To<T>().ToList();

            //get all product IT IS NOT PROMOTED ! FIX IT 
            var promotedProduct = this.product.All().To<T>().ToList();
            return promotedProduct;
        }

        public IEnumerable<T> GetPromotedProductsById<T>(int id, int itemsPerPage)
        {
            var product = this.product.All().Skip((id * itemsPerPage) + InitialPosts).Take(itemsPerPage).To<T>().ToList();

            return product;
        }

        public async Task<int> CreateAsync(string name, string description, decimal price, string location, string userId, IFormFile image, int brandId, string category)
        {
            int categoryId = 0;
            byte[] destinationImage;
            string[] imagePath;
            var imageName = string.Empty;

            if (image != null)
            {
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                destinationImage = memoryStream.ToArray();
            }

            using (var destinationStream = new MemoryStream(destinationImage))
            {
                var upload = new ImageUploadParams()
                {
                    File = new FileDescription(image.FileName, destinationStream),
                };
                var result = await this.cloudinary.UploadAsync(upload);
                var imageUri = result.Uri.AbsoluteUri;
                imagePath = imageUri.Split("upload/");
                imageName = imagePath[1];
            }
            }

            switch (category)
            {
                case "Electorinics":
                    categoryId = 1;
                    break;
                case "Auto":
                    categoryId = 2;
                    break;
                case "Fashon":
                    categoryId = 7;
                    break;
                case "Somthing Else":
                    categoryId = 6;
                    break;
                case "Garden":
                    categoryId = 4;
                    break;
                case "Real Estate":
                    categoryId = 3;
                    break;
                default:
                    categoryId = 9;
                    break;
            };
            //fix fk problem with brainId !
            // fix hardcoded numbers and get it from the form !
            var noImage = "NULL";

            Product addProduct = new Product
            {
                UserId = userId,
                Name = name,
                Description = description,
                Price = price,
                Location = location,
                CategoryId = 4,
                Image = noImage,
                BrandId = 1,
            };

            await this.product.AddAsync(addProduct);
            await this.product.SaveChangesAsync();
            return addProduct.Id;
        }

        public async Task<int> CreateImage(ICollection<IFormFile> images, int id)
        {
            foreach (var file in images)
            {
                byte[] destinationImage;
                string[] imagePath;
                var imageName = string.Empty;

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    destinationImage = memoryStream.ToArray();
                }

                using (var destinationStream = new MemoryStream(destinationImage))
                {
                    var upload = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, destinationStream),
                    };
                    var result = await this.cloudinary.UploadAsync(upload);
                    var test = result.Uri.AbsoluteUri;
                    imagePath = test.Split("upload/");
                    imageName = imagePath[1];
                }

                var img = new ImageProduct
                {
                    Name = imageName,
                    ProductId = id,
                };
                await this.imageProduct.AddAsync(img);
            }

            await this.imageProduct.SaveChangesAsync();

            return 1;
        }

        public T GetById<T>(int id)
        {
            var product = this.product.All().Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefault();

            return product;
        }


        public T GetByName<T>(string name)
        {
            var product = this.product.All().Where(x => x.Name == name)
                .To<T>()
                .FirstOrDefault();

            return product;
        }

        public IEnumerable<T> GetByCategory<T>(int id)
        {
            var products = this.product.All().Where(x => x.CategoryId == id).To<T>().ToList();
            return products;
        }

        public IEnumerable<T> GetUserProducts<T>(string userId)
        {
            var products = this.product.All()
                .OrderByDescending(x => x.CreatedOn)
                .Where(x => x.UserId == userId);

            return products.To<T>().ToList();
        }

        public async Task<bool> Count(string name)
        {
            var count = this.product.All().Where(x => x.Name == name).FirstOrDefault();
            count.SeenCount += 1;
            await this.product.SaveChangesAsync();
            return true;
        }

        public IEnumerable<T> MostExpensive<T>(string id,string region, int number)
        {
            if (number == 1)
            {
                var products = this.product.All().Where(x => x.Name.Contains(id) && x.Location == region).OrderByDescending(x => x.Price).To<T>().ToList();
                return products;
            }
            else if(number == 2)
            {
                var products = this.product.All().Where(x => x.Name.Contains(id) && x.Location == region).OrderBy(x => x.Price).To<T>().ToList();
                return products;
            }
            else
            {
                var products = this.product.All().Where(x => x.Name.Contains(id) && x.Location == region).OrderBy(x => x.CreatedOn).To<T>().ToList();
                return products;
            }
        }
    }
}
