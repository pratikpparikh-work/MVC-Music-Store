using System;
using Xunit;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Models
{
    public class OrderDetailTests
    {
        [Fact]
        public void OrderDetail_DefaultConstructor_SetsDefaultValues()
        {
            var detail = new OrderDetail();
            Assert.Equal(0, detail.OrderDetailId);
            Assert.Equal(0, detail.OrderId);
            Assert.Equal(0, detail.AlbumId);
            Assert.Equal(0, detail.Quantity);
            Assert.Equal(0m, detail.UnitPrice);
            Assert.Null(detail.Album);
            Assert.Null(detail.Order);
        }

        [Fact]
        public void OrderDetail_SetOrderDetailId_ReturnsCorrectValue()
        {
            var detail = new OrderDetail();
            detail.OrderDetailId = 10;
            Assert.Equal(10, detail.OrderDetailId);
        }

        [Fact]
        public void OrderDetail_SetOrderId_ReturnsCorrectValue()
        {
            var detail = new OrderDetail();
            detail.OrderId = 5;
            Assert.Equal(5, detail.OrderId);
        }

        [Fact]
        public void OrderDetail_SetAlbumId_ReturnsCorrectValue()
        {
            var detail = new OrderDetail();
            detail.AlbumId = 3;
            Assert.Equal(3, detail.AlbumId);
        }

        [Fact]
        public void OrderDetail_SetQuantity_ReturnsCorrectValue()
        {
            var detail = new OrderDetail();
            detail.Quantity = 2;
            Assert.Equal(2, detail.Quantity);
        }

        [Fact]
        public void OrderDetail_SetUnitPrice_ReturnsCorrectValue()
        {
            var detail = new OrderDetail();
            detail.UnitPrice = 8.99m;
            Assert.Equal(8.99m, detail.UnitPrice);
        }

        [Fact]
        public void OrderDetail_SetAlbum_ReturnsCorrectAlbum()
        {
            var detail = new OrderDetail();
            var album = new Album { AlbumId = 1, Title = "Test Album" };
            detail.Album = album;
            Assert.NotNull(detail.Album);
            Assert.Equal("Test Album", detail.Album.Title);
        }

        [Fact]
        public void OrderDetail_SetOrder_ReturnsCorrectOrder()
        {
            var detail = new OrderDetail();
            var order = new Order { OrderId = 1, Username = "testuser" };
            detail.Order = order;
            Assert.NotNull(detail.Order);
            Assert.Equal(1, detail.Order.OrderId);
        }

        [Fact]
        public void OrderDetail_InitializeWithAllValues_PropertiesAreSet()
        {
            var album = new Album { AlbumId = 1, Title = "Nevermind", Price = 8.99m };
            var order = new Order { OrderId = 1 };
            var detail = new OrderDetail
            {
                OrderDetailId = 1,
                OrderId = 1,
                AlbumId = 1,
                Quantity = 3,
                UnitPrice = 8.99m,
                Album = album,
                Order = order
            };

            Assert.Equal(1, detail.OrderDetailId);
            Assert.Equal(1, detail.OrderId);
            Assert.Equal(1, detail.AlbumId);
            Assert.Equal(3, detail.Quantity);
            Assert.Equal(8.99m, detail.UnitPrice);
            Assert.Equal("Nevermind", detail.Album.Title);
            Assert.Equal(1, detail.Order.OrderId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        public void OrderDetail_SetVariousQuantities_ReturnsCorrectValue(int quantity)
        {
            var detail = new OrderDetail();
            detail.Quantity = quantity;
            Assert.Equal(quantity, detail.Quantity);
        }

        [Fact]
        public void OrderDetail_SetAlbumToNull_ReturnsNull()
        {
            var detail = new OrderDetail { Album = new Album { AlbumId = 1 } };
            detail.Album = null;
            Assert.Null(detail.Album);
        }

        [Fact]
        public void OrderDetail_SetOrderToNull_ReturnsNull()
        {
            var detail = new OrderDetail { Order = new Order { OrderId = 1 } };
            detail.Order = null;
            Assert.Null(detail.Order);
        }
    }
}
