import 'product.dart';

class CartItem {
  final String id;
  final Product product;
  int quantity;

  CartItem({required this.id, required this.product, this.quantity = 1});

  double get totalPrice => product.effectivePrice * quantity;

  factory CartItem.fromJson(Map<String, dynamic> json) {
    return CartItem(
      id: json['id'],
      product: Product.fromJson(json['product']),
      quantity: json['quantity'],
    );
  }

  Map<String, dynamic> toJson() {
    return {'id': id, 'product': product.toJson(), 'quantity': quantity};
  }

  CartItem copyWith({String? id, Product? product, int? quantity}) {
    return CartItem(
      id: id ?? this.id,
      product: product ?? this.product,
      quantity: quantity ?? this.quantity,
    );
  }
}
