﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using antique_api.DBContext;
using antique_api.Helpers;
using antique_api.Models.Antique;
using antique_api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace antique_api.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CTX _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;


        public ProductsController(CTX context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            this._configuration = configuration;
            this._env = env;
        }

        [HttpGet]

        [Route("")]
        public async Task<ActionResult> GetProducts()
        {
            var model = _context.Products.Select(p => new
            {
                p.ID,
                p.Name,
                p.Price,
                p.Description,
                p.Category,
                p.Photos
            }).OrderByDescending(x => x.ID);


            return Ok(model.ToList());

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            //return _context.Products.Include("Photos").FirstOrDefault(x=>x.ID == id);
            var model = _context.Products.Select(p => new
            {
                p.ID,
                p.Name,
                p.Price,
                p.Description,
                p.Category,
                p.Photos
            }).FirstOrDefault(x => x.ID == id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        [HttpGet("GetByCategory/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            //List<Product> products = await _context.Products.Include("Category").Where(x => x.Category.Name == category).Include("Photos").ToListAsync();

            var model = _context.Products.Select(p => new
            {
                p.ID,
                p.Name,
                p.Price,
                p.Description,
                p.Category,
                p.Photos
            }).Where(x => x.Category.Name == category).OrderByDescending(x => x.ID);


            if (model == null)
            {
                return NotFound();
            }

            return Ok(model);
        }
        [HttpDelete]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult DeleteProduct(int id)
        {
            Product dproduct = _context.Products.Include("Photos").FirstOrDefault(x => x.ID == id);
            if (dproduct != null)
            {
                _context.Products.Remove(dproduct);
                foreach (var x in dproduct.Photos)
                {
                    InitStaticFiles.DeleteImageByFileName(_env, _configuration,
                                            new string[] { "ImagesPath", "ImagesPathProduct" },
                                            x.Path);
                }
                _context.SaveChanges();
                return Ok();

            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Authorize]

        [Route("editTest/{id}")]
        public async Task<ActionResult<Product>> Edit(int id, [FromBody] ProductModel model)
        {
            Product product = _context.Products.Include("Photos").FirstOrDefault(x => x.ID == id);

            using (CTX db = _context)
            {
                if (product != null)
                {
                    List<Photo> added_photos = new List<Photo>();
                    try
                    {
                        foreach (var x in product.Photos)
                        {
                            InitStaticFiles.DeleteImageByFileName(_env, _configuration,
                                                    new string[] { "ImagesPath", "ImagesPathProduct" },
                                                    x.Path);
                        }
                        foreach (var photo in model.ImgsBase64)
                        {

                            string imageName = Path.GetRandomFileName() + ".jpg";

                            string pathSaveImages = InitStaticFiles
                                       .CreateImageByFileName(_env, _configuration,
                                            new string[] { "ImagesPath", "ImagesPathProduct" },
                                            imageName,
                                            photo);
                            added_photos.Add(new Photo
                            {
                                Path = imageName
                            });

                        }
                    }
                    catch (Exception)
                    {
                        return BadRequest();
                    }

                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.Price = model.Price;
                    product.Photos.Clear();
                    product.Photos = added_photos;


                    db.SaveChanges();
                    return Ok(product);
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [HttpPut]
        [Authorize]

        [Route("add")]
        public async Task<ActionResult<Product>> AddProduct([FromBody] ProductModel product)
        {
            Product p;
            try
            {
                List<Photo> added_photos = new List<Photo>();
                foreach (var photo in product.ImgsBase64)
                {

                    string imageName = Path.GetRandomFileName() + ".jpg";

                    string pathSaveImages = InitStaticFiles
                               .CreateImageByFileName(_env, _configuration,
                                    new string[] { "ImagesPath", "ImagesPathProduct" },
                                    imageName,
                                    photo);
                    added_photos.Add(new Photo
                    {
                        Path = imageName
                    });

                }
                p = new Product
                {
                    Name = product.Name,
                    Category = _context.Categories.FirstOrDefault(x => x.Name == product.Category),
                    Description = product.Description,
                    Price = product.Price,
                    Photos = added_photos
                };
                _context.Products.Add(p);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return p;
        }

    }
}
