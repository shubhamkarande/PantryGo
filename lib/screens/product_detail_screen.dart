import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:go_router/go_router.dart';

import '../providers/product_provider.dart';
import '../providers/cart_provider.dart';
import '../models/product.dart';
import '../utils/app_theme.dart';
import '../utils/helpers.dart';

class ProductDetailScreen extends StatefulWidget {
  final String productId;

  const ProductDetailScreen({super.key, required this.productId});

  @override
  State<ProductDetailScreen> createState() => _ProductDetailScreenState();
}

class _ProductDetailScreenState extends State<ProductDetailScreen> {
  Product? product;

  @override
  void initState() {
    super.initState();
    _loadProduct();
  }

  void _loadProduct() {
    final productProvider = context.read<ProductProvider>();
    product = productProvider.getProductById(widget.productId);

    if (product == null) {
      // Product not found, navigate back
      WidgetsBinding.instance.addPostFrameCallback((_) {
        context.pop();
        Helpers.showSnackBar(context, 'Product not found', isError: true);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    if (product == null) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    return Scaffold(
      appBar: AppBar(
        title: Text(product!.name),
        actions: [
          IconButton(
            icon: const Icon(Icons.share),
            onPressed: () {
              // Share product
            },
          ),
        ],
      ),
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildProductImage(),
            Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildProductHeader(),
                  const SizedBox(height: 16),
                  _buildPriceSection(),
                  const SizedBox(height: 16),
                  _buildRatingSection(),
                  const SizedBox(height: 16),
                  _buildDescriptionSection(),
                  const SizedBox(height: 16),
                  _buildAvailabilitySection(),
                  const SizedBox(height: 24),
                ],
              ),
            ),
          ],
        ),
      ),
      bottomNavigationBar: _buildBottomBar(),
    );
  }

  Widget _buildProductImage() {
    return Container(
      height: 300,
      width: double.infinity,
      color: Colors.grey[100],
      child: Stack(
        children: [
          CachedNetworkImage(
            imageUrl: product!.imageUrl,
            height: 300,
            width: double.infinity,
            fit: BoxFit.cover,
            placeholder: (context, url) => Container(
              height: 300,
              color: Colors.grey[200],
              child: const Center(child: CircularProgressIndicator()),
            ),
            errorWidget: (context, url, error) => Container(
              height: 300,
              color: Colors.grey[200],
              child: const Icon(
                Icons.image_not_supported,
                size: 80,
                color: Colors.grey,
              ),
            ),
          ),
          if (product!.hasDiscount)
            Positioned(
              top: 16,
              left: 16,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 12,
                  vertical: 6,
                ),
                decoration: BoxDecoration(
                  color: Colors.red,
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Text(
                  '${product!.discountPercentage.toInt()}% OFF',
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 12,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
          if (!product!.isAvailable)
            Positioned.fill(
              child: Container(
                color: Colors.black54,
                child: const Center(
                  child: Text(
                    'Out of Stock',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildProductHeader() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(product!.name, style: AppTheme.headingLarge),
        const SizedBox(height: 4),
        Text(
          product!.unit,
          style: AppTheme.bodyMedium.copyWith(color: Colors.grey[600]),
        ),
        const SizedBox(height: 8),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
          decoration: BoxDecoration(
            color: AppTheme.primaryColor.withOpacity(0.1),
            borderRadius: BorderRadius.circular(4),
          ),
          child: Text(
            product!.category,
            style: AppTheme.bodySmall.copyWith(
              color: AppTheme.primaryColor,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildPriceSection() {
    return Row(
      children: [
        Text(
          Helpers.formatCurrency(product!.effectivePrice),
          style: AppTheme.headingMedium.copyWith(
            color: AppTheme.primaryColor,
            fontWeight: FontWeight.bold,
          ),
        ),
        if (product!.hasDiscount) ...[
          const SizedBox(width: 12),
          Text(
            Helpers.formatCurrency(product!.price),
            style: AppTheme.bodyMedium.copyWith(
              color: Colors.grey,
              decoration: TextDecoration.lineThrough,
            ),
          ),
          const SizedBox(width: 8),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
            decoration: BoxDecoration(
              color: Colors.green,
              borderRadius: BorderRadius.circular(4),
            ),
            child: Text(
              'Save ${Helpers.formatCurrency(product!.price - product!.effectivePrice)}',
              style: const TextStyle(
                color: Colors.white,
                fontSize: 12,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ],
    );
  }

  Widget _buildRatingSection() {
    return Row(
      children: [
        Row(
          children: List.generate(5, (index) {
            return Icon(
              index < product!.rating.floor()
                  ? Icons.star
                  : index < product!.rating
                  ? Icons.star_half
                  : Icons.star_border,
              color: Colors.amber,
              size: 20,
            );
          }),
        ),
        const SizedBox(width: 8),
        Text(
          '${product!.rating}',
          style: AppTheme.bodyMedium.copyWith(fontWeight: FontWeight.w600),
        ),
        const SizedBox(width: 4),
        Text(
          '(${product!.reviewCount} reviews)',
          style: AppTheme.bodySmall.copyWith(color: Colors.grey[600]),
        ),
      ],
    );
  }

  Widget _buildDescriptionSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text('Description', style: AppTheme.headingSmall),
        const SizedBox(height: 8),
        Text(
          product!.description,
          style: AppTheme.bodyMedium.copyWith(
            color: Colors.grey[700],
            height: 1.5,
          ),
        ),
      ],
    );
  }

  Widget _buildAvailabilitySection() {
    return Row(
      children: [
        Icon(
          product!.isAvailable ? Icons.check_circle : Icons.cancel,
          color: product!.isAvailable ? Colors.green : Colors.red,
          size: 20,
        ),
        const SizedBox(width: 8),
        Text(
          product!.isAvailable
              ? 'In Stock (${product!.stockQuantity} available)'
              : 'Out of Stock',
          style: AppTheme.bodyMedium.copyWith(
            color: product!.isAvailable ? Colors.green : Colors.red,
            fontWeight: FontWeight.w600,
          ),
        ),
      ],
    );
  }

  Widget _buildBottomBar() {
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
      child: Consumer<CartProvider>(
        builder: (context, cart, child) {
          final quantity = cart.getQuantity(product!.id);

          return Row(
            children: [
              if (quantity > 0) ...[
                Container(
                  decoration: BoxDecoration(
                    border: Border.all(color: AppTheme.primaryColor),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    children: [
                      IconButton(
                        onPressed: () => cart.decrementQuantity(product!.id),
                        icon: const Icon(Icons.remove),
                      ),
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        child: Text(
                          quantity.toString(),
                          style: AppTheme.bodyLarge.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      IconButton(
                        onPressed: () => cart.incrementQuantity(product!.id),
                        icon: const Icon(Icons.add),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: 16),
              ],
              Expanded(
                child: ElevatedButton(
                  onPressed: product!.isAvailable
                      ? () {
                          if (quantity == 0) {
                            cart.addItem(product!);
                            Helpers.showSnackBar(context, 'Added to cart');
                          } else {
                            context.push('/cart');
                          }
                        }
                      : null,
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(vertical: 16),
                  ),
                  child: Text(
                    quantity == 0 ? 'Add to Cart' : 'Go to Cart',
                    style: AppTheme.buttonText,
                  ),
                ),
              ),
            ],
          );
        },
      ),
    );
  }
}
