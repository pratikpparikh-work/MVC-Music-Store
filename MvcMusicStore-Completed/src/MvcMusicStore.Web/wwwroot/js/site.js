// MvcMusicStore - Site JavaScript
// Replaces legacy MicrosoftAjax.js, MicrosoftMvcAjax.js, MicrosoftMvcValidation.js

// Cart status update helper
function updateCartStatus(cartCount) {
    var cartStatus = document.getElementById('cart-status');
    if (cartStatus) {
        cartStatus.textContent = 'Cart (' + cartCount + ')';
    }
}
