using System.Collections.Generic;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Web.ViewModels
{
    /// <summary>
    /// View model for the shopping cart index page.
    /// Migrated from legacy ShoppingCartViewModel.
    /// </summary>
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; } = new();
        public decimal CartTotal { get; set; }
    }

    /// <summary>
    /// View model for the AJAX remove-from-cart response.
    /// Migrated from legacy ShoppingCartRemoveViewModel.
    /// </summary>
    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; } = string.Empty;
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
    }
}
