using EF_OneToMany.Data.efCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EF_OneToMany
{
    public class ShopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=ShopifyDb;Integrated Security=SSPI;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                         .ToTable("Urunler"); // Db tarafındaki TABLE MAP ETMENİN, FLUENT API TARAFI.

            modelBuilder.Entity<ProductCategory>()
                         .HasKey(t => new { t.ProductId, t.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                         .HasOne(pc => pc.Product)
                         .WithMany(p => p.ProductCategories) // onModelCreating altındaki modelBuilder Db atamalarına "FLUENT API" denir.
                         .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                        .HasOne(pc => pc.Category)
                        .WithMany(c => c.ProductCategories)
                        .HasForeignKey(pc => pc.CategoryId);

            modelBuilder.Entity<Customer>()
                        .Property(p => p.IdentityNumber) //  ------ KİMLİK NO İÇİN FLUENT API ÖRNEĞİ.
                        .IsRequired()
                        .HasMaxLength(11);
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Username)
                        .IsUnique();
        }
    }

    public class Product
    {
        // [DatabaseGenerated(DatabaseGeneratedOption.None)] ----- IDENTITY OTO ARTMAZ.BU YÜZDEN VALUE ATAMADAN İŞLEM YAPAMAYIZ.

        // [DatabaseGenerated(DatabaseGeneratedOption.Identity] ----- ATANAN NESNE İÇİN +1 IDENTITY VE DEĞİŞMEZ.

        [Key] // -----------> nesne üzerindeki tanımlamalara "DATA ANNOTATION" denir.
        public int Product_Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // - DEĞİŞMEYEN IDENTITY
        public DateTime InsertedDate { get; set; } = DateTime.Now;
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;
        public List<ProductCategory> ProductCategories { get; set; }

    }

    public class Category
    {
        [Key]
        public int Category_Id { get; set; }
        public string CategoryName { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Customer Customer { get; set; }
        public List<Address> Addresses { get; set; } // NAVIGATION PROPERTY

    }

    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public User User { get; set; }  // NAVIGATION PROPERTY
        public int UserId { get; set; } // int 
    }

    public class Customer
    {
        [Key]
        [Column("CustomerID")] // Db tarafında Column Name verilir.
        public int Id { get; set; }
        //[Required] NULL GEÇİLEMEZ.
        public string IdentityNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped] // - Db Üzerinde Görmeyiz.
        public string FullName { get; set; }
        public User User { get; set; }
        public int UserId { get; set; } // USER TABLOSUYLA ONE-TO-ONE RELATION
    }

    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxNumber { get; set; }
    }

    // [NotMapped] -- Db Üzerinde Tablo Oluşturulmaz.
    [Table("UrunKategorileri")] // --- DB TARAFINDA TABLE NAME DEĞİŞİR.
    public class ProductCategory
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
    }

    public static class DataSeeding
    {
        public static void Seed(DbContext context)
        {
            if (context.Database.GetPendingMigrations().Count() == 0)
            {
                if (context is ShopContext)
                {
                    ShopContext _context = context as ShopContext;
                    if (_context.Products.Count() == 0)
                    {
                        _context.Products.AddRange(Products);
                    }
                    if (_context.Categories.Count() == 0)
                    {
                        _context.Categories.AddRange(Categories);
                    }
                }

                context.SaveChanges();

                // SHOP- CONTEXT
                // ABC VS. - CONTEXT
            }
        }

        private static Product[] Products =
        {
            new Product() { ProductName="PHILIPPS OLED TV+", Price = 12500},
            new Product() { ProductName="ViewSonic C-MHD 144Hz", Price = 1750},
            new Product() { ProductName="Bloody RGB Keyboard", Price = 250},
            new Product() { ProductName="CoolerMaster ARGB Fan Controller", Price = 400},
            new Product() { ProductName="ThermalTake US200 Gaming+", Price = 985}
        };

        private static Category[] Categories =
        {
            new Category() { CategoryName="Televizyon" },
            new Category() { CategoryName="Monitör" },
            new Category() { CategoryName="Klavye" },
            new Category() { CategoryName="Fan Controller" },
            new Category() { CategoryName="Bilgisayar"}
        };

    }

    public class CustomerDemo
    {
        public CustomerDemo()
        {
            Orders = new List<OrderDemo>();
        }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
        public List <OrderDemo> Orders { get; set; }
    }

    public class OrderDemo
    {
        public int OrderId { get; set; }
        public decimal Total { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            //// ENTITY FRAMEWORK İLE KLASİK SQL SORGUSU KULLANMA.
            //using (var db = new NorthwindContext())
            //{
            //    var city = "Madrid";
            //    var customers = db.Customers.FromSqlRaw("select * from customers where city = {0}", city).ToList();

            //    foreach (var c in customers)
            //    {
            //        Console.WriteLine(c.ContactName);
            //    }
            //}
        }


        static void InsertUsers()
        {
            var users = new List<User>
            {
                new User(){Username="atasahin", Email="atasahin@gmail.com"},
                new User(){Username="mehlikasahin", Email="info@sahin.com"},
                new User(){Username="mervesahin", Email="merve@sahin.com"}
            };

            using (var db = new ShopContext())
            {
                db.Users.AddRange(users);
                db.SaveChanges();

                Console.WriteLine("Kullanıcılar Eklendi.");
            }
        }

        static void InsertAddresses()
        {
            var addresses = new List<Address>
            {
                new Address(){ FullName ="Ata Şahin", Title = "Ev Adresi", Body = "İstanbul", UserId = 1},
                new Address(){ FullName ="Ata Şahin", Title = "İş Adresi", Body = "İstanbul", UserId = 1},
                new Address(){ FullName ="Mehlika Şahin", Title = "İş Adresi", Body = "Ankara", UserId = 2},
                new Address(){ FullName ="Merve Şahin", Title = "Ev Adresi", Body = "İzmir", UserId = 3}
            };

            using (var db = new ShopContext())
            {
                db.Addresses.AddRange(addresses);
                db.SaveChanges();

                Console.WriteLine("Adres Kayıtları Eklendi.");
            }
        }

        static void InsertAdressByUserName()
        {
            using (var db = new ShopContext())
            {
                var user = db.Users.FirstOrDefault(i => i.Username == "atasahin");

                if (user != null)
                {
                    user.Addresses = new List<Address>();

                    user.Addresses.AddRange(
                    new List<Address>(){
                        new Address() { FullName = "Ata Şahin", Title = "Ev Adresi 2", Body = "İstanbul" },
                        new Address() { FullName = "Ata Şahin", Title = "İş Adresi 2", Body = "İstanbul" }
                    });

                    db.SaveChanges();
                    Console.WriteLine("Adresler Eklendi.");
                }
            }
        }

        static void InsertCustomers(int id)
        {

            using (var db = new ShopContext())
            {
                var customer = new Customer()
                {
                    IdentityNumber = "12345",
                    FirstName = "Hawk",
                    LastName = "Technology",
                    UserId = id
                };

                db.Customers.Add(customer);
                db.SaveChanges();

                Console.WriteLine("Customer Added.");
            }
        }

        static void OneToOneInsertUserCustomer()
        {
            using (var db = new ShopContext())
            {
                var user = new User()
                {
                    Username = "ata",
                    Email = "info@atasahin.com",

                    Customer = new Customer()
                    {
                        FirstName = "Ata",
                        LastName = "Şahin",
                        IdentityNumber = "45455551564"
                    }
                };

                db.Users.Add(user);
                db.SaveChanges();

                Console.WriteLine("Kayıt Eklendi.");
            }
        }

        static void GereksizInsert()
        {
            using (var db = new ShopContext())
            {
                var products = new List<Product>()
                {
                    new Product(){ ProductName ="Asus Rog STRIX 1660", Price=4500 },
                    new Product(){ ProductName ="Ryzen 5 1600 AFBox", Price=1500 },
                    new Product(){ ProductName ="Ryzen 5 3600", Price=2400 },
                };

                db.Products.AddRange(products);

                var categories = new List<Category>()
                {
                    new Category(){ CategoryName="Ekran Kartı"},
                    new Category(){ CategoryName="İşlemci"},
                    new Category(){ CategoryName="Anakart"}
                };

                db.Categories.AddRange(categories);
                db.SaveChanges();

                Console.WriteLine("Kayıtlar Eklendi.");
            }
        }

        static void IncelemeKodlari()
        {
            //InsertUsers();
            //InsertAddresses();

            //using(var db = new ShopContext())
            //{
            //    int[] categoryIDs = new int[3] { 1, 2, 3 };

            //    var p = db.Products.Find(1);
            //    p.ProductCategories = categoryIDs.Select(cIds => new ProductCategory()
            //    {
            //        CategoryId = cIds,
            //        ProductId=p.Product_Id

            //    }).ToList();

            //    db.SaveChanges();
            //}

            //using (var db = new ShopContext())
            //{
            //var p = new Product()
            //{
            //    ProductName = "Samsung Mobile S9",
            //    Price = 6850,
            //};
            //db.Products.Add(p);
            //db.SaveChanges();

            //Console.WriteLine("Eklendi.");

            //var p = db.Products.FirstOrDefault();
            //p.ProductName = "Muratti Rosso";
            //db.SaveChanges();
            //}

            //DataSeeding.Seed(new ShopContext());

            //Console.WriteLine("DataSeed Eklendi.");
        }

        static void LinqSorguTekTablo()
        {
            // -- TÜM MÜŞTERİ KAYITLARINI GETİRİN.
            //using (var db = new NorthwindContext())
            //{
            //    var customers = db.Customers.ToList();

            //    foreach (var c in customers)
            //    {
            //        Console.WriteLine(c.ContactName);
            //    }
            //}



            // -- SADECE FIRSTNAME ve LASTNAME GETİRİN.

            //using (var db = new NorthwindContext())
            //{
            //    var employees = db.Employees.Select(c => new { c.FirstName, c.LastName });

            //    foreach (var e in employees)
            //    {
            //        Console.WriteLine(e.FirstName + " " + e.LastName);
            //    }
            //}





            //using (var db = new NorthwindContext())
            //{

            // LONDRADA YAŞAYAN PERSONELLERİ GETİRİN.
            //var employees = db.Employees.Where(e => e.City == "London").Select(s=> new {s.FirstName, s.LastName, s.HomePhone }).ToList();

            //foreach (var e in employees)
            //{
            //Console.WriteLine(e.FirstName + " " + e.LastName);
            //}





            // -- "Beverages" KATEGORİSİN AİT ÜRÜNLERİ GETİRİN.

            //var products = db.Products.Where(p => p.CategoryId == 1).Select(s => new { s.ProductName, s.Category.CategoryName }).ToList();

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName + " " + p.CategoryName);
            //}




            // EN SON EKLENEN 5 ÜRÜN BİLGİSİNİ GETİRİN.

            //var products = db.Products.OrderByDescending(i => i.ProductId).Take(5);

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName);
            //}





            // FİYATI 10 İLE 30 ARASINDA OLAN ÜRÜNLERİ GETİRİN.

            //var products = db.Products.Where(i => i.UnitPrice >= 40 && i.UnitPrice <= 60).ToList();

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName + " " + p.UnitPrice);
            //}



            // "BEVERAGES" KATEGORİSİNDEKİ ÜRÜNLERİN ORTALAMA FİYATI.

            //var ortalama = db.Products.Where(i => i.CategoryId == 1).Average(i => i.UnitPrice);

            //Console.WriteLine("Ortalama: {0}", ortalama);





            // "BEVERAGES" KATEGORİSİNDE KAÇ ÜRÜN VAR?

            //var kacUrun = db.Products.Where(i => i.CategoryId == 1).Count();
            //Console.WriteLine("Toplam Ürün: {0}", kacUrun);





            // "BEVERAGES"  veya "CONDIMENTS" KATEGORİSİNDE ÜRÜNLERİN TOPLAM FİYATI.

            //var toplam = db.Products.Where(i => i.CategoryId == 1 || i.CategoryId == 2).Sum(i => i.UnitPrice);
            //Console.WriteLine("Toplam: {0}", toplam);



            // İÇİNDE "TEA" KELİMESİ İÇEREN ÜRÜNLERİ GETİRİN.

            //var products = db.Products.Where(i => i.ProductName.Contains("Louisiana")).ToList();

            //foreach (var p in products)
            //{
            //    Console.WriteLine(p.ProductName);
            //}



            // EN PAHALI VE EN UCUZ ÜRÜNÜ LİSTELEYİN.


            //var maxPrice = db.Products.Max(m => m.UnitPrice);
            //var minPrice = db.Products.Min(m => m.UnitPrice);

            //Console.WriteLine("Max Price: {0}", maxPrice);
            //Console.WriteLine("Min Price: {0}", minPrice);
            //}

        }
    }
}
