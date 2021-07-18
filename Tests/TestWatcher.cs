using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Entities.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.Entities.Tests
{
    [TestClass]
    public class Watcher
    {
        [TestMethod]
        public async Task watching_works()
        {
            var watcher = DB.Watcher<Flower>("test");
            var allFlowers = new List<Flower>();

            watcher.Start(
                EventType.Created | EventType.Updated,
                f => f.FullDocument.Name == "test");

            await Task.Delay(500);

            watcher.OnChanges +=
                flowers => allFlowers.AddRange(flowers);

            await new[] {
                new Flower { Name = "test" },
                new Flower { Name = "test" },
                new Flower { Name = "test" }
            }.SaveAsync();

            var flower = new Flower { Name = "test" };
            await flower.SaveAsync();

            await flower.DeleteAsync();

            await Task.Delay(500);

            Assert.AreEqual(4, allFlowers.Count);
        }

        private class FlowerItem
        {
            public string FlowerID { get; set; }
            public string FlowerColor { get; set; }
            public string FlowerCatName { get; set; }
            public int FlowerCatCode { get; set; }
            public string FlowerName { get; set; }
        }

        [TestMethod]
        public async Task watching_with_projection_works()
        {
            var watcher = DB.Watcher<Flower, FlowerItem>("test-with-projection");
            var flowerItems = new List<FlowerItem>();

            watcher.Start(
                EventType.Created | EventType.Updated,
                f => new FlowerItem
                {
                    FlowerID = f.ID.ToString(),
                    FlowerCatCode = f.FlowerCat.Code,
                    FlowerCatName = f.FlowerCat.Name,
                    FlowerColor = f.Color
                },
                f => f.FullDocument.Color == "red");

            await Task.Delay(500);

            watcher.OnChangesAsync += async fItems =>
            {
                flowerItems.AddRange(fItems);
                await Task.CompletedTask;
            };

            await new[] {
                new Flower { Name = "test", Color = "blue", FlowerCat = new Category{ Name = "cat", Code = 1 } },
                new Flower { Name = "test", Color = "red", FlowerCat = new Category{ Name = "cat", Code = 1 } },
                new Flower { Name = "test", Color = "red", FlowerCat = new Category{ Name = "cat", Code = 1 } },
                new Flower { Name = "test", Color = "red", FlowerCat = new Category{ Name = "cat", Code = 1 } }
            }.SaveAsync();

            var flower = new Flower { Name = "test" };
            await flower.SaveAsync();
            await flower.DeleteAsync();

            await Task.Delay(500);

            Assert.AreEqual(3, flowerItems.Count);
            Assert.IsTrue(
                flowerItems[0].FlowerName == null &&
                flowerItems[0].FlowerColor == "red" &&
                flowerItems[0].FlowerCatName == "cat" &&
                flowerItems[0].FlowerCatCode == 1 &&
                flowerItems[0].FlowerID != null);
        }

        [TestMethod]
        public async Task watching_with_filter_builders()
        {
            var guid = Guid.NewGuid().ToString();

            var watcher = DB.Watcher<Flower>("test-with-filter-builders");
            var allFlowers = new List<Flower>();

            watcher.Start(
                EventType.Created | EventType.Updated,
                b => b.Eq(d => d.FullDocument.Name, guid));

            await Task.Delay(500);

            watcher.OnChanges +=
                flowers => allFlowers.AddRange(flowers);

            await new[] {
                new Flower { Name = guid },
                new Flower { Name = guid },
                new Flower { Name = guid }
            }.SaveAsync();

            var flower = new Flower { Name = guid };
            await flower.SaveAsync();

            await flower.DeleteAsync();

            await Task.Delay(500);

            Assert.AreEqual(4, allFlowers.Count);
        }

        [TestMethod]
        public async Task watching_with_filter_builders_CSD()
        {
            var guid = Guid.NewGuid().ToString();

            var watcher = DB.Watcher<Flower>("test-with-filter-builders-csd");
            var allFlowers = new List<Flower>();

            watcher.Start(
                EventType.Created | EventType.Updated,
                b => b.Eq(d => d.FullDocument.Name, guid));

            await Task.Delay(500);

            watcher.OnChangesCSDAsync += async csDocs =>
            {
                allFlowers.AddRange(csDocs.Select(x => x.FullDocument));
                await Task.CompletedTask;
            };

            await new[] {
                new Flower { Name = guid },
                new Flower { Name = "exclude me" },
                new Flower { Name = guid },
                new Flower { Name = guid },
            }.SaveAsync();

            var flower = new Flower { Name = guid };
            await flower.SaveAsync();

            await flower.DeleteAsync();

            await Task.Delay(500);

            Assert.AreEqual(4, allFlowers.Count);
        }
    }
}
