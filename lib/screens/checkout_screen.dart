import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';

import '../providers/cart_provider.dart';
import '../providers/auth_provider.dart';
import '../providers/order_provider.dart';
import '../models/address.dart';
// import '../services/payment_service.dart'; // Temporarily disabled
import '../utils/app_theme.dart';
import '../utils/helpers.dart';
import '../utils/constants.dart';

class CheckoutScreen extends StatefulWidget {
  const CheckoutScreen({super.key});

  @override
  State<CheckoutScreen> createState() => _CheckoutScreenState();
}

class _CheckoutScreenState extends State<CheckoutScreen> {
  String? selectedDeliverySlot;
  bool isProcessing = false;

  @override
  void initState() {
    super.initState();
    // PaymentService.initialize(); // Temporarily disabled
  }

  @override
  void dispose() {
    // PaymentService.dispose(); // Temporarily disabled
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Checkout')),
      body: Consumer3<CartProvider, AuthProvider, OrderProvider>(
        builder: (context, cart, auth, order, child) {
          if (cart.isEmpty) {
            return const Center(child: Text('Your cart is empty'));
          }

          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildDeliveryAddressSection(auth),
                const SizedBox(height: 20),
                _buildDeliverySlotSection(),
                const SizedBox(height: 20),
                _buildOrderItemsSection(cart),
                const SizedBox(height: 20),
                _buildOrderSummarySection(cart),
              ],
            ),
          );
        },
      ),
      bottomNavigationBar: _buildBottomBar(),
    );
  }

  Widget _buildDeliveryAddressSection(AuthProvider auth) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Delivery Address', style: AppTheme.headingSmall),
                TextButton(
                  onPressed: () {
                    // Navigate to address selection
                  },
                  child: const Text('Change'),
                ),
              ],
            ),
            const SizedBox(height: 12),
            if (auth.selectedAddress != null) ...[
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  border: Border.all(color: AppTheme.primaryColor),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      auth.selectedAddress!.name,
                      style: AppTheme.bodyMedium.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      auth.selectedAddress!.fullAddress,
                      style: AppTheme.bodySmall,
                    ),
                    const SizedBox(height: 4),
                    Text(
                      auth.selectedAddress!.phone,
                      style: AppTheme.bodySmall,
                    ),
                  ],
                ),
              ),
            ] else ...[
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: Colors.grey[100],
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.location_off, color: Colors.grey),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Text(
                        'No delivery address selected',
                        style: AppTheme.bodyMedium.copyWith(
                          color: Colors.grey[600],
                        ),
                      ),
                    ),
                    ElevatedButton(
                      onPressed: () {
                        // Navigate to add address
                      },
                      child: const Text('Add Address'),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildDeliverySlotSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Delivery Slot (Optional)', style: AppTheme.headingSmall),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: AppConstants.deliverySlots.map((slot) {
                final isSelected = selectedDeliverySlot == slot;
                return ChoiceChip(
                  label: Text(slot),
                  selected: isSelected,
                  onSelected: (selected) {
                    setState(() {
                      selectedDeliverySlot = selected ? slot : null;
                    });
                  },
                  selectedColor: AppTheme.primaryColor.withOpacity(0.2),
                  labelStyle: TextStyle(
                    color: isSelected ? AppTheme.primaryColor : null,
                    fontWeight: isSelected ? FontWeight.w600 : null,
                  ),
                );
              }).toList(),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildOrderItemsSection(CartProvider cart) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Order Items (${cart.itemCount})',
              style: AppTheme.headingSmall,
            ),
            const SizedBox(height: 12),
            ...cart.items
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

  Widget _buildOrderSummarySection(CartProvider cart) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Order Summary', style: AppTheme.headingSmall),
            const SizedBox(height: 12),
            _buildSummaryRow('Subtotal', cart.subtotal),
            _buildSummaryRow('Tax', cart.tax),
            _buildSummaryRow('Delivery Charge', cart.deliveryCharge),
            if (cart.promoDiscount > 0)
              _buildSummaryRow(
                'Discount',
                -cart.promoDiscount,
                isDiscount: true,
              ),
            const Divider(),
            _buildSummaryRow('Total', cart.total, isTotal: true),
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
      child: Consumer3<CartProvider, AuthProvider, OrderProvider>(
        builder: (context, cart, auth, order, child) {
          return SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: isProcessing || auth.selectedAddress == null
                  ? null
                  : () => _processPayment(cart, auth, order),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
              child: isProcessing
                  ? const Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        SizedBox(
                          width: 20,
                          height: 20,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            valueColor: AlwaysStoppedAnimation<Color>(
                              Colors.white,
                            ),
                          ),
                        ),
                        SizedBox(width: 12),
                        Text('Processing...'),
                      ],
                    )
                  : Text(
                      'Pay ${Helpers.formatCurrency(cart.total)}',
                      style: AppTheme.buttonText,
                    ),
            ),
          );
        },
      ),
    );
  }

  Future<void> _processPayment(
    CartProvider cart,
    AuthProvider auth,
    OrderProvider order,
  ) async {
    if (auth.selectedAddress == null) {
      Helpers.showSnackBar(
        context,
        'Please select a delivery address',
        isError: true,
      );
      return;
    }

    setState(() {
      isProcessing = true;
    });

    try {
      // Mock payment success for testing
      await Future.delayed(const Duration(seconds: 2));
      final mockPaymentId = 'mock_pay_${DateTime.now().millisecondsSinceEpoch}';
      _onPaymentSuccess(mockPaymentId, cart, auth, order);
    } catch (e) {
      _onPaymentError('Failed to initiate payment: $e');
    }
  }

  Future<void> _onPaymentSuccess(
    String paymentId,
    CartProvider cart,
    AuthProvider auth,
    OrderProvider order,
  ) async {
    try {
      final orderId = await order.createOrder(
        items: cart.items,
        subtotal: cart.subtotal,
        tax: cart.tax,
        deliveryCharge: cart.deliveryCharge,
        discount: cart.promoDiscount,
        total: cart.total,
        deliveryAddress: auth.selectedAddress!,
        paymentId: paymentId,
        deliverySlot: selectedDeliverySlot,
        promoCode: cart.promoCode.isNotEmpty ? cart.promoCode : null,
      );

      if (orderId != null) {
        cart.clearCart();

        setState(() {
          isProcessing = false;
        });

        Helpers.showSnackBar(context, 'Order placed successfully!');
        context.pushReplacement('/order-tracking/$orderId');
      } else {
        throw Exception('Failed to create order');
      }
    } catch (e) {
      _onPaymentError('Order creation failed: $e');
    }
  }

  void _onPaymentError(String error) {
    setState(() {
      isProcessing = false;
    });

    Helpers.showSnackBar(context, error, isError: true);
  }
}
