import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';
import 'package:cached_network_image/cached_network_image.dart';

import '../providers/cart_provider.dart';
import '../providers/auth_provider.dart';
import '../models/cart_item.dart';
import '../utils/app_theme.dart';
import '../utils/helpers.dart';
import '../utils/constants.dart';

class CartScreen extends StatefulWidget {
  const CartScreen({super.key});

  @override
  State<CartScreen> createState() => _CartScreenState();
}

class _CartScreenState extends State<CartScreen> {
  final TextEditingController _promoController = TextEditingController();

  @override
  void dispose() {
    _promoController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Shopping Cart'),
        actions: [
          Consumer<CartProvider>(
            builder: (context, cart, child) {
              if (cart.isEmpty) return const SizedBox.shrink();

              return TextButton(
                onPressed: () => _showClearCartDialog(context),
                child: const Text(
                  'Clear All',
                  style: TextStyle(color: Colors.white),
                ),
              );
            },
          ),
        ],
      ),
      body: Consumer<CartProvider>(
        builder: (context, cart, child) {
          if (cart.isEmpty) {
            return _buildEmptyCart();
          }

          return Column(
            children: [
              Expanded(
                child: ListView.builder(
                  padding: const EdgeInsets.all(16),
                  itemCount: cart.items.length,
                  itemBuilder: (context, index) {
                    final item = cart.items[index];
                    return _buildCartItem(item, cart);
                  },
                ),
              ),
              _buildPromoCodeSection(cart),
              _buildOrderSummary(cart),
            ],
          );
        },
      ),
    );
  }

  Widget _buildEmptyCart() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.shopping_cart_outlined, size: 80, color: Colors.grey[400]),
          const SizedBox(height: 16),
          Text(
            'Your cart is empty',
            style: AppTheme.headingMedium.copyWith(color: Colors.grey[600]),
          ),
          const SizedBox(height: 8),
          Text(
            'Add some groceries to get started',
            style: AppTheme.bodyMedium.copyWith(color: Colors.grey[500]),
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: () => context.pop(),
            child: const Text('Start Shopping'),
          ),
        ],
      ),
    );
  }

  Widget _buildCartItem(CartItem item, CartProvider cart) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          children: [
            ClipRRect(
              borderRadius: BorderRadius.circular(8),
              child: CachedNetworkImage(
                imageUrl: item.product.imageUrl,
                width: 60,
                height: 60,
                fit: BoxFit.cover,
                placeholder: (context, url) => Container(
                  width: 60,
                  height: 60,
                  color: Colors.grey[200],
                  child: const Icon(Icons.image),
                ),
                errorWidget: (context, url, error) => Container(
                  width: 60,
                  height: 60,
                  color: Colors.grey[200],
                  child: const Icon(Icons.image_not_supported),
                ),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.product.name,
                    style: AppTheme.bodyMedium.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 4),
                  Text(item.product.unit, style: AppTheme.bodySmall),
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Text(
                        Helpers.formatCurrency(item.product.effectivePrice),
                        style: AppTheme.priceText,
                      ),
                      if (item.product.hasDiscount) ...[
                        const SizedBox(width: 8),
                        Text(
                          Helpers.formatCurrency(item.product.price),
                          style: AppTheme.discountText.copyWith(fontSize: 12),
                        ),
                      ],
                    ],
                  ),
                ],
              ),
            ),
            Column(
              children: [
                Container(
                  decoration: BoxDecoration(
                    border: Border.all(color: AppTheme.primaryColor),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        onPressed: () =>
                            cart.decrementQuantity(item.product.id),
                        icon: const Icon(Icons.remove, size: 18),
                        constraints: const BoxConstraints(
                          minWidth: 32,
                          minHeight: 32,
                        ),
                        padding: EdgeInsets.zero,
                      ),
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 12),
                        child: Text(
                          item.quantity.toString(),
                          style: AppTheme.bodyMedium.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      IconButton(
                        onPressed: () =>
                            cart.incrementQuantity(item.product.id),
                        icon: const Icon(Icons.add, size: 18),
                        constraints: const BoxConstraints(
                          minWidth: 32,
                          minHeight: 32,
                        ),
                        padding: EdgeInsets.zero,
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  Helpers.formatCurrency(item.totalPrice),
                  style: AppTheme.bodyMedium.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppTheme.primaryColor,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPromoCodeSection(CartProvider cart) {
    return Container(
      padding: const EdgeInsets.all(16),
      color: Colors.grey[50],
      child: Column(
        children: [
          if (cart.promoCode.isNotEmpty) ...[
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.green[50],
                border: Border.all(color: Colors.green),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  const Icon(Icons.check_circle, color: Colors.green),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      'Promo code "${cart.promoCode}" applied',
                      style: AppTheme.bodyMedium.copyWith(
                        color: Colors.green[700],
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  TextButton(
                    onPressed: () {
                      cart.removePromoCode();
                      _promoController.clear();
                    },
                    child: const Text('Remove'),
                  ),
                ],
              ),
            ),
          ] else ...[
            Row(
              children: [
                Expanded(
                  child: TextField(
                    controller: _promoController,
                    decoration: const InputDecoration(
                      hintText: 'Enter promo code',
                      border: OutlineInputBorder(),
                    ),
                    textCapitalization: TextCapitalization.characters,
                  ),
                ),
                const SizedBox(width: 12),
                ElevatedButton(
                  onPressed: () => _applyPromoCode(cart),
                  child: const Text('Apply'),
                ),
              ],
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildOrderSummary(CartProvider cart) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        boxShadow: [
          BoxShadow(
            color: Colors.grey.withOpacity(0.2),
            spreadRadius: 1,
            blurRadius: 4,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Column(
        children: [
          _buildSummaryRow('Subtotal', cart.subtotal),
          _buildSummaryRow('Tax', cart.tax),
          _buildSummaryRow('Delivery Charge', cart.deliveryCharge),
          if (cart.promoDiscount > 0)
            _buildSummaryRow('Discount', -cart.promoDiscount, isDiscount: true),
          const Divider(),
          _buildSummaryRow('Total', cart.total, isTotal: true),
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: () => _proceedToCheckout(context),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
              child: Text(
                'Proceed to Checkout (${cart.itemCount} items)',
                style: AppTheme.buttonText,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSummaryRow(
    String label,
    double amount, {
    bool isTotal = false,
    bool isDiscount = false,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: isTotal
                ? AppTheme.bodyLarge.copyWith(fontWeight: FontWeight.bold)
                : AppTheme.bodyMedium,
          ),
          Text(
            Helpers.formatCurrency(amount.abs()),
            style: isTotal
                ? AppTheme.bodyLarge.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppTheme.primaryColor,
                  )
                : AppTheme.bodyMedium.copyWith(
                    color: isDiscount ? Colors.green : null,
                  ),
          ),
        ],
      ),
    );
  }

  Future<void> _applyPromoCode(CartProvider cart) async {
    final code = _promoController.text.trim();
    if (code.isEmpty) return;

    final success = await cart.applyPromoCode(code);
    if (success) {
      Helpers.showSnackBar(context, 'Promo code applied successfully!');
    } else {
      Helpers.showSnackBar(context, 'Invalid promo code', isError: true);
    }
  }

  Future<void> _showClearCartDialog(BuildContext context) async {
    final confirmed = await Helpers.showConfirmationDialog(
      context,
      title: 'Clear Cart',
      message: 'Are you sure you want to remove all items from your cart?',
      confirmText: 'Clear',
      cancelText: 'Cancel',
    );

    if (confirmed) {
      context.read<CartProvider>().clearCart();
      Helpers.showSnackBar(context, 'Cart cleared');
    }
  }

  void _proceedToCheckout(BuildContext context) {
    final authProvider = context.read<AuthProvider>();

    if (!authProvider.isAuthenticated) {
      Helpers.showSnackBar(context, 'Please login to continue', isError: true);
      // Navigate to login screen
      return;
    }

    context.push('/checkout');
  }
}
