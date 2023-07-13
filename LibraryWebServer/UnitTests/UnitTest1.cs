using LibraryWebServer.Controllers;
using LibraryWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace TestProject1
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        LibraryContext MakeTinyDB()
        {
            var contextOptions = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase("LibraryControllerTest")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .UseApplicationServiceProvider(NewServiceProvider())
                .Options;

            var db = new LibraryContext(contextOptions);

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Titles t = new Titles();
            t.Author = "Fake author";
            t.Title = "Fake title";
            t.Isbn = "123-5678901234";

            db.Titles.Add(t);
            db.SaveChanges();

            return db;
        }

        [Fact]
        public void Test1()
        {
            LibraryContext db = MakeTinyDB();

            HomeController c = new(db);

            var allTitlesResult = c.AllTitles() as JsonResult;


            dynamic x = allTitlesResult.Value;

            Assert.Equal(1, x.Length);
            //output.WriteLine("x:");
            //output.WriteLine(JsonConvert.SerializeObject(x, Formatting.Indented));
            //output.WriteLine("x[0]:");
            //output.WriteLine(JsonConvert.SerializeObject(x[0], Formatting.Indented));
            //output.WriteLine("x[0] properties:");
            //// Print the properties of x[0] to inspect its structure
            //var properties = x[0].GetType().GetProperties();
            //foreach (var property in properties)
            //{
            //    output.WriteLine($"Property Name: {property.Name}");
            //}

            Type xDynamicType = x.GetType();
            output.WriteLine($"x dynamic type: {xDynamicType}");

            Type xZeroDynamicType = x[0].GetType();
            output.WriteLine($"x[0] dynamic type: {xZeroDynamicType}");

            //var jsonArray = JArray.Parse(JsonConvert.SerializeObject(x));
            //var firstItem = jsonArray[0];
            //var isbn = (string)firstItem["isbn"];
            Console.WriteLine(x[0].title);
           // Assert.Equal("123-5678901234", isbn);

             Assert.Equal("123-5678901234", x[0].isbn);
            Assert.Equal("Fake title", x[0].title);

        }

        private static ServiceProvider NewServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
