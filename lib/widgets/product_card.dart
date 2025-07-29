import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:cached_network_image/cached_network_image.dart';

import '../models/product.dart';
import '../providers/cart_provider.dart';
import '../utils/app_theme.dart';
import '../utils/helpers.dart';
import '../utils/constants.dart';

class ProductCard extends StatelessWidget {
  final Product product;
  final VoidCallback onTap;

  const ProductCard({super.key, required this.product, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildProductImage(),
            Expanded(
              child: Padding(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _buildProductName(),
                    const SizedBox(height: 4),
                    _buildProductUnit(),
                    const SizedBox(height: 8),
                    _buildPriceSection(),
                    const Spacer(),
                    _buildAddToCartButton(context),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildProductImage() {
    return Stack(
      children: [
        ClipRRect(
          borderRadius: const BorderRadius.vertical(top: Radius.circular(12)),
          child: CachedNetworkImage(
            imageUrl: product.imageUrl,
            height: 120,
            width: double.infinity,
            fit: BoxFit.cover,
            placeholder: (context, url) => Container(
              height: 120,
              color: Colors.grey[200],
              child: const Center(child: CircularProgressIndicator()),
            ),
            errorWidget: (context, url, error) => Container(
              height: 120,
              color: Colors.grey[200],
              child: const Icon(
                Icons.image_not_supported,
                size: 40,
                color: Colors.grey,
              ),
            ),
          ),
        ),
        if (product.hasDiscount)
          Positioned(
            top: 8,
            left: 8,
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
              decoration: BoxDecoration(
                color: Colors.red,
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(
                '${product.discountPercentage.toInt()}% OFF',
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 10,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ),
        if (!product.isAvailable)
          Positioned.fill(
            child: Container(
              decoration: BoxDecoration(
                color: Colors.black54,
                borderRadius: const BorderRadius.vertical(
                  top: Radius.circular(12),
                ),
              ),
              child: const Center(
                child: Text(
                  'Out of Stock',
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
          ),
      ],
    );
  }

  Widget _buildProductName() {
    return Text(
      product.name,
      style: AppTheme.bodyMedium.copyWith(fontWeight: FontWeight.w600),
      maxLines: 2,
      overflow: TextOverflow.ellipsis,
    );
  }

  Widget _buildProductUnit() {
    return Text(product.unit, style: AppTheme.bodySmall);
  }

  Widget _buildPriceSection() {
    return Row(
      children: [
        Text(
          Helpers.formatCurrency(product.effectivePrice),
          style: AppTheme.priceText,
        ),
        if (product.hasDiscount) ...[
          const SizedBox(width: 8),
          Text(
            Helpers.formatCurrency(product.price),
            style: AppTheme.discountText,
          ),
        ],
      ],
    );
  }

  Widget _buildAddToCartButton(BuildContext context) {
    return Consumer<CartProvider>(
      builder: (context, cart, child) {
        final quantity = cart.getQuantity(product.id);

        if (quantity == 0) {
          return SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: product.isAvailable
                  ? () {
                      cart.addItem(product);
                      Helpers.showSnackBar(context, 'Added to cart');
                    }
                  : null,
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 8),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
              ),
              child: const Text('Add to Cart'),
            ),
          );
        }

        return Container(
          decoration: BoxDecoration(
            border: Border.all(color: AppTheme.primaryColor),
            borderRadius: BorderRadius.circular(8),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              IconButton(
                onPressed: () => cart.decrementQuantity(product.id),
                icon: const Icon(Icons.remove, size: 18),
                constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
                padding: EdgeInsets.zero,
              ),
              Text(
                quantity.toString(),
                style: AppTheme.bodyMedium.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              IconButton(
                onPressed: () => cart.incrementQuantity(product.id),
                icon: const Icon(Icons.add, size: 18),
                constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
                padding: EdgeInsets.zero,
              ),
            ],
          ),
        );
      },
    );
  }
}
