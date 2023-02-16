using eCommerceShopAdminProject.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceShopAdminProject.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44376/api/admin/getallactiveproducts";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Product>>().Result;

            return View(data);
        }

        public IActionResult ImportProducts(IFormFile file)
        {
            string uploadPath = $"{Directory.GetCurrentDirectory()}\\{file.FileName}";

            using(FileStream fileStream = System.IO.File.Create(uploadPath))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            List<Product> products = getAllProductsFromFile(file.FileName);

            HttpClient client = new HttpClient();

            string URL = "https://localhost:44376/api/admin/ImportProducts";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Product");
        }

        private List<Product> getAllProductsFromFile(string fileName)
        {
            List<Product> products = new List<Product>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using(var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while(reader.Read())
                    {
                        products.Add(new Models.Product
                        {
                            Name = reader.GetValue(0).ToString(),
                            Image = reader.GetValue(1).ToString(),
                            Description = reader.GetValue(2).ToString(),
                            Rating = (int)(double)reader.GetValue(3),
                            Price = (int)(double)reader.GetValue(4),
                            Category = reader.GetValue(5).ToString()
                        });
                    }
                }
            }

            return products;
        }

        public IActionResult DeleteProduct(Guid productId)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44376/api/admin/deleteactiveproduct";
            var model = new
            {
                Id = productId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Product");
        }

        public IActionResult GetProduct(Guid productId)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44376/api/admin/getproduct";
            var model = new
            {
                Id = productId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Product>().Result;

            return View(data);
        }

        public IActionResult EditProduct([Bind("Id,Name,Image,Description,Rating,Price,Category")] Product model)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44376/api/admin/editproduct";
            var product = new
            {
                Id = model.Id,
                Name = model.Name,
                Image = model.Image,
                Description = model.Description,
                Rating = model.Rating,
                Price = model.Price,
                Category = model.Category
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsStringAsync().Result;

            return RedirectToAction("Index", "Product");
        }
    }
}
