import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';

import '../providers/order_provider.dart';
import '../models/order.dart';
import '../utils/app_theme.dart';
import '../utils/helpers.dart';
import '../utils/constants.dart';

class OrderTrackingScreen extends StatefulWidget {
  final String orderId;

  const OrderTrackingScreen({super.key, required this.orderId});

  @override
  State<OrderTrackingScreen> createState() => _OrderTrackingScreenState();
}

class _OrderTrackingScreenState extends State<OrderTrackingScreen> {
  Order? order;

  @override
  void initState() {
    super.initState();
    _loadOrder();
  }

  void _loadOrder() {
    final orderProvider = context.read<OrderProvider>();
    order = orderProvider.getOrderById(widget.orderId);

    if (order == null) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        context.pop();
        Helpers.showSnackBar(context, 'Order not found', isError: true);
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    if (order == null) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    return Scaffold(
      appBar: AppBar(
        title: Text('Order #${order!.id.substring(0, 8)}'),
        actions: [
          IconButton(
            icon: const Icon(Icons.share),
            onPressed: () {
              // Share order details
            },
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () async {
          // Refresh order status
          setState(() {});
        },
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildOrderStatusCard(),
              const SizedBox(height: 20),
              _buildOrderProgressTracker(),
              const SizedBox(height: 20),
              _buildDeliveryInfoCard(),
              const SizedBox(height: 20),
              _buildOrderItemsCard(),
              const SizedBox(height: 20),
              _buildOrderSummaryCard(),
              const SizedBox(height: 20),
              if (order!.canCancel) _buildCancelOrderButton(),
            ],
          ),
        ),
      ),
      bottomNavigationBar: _buildBottomBar(),
    );
  }

  Widget _buildOrderStatusCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(
                  Helpers.getOrderStatusIcon(
                    order!.status.toString().split('.').last,
                  ),
                  color: Helpers.getOrderStatusColor(
                    order!.status.toString().split('.').last,
                  ),
                  size: 24,
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        order!.statusText,
                        style: AppTheme.headingSmall.copyWith(
                          color: Helpers.getOrderStatusColor(
                            order!.status.toString().split('.').last,
                          ),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(_getStatusMessage(), style: AppTheme.bodySmall),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Order placed on', style: AppTheme.bodySmall),
                Text(
                  Helpers.formatDateTime(order!.createdAt),
                  style: AppTheme.bodySmall.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
            if (order!.deliveredAt != null) ...[
              const SizedBox(height: 4),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text('Delivered on', style: AppTheme.bodySmall),
                  Text(
                    Helpers.formatDateTime(order!.deliveredAt!),
                    style: AppTheme.bodySmall.copyWith(
                      fontWeight: FontWeight.w600,
                      color: Colors.green,
                    ),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildOrderProgressTracker() {
    final statuses = [
      OrderStatus.placed,
      OrderStatus.confirmed,
      OrderStatus.packed,
      OrderStatus.outForDelivery,
      OrderStatus.delivered,
    ];

    final currentIndex = statuses.indexOf(order!.status);

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Order Progress', style: AppTheme.headingSmall),
            const SizedBox(height: 16),
            Column(
              children: statuses.asMap().entries.map((entry) {
                final index = entry.key;
                final status = entry.value;
                final isCompleted = index <= currentIndex;
                final isCurrent = index == currentIndex;

                return Row(
                  children: [
                    Column(
                      children: [
                        Container(
                          width: 24,
                          height: 24,
                          decoration: BoxDecoration(
                            color: isCompleted
                                ? AppTheme.primaryColor
                                : Colors.grey[300],
                            shape: BoxShape.circle,
                          ),
                          child: Icon(
                            isCompleted ? Icons.check : Icons.circle,
                            color: Colors.white,
                            size: 16,
                          ),
                        ),
                        if (index < statuses.length - 1)
                          Container(
                            width: 2,
                            height: 40,
                            color: isCompleted
                                ? AppTheme.primaryColor
                                : Colors.grey[300],
                          ),
                      ],
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            _getStatusTitle(status),
                            style: AppTheme.bodyMedium.copyWith(
                              fontWeight: isCurrent
                                  ? FontWeight.bold
                                  : FontWeight.normal,
                              color: isCompleted
                                  ? AppTheme.primaryColor
                                  : Colors.grey[600],
                            ),
                          ),
                          if (isCurrent) ...[
                            const SizedBox(height: 4),
                            Text(
                              _getStatusMessage(),
                              style: AppTheme.bodySmall.copyWith(
                                color: Colors.grey[600],
                              ),
                            ),
                          ],
                        ],
                      ),
                    ),
                  ],
                );
              }).toList(),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildDeliveryInfoCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Delivery Information', style: AppTheme.headingSmall),
            const SizedBox(height: 12),
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(Icons.location_on, color: Colors.grey),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        order!.deliveryAddress.name,
                        style: AppTheme.bodyMedium.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        order!.deliveryAddress.fullAddress,
                        style: AppTheme.bodySmall,
                      ),
                      const SizedBox(height: 4),
                      Text(
                        order!.deliveryAddress.phone,
                        style: AppTheme.bodySmall,
                      ),
                    ],
                  ),
                ),
              ],
            ),
            if (order!.deliverySlot != null) ...[
              const SizedBox(height: 12),
              Row(
                children: [
                  const Icon(Icons.schedule, color: Colors.grey),
                  const SizedBox(width: 12),
                  Text(
                    'Delivery Slot: ${order!.deliverySlot}',
                    style: AppTheme.bodyMedium,
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildOrderItemsCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Order Items (${order!.items.length})',
              style: AppTheme.headingSmall,
            ),
            const SizedBox(height: 12),
            ...order!.items
                .map(
                  (item) => Padding(
                    padding: const EdgeInsets.only(bottom: 8),
                    child: Row(
                      children: [
                        Expanded(
                          child: Text(
                            '${item.product.name} x ${item.quantity}',
                            style: AppTheme.bodyMedium,
                          ),
                        ),
                        Text(
                          Helpers.formatCurrency(item.totalPrice),
                          style: AppTheme.bodyMedium.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  ),
                )
                .toList(),
          ],
        ),
      ),
    );
  }

  Widget _buildOrderSummaryCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Order Summary', style: AppTheme.headingSmall),
            const SizedBox(height: 12),
            _buildSummaryRow('Subtotal', order!.subtotal),
            _buildSummaryRow('Tax', order!.tax),
            _buildSummaryRow('Delivery Charge', order!.deliveryCharge),
            if (order!.discount > 0)
              _buildSummaryRow('Discount', -order!.discount, isDiscount: true),
            const Divider(),
            _buildSummaryRow('Total', order!.total, isTotal: true),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Payment ID', style: AppTheme.bodySmall),
                Text(
                  order!.paymentId,
                  style: AppTheme.bodySmall.copyWith(
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
          ],
        ),
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

  Widget _buildCancelOrderButton() {
    return SizedBox(
      width: double.infinity,
      child: OutlinedButton(
        onPressed: () => _showCancelOrderDialog(),
        style: OutlinedButton.styleFrom(
          foregroundColor: Colors.red,
          side: const BorderSide(color: Colors.red),
          padding: const EdgeInsets.symmetric(vertical: 12),
        ),
        child: const Text('Cancel Order'),
      ),
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
      child: Row(
        children: [
          Expanded(
            child: OutlinedButton(
              onPressed: () => context.push('/order-history'),
              child: const Text('View All Orders'),
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: ElevatedButton(
              onPressed: () => context.pushReplacement('/'),
              child: const Text('Continue Shopping'),
            ),
          ),
        ],
      ),
    );
  }

  String _getStatusTitle(OrderStatus status) {
    switch (status) {
      case OrderStatus.placed:
        return 'Order Placed';
      case OrderStatus.confirmed:
        return 'Order Confirmed';
      case OrderStatus.packed:
        return 'Order Packed';
      case OrderStatus.outForDelivery:
        return 'Out for Delivery';
      case OrderStatus.delivered:
        return 'Delivered';
      case OrderStatus.cancelled:
        return 'Cancelled';
    }
  }

  String _getStatusMessage() {
    return AppConstants.orderStatusMessages[order!.status
            .toString()
            .split('.')
            .last] ??
        'Order status updated';
  }

  Future<void> _showCancelOrderDialog() async {
    final confirmed = await Helpers.showConfirmationDialog(
      context,
      title: 'Cancel Order',
      message:
          'Are you sure you want to cancel this order? This action cannot be undone.',
      confirmText: 'Cancel Order',
      cancelText: 'Keep Order',
    );

    if (confirmed) {
      final orderProvider = context.read<OrderProvider>();
      await orderProvider.cancelOrder(order!.id);

      setState(() {
        order = orderProvider.getOrderById(widget.orderId);
      });

      Helpers.showSnackBar(context, 'Order cancelled successfully');
    }
  }
}
