﻿using System;
using System.Linq;
using RedisApp.Data;
using RedisApp.Cache;
using RedisApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace RedisApp.Controllers
{

        [Route("api/[controller]")]
        [ApiController]
        public class ProductController : ControllerBase
        {
            private readonly DbContextClass _dbContext;
            private readonly ICacheService _cacheService;
            public ProductController(DbContextClass dbContext, ICacheService cacheService)
            {
                _dbContext = dbContext;
                _cacheService = cacheService;
            }
            [HttpGet("products")]
            public IEnumerable<Product> Get()
            {
                var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
                if (cacheData != null)
                {
                    return cacheData;
                }
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                cacheData = _dbContext.Products.ToList();
                _cacheService.SetData<IEnumerable<Product>>("product", cacheData, expirationTime);
                return cacheData;
            }
            [HttpGet("product")]
            public Product Get(int id)
            {
                Product filteredData;
                var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
                if (cacheData != null)
                {
                    filteredData = cacheData.Where(x => x.ProductId == id).FirstOrDefault();
                    return filteredData;
                }
                filteredData = _dbContext.Products.Where(x => x.ProductId == id).FirstOrDefault();
                return filteredData;
            }
            [HttpPost("addproduct")]
            public async Task<Product> Post(Product value)
            {
                var obj = await _dbContext.Products.AddAsync(value);
                _cacheService.RemoveData("product");
                _dbContext.SaveChanges();
                return obj.Entity;
            }
            [HttpPut("updateproduct")]
            public void Put(Product product)
            {
                _dbContext.Products.Update(product);
                _cacheService.RemoveData("product");
                _dbContext.SaveChanges();
            }
            [HttpDelete("deleteproduct")]
            public void Delete(int Id)
            {
                var filteredData = _dbContext.Products.Where(x => x.ProductId == Id).FirstOrDefault();
                _dbContext.Remove(filteredData);
                _cacheService.RemoveData("product");
                _dbContext.SaveChanges();
            }
        }
    }