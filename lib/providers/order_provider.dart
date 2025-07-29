import 'package:flutter/foundation.dart';
import '../models/order.dart';
import '../models/cart_item.dart';
import '../models/address.dart';
import '../services/api_service.dart';

class OrderProvider with ChangeNotifier {
  List<Order> _orders = [];
  Order? _currentOrder;
  bool _isLoading = false;

  List<Order> get orders => _orders;
  Order? get currentOrder => _currentOrder;
  bool get isLoading => _isLoading;

  Future<String?> createOrder({
    required List<CartItem> items,
    required double subtotal,
    required double tax,
    required double deliveryCharge,
    required double discount,
    required double total,
    required Address deliveryAddress,
    required String paymentId,
    String? deliverySlot,
    String? promoCode,
  }) async {
    _isLoading = true;
    notifyListeners();

    try {
      final orderData = {
        'items': items.map((item) => item.toJson()).toList(),
        'subtotal': subtotal,
        'tax': tax,
        'deliveryCharge': deliveryCharge,
        'discount': discount,
        'total': total,
        'deliveryAddress': deliveryAddress.toJson(),
        'paymentId': paymentId,
        'deliverySlot': deliverySlot,
        'promoCode': promoCode,
      };

      final response = await ApiService.createOrder(orderData);

      if (response['success']) {
        final order = Order.fromJson(response['order']);
        _currentOrder = order;
        _orders.insert(0, order);
        notifyListeners();
        return order.id;
      }
      return null;
    } catch (e) {
      print('Error creating order: $e');
      // Create mock order for demo
      final order = Order(
        id: DateTime.now().millisecondsSinceEpoch.toString(),
        items: items,
        subtotal: subtotal,
        tax: tax,
        deliveryCharge: deliveryCharge,
        discount: discount,
        total: total,
        deliveryAddress: deliveryAddress,
        paymentId: paymentId,
        createdAt: DateTime.now(),
        deliverySlot: deliverySlot,
        promoCode: promoCode,
      );

      _currentOrder = order;
      _orders.insert(0, order);
      notifyListeners();
      return order.id;
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> loadOrders(String userId) async {
    _isLoading = true;
    notifyListeners();

    try {
      final response = await ApiService.getUserOrders(userId);
      _orders = (response['orders'] as List)
          .map((json) => Order.fromJson(json))
          .toList();
      notifyListeners();
    } catch (e) {
      print('Error loading orders: $e');
      // Load mock orders for demo
      _orders = _getMockOrders();
      notifyListeners();
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> updateOrderStatus(String orderId, OrderStatus status) async {
    try {
      final response = await ApiService.updateOrderStatus(orderId, status);

      if (response['success']) {
        final index = _orders.indexWhere((order) => order.id == orderId);
        if (index >= 0) {
          // Create updated order with new status
          final updatedOrder = Order(
            id: _orders[index].id,
            items: _orders[index].items,
            subtotal: _orders[index].subtotal,
            tax: _orders[index].tax,
            deliveryCharge: _orders[index].deliveryCharge,
            discount: _orders[index].discount,
            total: _orders[index].total,
            deliveryAddress: _orders[index].deliveryAddress,
            status: status,
            createdAt: _orders[index].createdAt,
            deliveredAt: status == OrderStatus.delivered
                ? DateTime.now()
                : _orders[index].deliveredAt,
            deliverySlot: _orders[index].deliverySlot,
            promoCode: _orders[index].promoCode,
            paymentId: _orders[index].paymentId,
            trackingUrl: _orders[index].trackingUrl,
          );

          _orders[index] = updatedOrder;

          if (_currentOrder?.id == orderId) {
            _currentOrder = updatedOrder;
          }

          notifyListeners();
        }
      }
    } catch (e) {
      print('Error updating order status: $e');
    }
  }

  Future<void> cancelOrder(String orderId) async {
    await updateOrderStatus(orderId, OrderStatus.cancelled);
  }

  Order? getOrderById(String orderId) {
    try {
      return _orders.firstWhere((order) => order.id == orderId);
    } catch (e) {
      return null;
    }
  }

  void setCurrentOrder(Order order) {
    _currentOrder = order;
    notifyListeners();
  }

  List<Order> _getMockOrders() {
    // Return empty list for now - orders will be created when user places them
    return [];
  }
}
