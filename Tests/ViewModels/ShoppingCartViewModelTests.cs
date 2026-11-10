using System.Collections.Generic;
using Xunit;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Tests.ViewModels
{
    public class ShoppingCartViewModelTests
    {
        [Fact]
        public void ShoppingCartViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var viewModel = new ShoppingCartViewModel();
            Assert.NotNull(viewModel.CartItems);
            Assert.Empty(viewModel.CartItems);
            Assert.Equal(0m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_SetCartTotal_ReturnsCorrectValue()
        {
            var viewModel = new ShoppingCartViewModel();
            viewModel.CartTotal = 49.99m;
            Assert.Equal(49.99m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_AddCartItem_ItemIsInList()
        {
            var viewModel = new ShoppingCartViewModel();
            var cartItem = new Cart { RecordId = 1, CartId = "test", AlbumId = 1, Count = 2 };
            viewModel.CartItems.Add(cartItem);
            Assert.Single(viewModel.CartItems);
        }

        [Fact]
        public void ShoppingCartViewModel_SetCartItems_ReturnsCorrectList()
        {
            var cartItems = new List<Cart>
            {
                new Cart { RecordId = 1, CartId = "test", AlbumId = 1, Count = 1 },
                new Cart { RecordId = 2, CartId = "test", AlbumId = 2, Count = 3 }
            };
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cartItems,
                CartTotal = 35.96m
            };
            Assert.Equal(2, viewModel.CartItems.Count);
            Assert.Equal(35.96m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_CartTotalIsZero_WhenNoItems()
        {
            var viewModel = new ShoppingCartViewModel();
            Assert.Equal(0m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_SetCartTotalToLargeValue_ReturnsCorrectValue()
        {
            var viewModel = new ShoppingCartViewModel();
            viewModel.CartTotal = 9999.99m;
            Assert.Equal(9999.99m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartViewModel_CartItems_CanHoldMultipleItems()
        {
            var viewModel = new ShoppingCartViewModel();
            for (int i = 1; i <= 5; i++)
            {
                viewModel.CartItems.Add(new Cart { RecordId = i, CartId = "test", AlbumId = i, Count = 1 });
            }
            Assert.Equal(5, viewModel.CartItems.Count);
        }
    }

    public class ShoppingCartRemoveViewModelTests
    {
        [Fact]
        public void ShoppingCartRemoveViewModel_DefaultConstructor_SetsDefaultValues()
        {
            var viewModel = new ShoppingCartRemoveViewModel();
            Assert.Equal(string.Empty, viewModel.Message);
            Assert.Equal(0m, viewModel.CartTotal);
            Assert.Equal(0, viewModel.CartCount);
            Assert.Equal(0, viewModel.ItemCount);
            Assert.Equal(0, viewModel.DeleteId);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetMessage_ReturnsCorrectValue()
        {
            var viewModel = new ShoppingCartRemoveViewModel();
            viewModel.Message = "Item removed from cart.";
            Assert.Equal("Item removed from cart.", viewModel.Message);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetCartTotal_ReturnsCorrectValue()
        {
            var viewModel = new ShoppingCartRemoveViewModel();
            viewModel.CartTotal = 25.99m;
            Assert.Equal(25.99m, viewModel.CartTotal);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetCartCount_ReturnsCorrectValue()
        {
            var viewModel = new ShoppingCartRemoveViewModel();
            viewModel.CartCount = 3;
            Assert.Equal(3, viewModel.CartCount);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetItemCount_ReturnsCorrectValue()
        {
            var viewModel = new ShoppingCartRemoveViewModel();
            viewModel.ItemCount = 2;
            Assert.Equal(2, viewModel.ItemCount);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_SetDeleteId_ReturnsCorrectValue()
        {
            var viewModel = new ShoppingCartRemoveViewModel();
            viewModel.DeleteId = 42;
            Assert.Equal(42, viewModel.DeleteId);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_InitializeWithAllValues_PropertiesAreSet()
        {
            var viewModel = new ShoppingCartRemoveViewModel
            {
                Message = "Album removed.",
                CartTotal = 17.98m,
                CartCount = 2,
                ItemCount = 1,
                DeleteId = 5
            };
            Assert.Equal("Album removed.", viewModel.Message);
            Assert.Equal(17.98m, viewModel.CartTotal);
            Assert.Equal(2, viewModel.CartCount);
            Assert.Equal(1, viewModel.ItemCount);
            Assert.Equal(5, viewModel.DeleteId);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_CartCountIsZero_WhenCartEmpty()
        {
            var viewModel = new ShoppingCartRemoveViewModel { CartCount = 0 };
            Assert.Equal(0, viewModel.CartCount);
        }

        [Fact]
        public void ShoppingCartRemoveViewModel_ItemCountIsZero_WhenItemRemoved()
        {
            var viewModel = new ShoppingCartRemoveViewModel { ItemCount = 0 };
            Assert.Equal(0, viewModel.ItemCount);
        }
    }
}
