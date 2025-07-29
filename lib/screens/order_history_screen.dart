import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';

import '../providers/order_provider.dart';
import '../providers/auth_provider.dart';
import '../providers/cart_provider.dart';
import '../models/order.dart';
import '../utils/app_theme.dart';
import '../utils/helpers.dart';

class OrderHistoryScreen extends StatefulWidget {
  const OrderHistoryScreen({super.key});

  @override
  State<OrderHistoryScreen> createState() => _OrderHistoryScreenState();
}

class _OrderHistoryScreenState extends State<OrderHistoryScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final authProvider = context.read<AuthProvider>();
      if (authProvider.isAuthenticated) {
        context.read<OrderProvider>().loadOrders(authProvider.userId!);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Order History')),
      body: Consumer2<OrderProvider, AuthProvider>(
        builder: (context, orderProvider, authProvider, child) {
          if (!authProvider.isAuthenticated) {
            return _buildNotLoggedIn();
          }

          if (orderProvider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (orderProvider.orders.isEmpty) {
            return _buildEmptyOrders();
          }

          return RefreshIndicator(
            onRefresh: () => orderProvider.loadOrders(authProvider.userId!),
            child: ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: orderProvider.orders.length,
              itemBuilder: (context, index) {
                final order = orderProvider.orders[index];
                return _buildOrderCard(order);
              },
            ),
          );
        },
      ),
    );
  }

  Widget _buildNotLoggedIn() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.person_outline, size: 80, color: Colors.grey[400]),
          const SizedBox(height: 16),
          Text(
            'Please login to view your orders',
            style: AppTheme.headingMedium.copyWith(color: Colors.grey[600]),
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: () {
              // Navigate to login screen
            },
            child: const Text('Login'),
          ),
        ],
      ),
    );
  }

  Widget _buildEmptyOrders() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.receipt_long_outlined, size: 80, color: Colors.grey[400]),
          const SizedBox(height: 16),
          Text(
            'No orders yet',
            style: AppTheme.headingMedium.copyWith(color: Colors.grey[600]),
          ),
          const SizedBox(height: 8),
          Text(
            'Start shopping to see your orders here',
            style: AppTheme.bodyMedium.copyWith(color: Colors.grey[500]),
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: () => context.pushReplacement('/'),
            child: const Text('Start Shopping'),
          ),
        ],
      ),
    );
  }

  Widget _buildOrderCard(Order order) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: InkWell(
        onTap: () => context.push('/order-tracking/${order.id}'),
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Order #${order.id.substring(0, 8)}',
                    style: AppTheme.bodyMedium.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 4,
                    ),
                    decoration: BoxDecoration(
                      color: Helpers.getOrderStatusColor(
                        order.status.toString().split('.').last,
                      ).withOpacity(0.1),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      order.statusText,
                      style: AppTheme.bodySmall.copyWith(
                        color: Helpers.getOrderStatusColor(
                          order.status.toString().split('.').last,
                        ),
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Text(
                'Placed on ${Helpers.formatDateTime(order.createdAt)}',
                style: AppTheme.bodySmall.copyWith(color: Colors.grey[600]),
              ),
              const SizedBox(height: 8),
              Text(
                '${order.items.length} item${order.items.length > 1 ? 's' : ''}',
                style: AppTheme.bodySmall,
              ),
              const SizedBox(height: 8),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Total: ${Helpers.formatCurrency(order.total)}',
                    style: AppTheme.bodyMedium.copyWith(
                      fontWeight: FontWeight.bold,
                      color: AppTheme.primaryColor,
                    ),
                  ),
                  Row(
                    children: [
                      if (order.status == OrderStatus.delivered) ...[
                        TextButton(
                          onPressed: () => _reorderItems(order),
                          child: const Text('Reorder'),
                        ),
                        const SizedBox(width: 8),
                      ],
                      TextButton(
                        onPressed: () =>
                            context.push('/order-tracking/${order.id}'),
                        child: const Text('View Details'),
                      ),
                    ],
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _reorderItems(Order order) {
    // Add all items from the order back to cart
    final cartProvider = context.read<CartProvider>();

    for (final item in order.items) {
      cartProvider.addItem(item.product, quantity: item.quantity);
    }

    Helpers.showSnackBar(context, 'Items added to cart');
    context.push('/cart');
  }
}
