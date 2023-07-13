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

            var jsonArray = JArray.Parse(JsonConvert.SerializeObject(x));
            var firstItem = jsonArray[0];
            var author = (string)firstItem["author"];
            var isbn = (string)firstItem["isbn"];
            var title = (string)firstItem["title"];

            Assert.Equal("123-5678901234", isbn);
            Assert.Equal("Fake author", author);
            Assert.Equal("Fake title", title);
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
