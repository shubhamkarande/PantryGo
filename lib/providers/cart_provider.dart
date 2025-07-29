import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'dart:convert';
import '../models/cart_item.dart';
import '../models/product.dart';

class CartProvider with ChangeNotifier {
  List<CartItem> _items = [];
  String _promoCode = '';
  double _promoDiscount = 0.0;

  List<CartItem> get items => _items;
  String get promoCode => _promoCode;
  double get promoDiscount => _promoDiscount;

  int get itemCount => _items.fold(0, (sum, item) => sum + item.quantity);

  double get subtotal => _items.fold(0.0, (sum, item) => sum + item.totalPrice);

  double get tax => subtotal * 0.05; // 5% tax

  double get deliveryCharge => subtotal > 500 ? 0.0 : 40.0;

  double get total => subtotal + tax + deliveryCharge - _promoDiscount;

  bool get isEmpty => _items.isEmpty;

  CartProvider() {
    _loadCart();
  }

  Future<void> _loadCart() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cartData = prefs.getString('cart');
      if (cartData != null) {
        final List<dynamic> jsonList = json.decode(cartData);
        _items = jsonList.map((json) => CartItem.fromJson(json)).toList();
        notifyListeners();
      }
    } catch (e) {
      print('Error loading cart: $e');
    }
  }

  Future<void> _saveCart() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final cartData = json.encode(
        _items.map((item) => item.toJson()).toList(),
      );
      await prefs.setString('cart', cartData);
    } catch (e) {
      print('Error saving cart: $e');
    }
  }

  void addItem(Product product, {int quantity = 1}) {
    final existingIndex = _items.indexWhere(
      (item) => item.product.id == product.id,
    );

    if (existingIndex >= 0) {
      _items[existingIndex].quantity += quantity;
    } else {
      _items.add(
        CartItem(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          product: product,
          quantity: quantity,
        ),
      );
    }

    _saveCart();
    notifyListeners();
  }

  void removeItem(String productId) {
    _items.removeWhere((item) => item.product.id == productId);
    _saveCart();
    notifyListeners();
  }

  void updateQuantity(String productId, int quantity) {
    if (quantity <= 0) {
      removeItem(productId);
      return;
    }

    final index = _items.indexWhere((item) => item.product.id == productId);
    if (index >= 0) {
      _items[index].quantity = quantity;
      _saveCart();
      notifyListeners();
    }
  }

  void incrementQuantity(String productId) {
    final index = _items.indexWhere((item) => item.product.id == productId);
    if (index >= 0) {
      _items[index].quantity++;
      _saveCart();
      notifyListeners();
    }
  }

  void decrementQuantity(String productId) {
    final index = _items.indexWhere((item) => item.product.id == productId);
    if (index >= 0) {
      if (_items[index].quantity > 1) {
        _items[index].quantity--;
      } else {
        _items.removeAt(index);
      }
      _saveCart();
      notifyListeners();
    }
  }

  int getQuantity(String productId) {
    final item = _items.firstWhere(
      (item) => item.product.id == productId,
      orElse: () => CartItem(
        id: '',
        product: Product(
          id: '',
          name: '',
          description: '',
          price: 0,
          imageUrl: '',
          category: '',
          unit: '',
        ),
        quantity: 0,
      ),
    );
    return item.quantity;
  }

  bool isInCart(String productId) {
    return _items.any((item) => item.product.id == productId);
  }

  Future<bool> applyPromoCode(String code) async {
    // Mock promo code validation
    // In real app, this would call API
    switch (code.toUpperCase()) {
      case 'SAVE10':
        _promoCode = code;
        _promoDiscount = subtotal * 0.1; // 10% discount
        notifyListeners();
        return true;
      case 'FLAT50':
        _promoCode = code;
        _promoDiscount = 50.0; // Flat 50 discount
        notifyListeners();
        return true;
      case 'FIRST20':
        _promoCode = code;
        _promoDiscount = subtotal * 0.2; // 20% discount for first order
        notifyListeners();
        return true;
      default:
        return false;
    }
  }

  void removePromoCode() {
    _promoCode = '';
    _promoDiscount = 0.0;
    notifyListeners();
  }

  void clearCart() {
    _items.clear();
    _promoCode = '';
    _promoDiscount = 0.0;
    _saveCart();
    notifyListeners();
  }
}
