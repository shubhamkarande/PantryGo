import 'cart_item.dart';
import 'address.dart';

enum OrderStatus {
  placed,
  confirmed,
  packed,
  outForDelivery,
  delivered,
  cancelled,
}

class Order {
  final String id;
  final List<CartItem> items;
  final double subtotal;
  final double tax;
  final double deliveryCharge;
  final double discount;
  final double total;
  final Address deliveryAddress;
  final OrderStatus status;
  final DateTime createdAt;
  final DateTime? deliveredAt;
  final String? deliverySlot;
  final String? promoCode;
  final String paymentId;
  final String? trackingUrl;

  Order({
    required this.id,
    required this.items,
    required this.subtotal,
    required this.tax,
    required this.deliveryCharge,
    this.discount = 0,
    required this.total,
    required this.deliveryAddress,
    this.status = OrderStatus.placed,
    required this.createdAt,
    this.deliveredAt,
    this.deliverySlot,
    this.promoCode,
    required this.paymentId,
    this.trackingUrl,
  });

  factory Order.fromJson(Map<String, dynamic> json) {
    return Order(
      id: json['id'],
      items: (json['items'] as List)
          .map((item) => CartItem.fromJson(item))
          .toList(),
      subtotal: json['subtotal'].toDouble(),
      tax: json['tax'].toDouble(),
      deliveryCharge: json['deliveryCharge'].toDouble(),
      discount: json['discount']?.toDouble() ?? 0,
      total: json['total'].toDouble(),
      deliveryAddress: Address.fromJson(json['deliveryAddress']),
      status: OrderStatus.values.firstWhere(
        (e) => e.toString().split('.').last == json['status'],
        orElse: () => OrderStatus.placed,
      ),
      createdAt: DateTime.parse(json['createdAt']),
      deliveredAt: json['deliveredAt'] != null
          ? DateTime.parse(json['deliveredAt'])
          : null,
      deliverySlot: json['deliverySlot'],
      promoCode: json['promoCode'],
      paymentId: json['paymentId'],
      trackingUrl: json['trackingUrl'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'items': items.map((item) => item.toJson()).toList(),
      'subtotal': subtotal,
      'tax': tax,
      'deliveryCharge': deliveryCharge,
      'discount': discount,
      'total': total,
      'deliveryAddress': deliveryAddress.toJson(),
      'status': status.toString().split('.').last,
      'createdAt': createdAt.toIso8601String(),
      'deliveredAt': deliveredAt?.toIso8601String(),
      'deliverySlot': deliverySlot,
      'promoCode': promoCode,
      'paymentId': paymentId,
      'trackingUrl': trackingUrl,
    };
  }

  String get statusText {
    switch (status) {
      case OrderStatus.placed:
        return 'Order Placed';
      case OrderStatus.confirmed:
        return 'Confirmed';
      case OrderStatus.packed:
        return 'Packed';
      case OrderStatus.outForDelivery:
        return 'Out for Delivery';
      case OrderStatus.delivered:
        return 'Delivered';
      case OrderStatus.cancelled:
        return 'Cancelled';
    }
  }

  bool get canCancel =>
      status == OrderStatus.placed || status == OrderStatus.confirmed;
  bool get isCompleted =>
      status == OrderStatus.delivered || status == OrderStatus.cancelled;
}
