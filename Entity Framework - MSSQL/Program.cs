using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace efCore_SQLITE
{

    public class ShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=ShopDb;Integrated Security=SSPI;");
        }
    }

    public class Product
    {
        [Key]
        public int Product_Id { get; set; }

        [MaxLength(45)]
        [Required]
        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Category_Id { get; set; }
    }

    public class Category
    {
        [Key]
        public int Category_Id { get; set; }

        public string CategoryName { get; set; }
    }

    public class Order
    {
        [Key]
        public int Order_Id { get; set; }
        public int ProductID { get; set; }
        public DateTime DateAdded { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            GetAllProducts();
        }

        static void AddProduct()
        {
            using (var db = new ShopContext())
            {
                var p = new Product { ProductName = "MSI GP62M", Price = 4000 };
                db.Products.Add(p);
                db.SaveChanges();

                Console.WriteLine("Veri Eklendi.");
            }
        }

        static void AddProducts()
        {
            using (var db = new ShopContext())
            {
                var products = new List<Product>()
                {
                    new Product { ProductName = "GIGABYTE B450 S2H", Price =500 },
                    new Product { ProductName = "GIGABYTE RTX 2080 Ti", Price =4000 },
                    new Product { ProductName = "NVIDIA RTX 3080", Price =8000 },
                };

                db.Products.AddRange(products);
                db.SaveChanges();

                Console.WriteLine("Veriler Eklendi.");
            }
        }

        static void GetAllProducts()
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.ToList();

                foreach (var p in products)
                {
                    Console.WriteLine($"Name: {p.ProductName} - Price: {p.Price}");
                }
            }
        }

        static void GetProductByID(int id)
        {
            using (var context = new ShopContext())
            {
                var result = context.Products.Where(p => p.Product_Id == id).FirstOrDefault();

                Console.WriteLine($"Name: {result.ProductName} - Price: {result.Price}");
            }
        }

        static void GetProductByName(string name)
        {

            using (var context = new ShopContext())
            {
                var products = context
                                    .Products
                                    .Where(p => p.ProductName.ToLower().Contains(name.ToLower()))
                                    .Select(p =>
                                    new
                                    {
                                        p.ProductName,
                                        p.Price
                                    })
                                    .ToList();


                foreach (var p in products)
                {
                    Console.WriteLine($"Name: {p.ProductName} - Price: {p.Price}");
                }
            }
        }

        static void UpdateProduct(int id)
        {
            using (var db = new ShopContext())
            {
                var entity = new Product() { Product_Id = id };
                db.Products.Attach(entity);

                entity.Price = 3000;
                db.SaveChanges();
            }





                // ----------------- SELECT ÇEKEREK UPDATE SORGUSU
                //// CHANGE TRACKING
                //var p = db.Products.Where(i => i.Product_Id == id).FirstOrDefault();
                //if (p!=null)
                //{
                //    p.Price *= 1.5m;
                //    db.SaveChanges();

                //    Console.WriteLine("Güncelleme Başarılı.");
                //}
            
        }

        static void DeleteProduct(int id)
        {
            using (var db = new ShopContext())
            {
                var p = db.Products.FirstOrDefault(i => i.Product_Id == id);
                if (p != null)
                {
                    db.Products.Remove(p);
                    db.SaveChanges();

                    Console.WriteLine("Veri Silindi.");
                }
            }
        }
    }
}

