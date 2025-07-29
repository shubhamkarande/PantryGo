import 'package:flutter/foundation.dart';
import '../models/product.dart';
import '../models/category.dart' as model;
import '../services/api_service.dart';

class ProductProvider with ChangeNotifier {
  List<Product> _products = [];
  List<model.Category> _categories = [];
  List<Product> _featuredProducts = [];
  List<Product> _searchResults = [];
  String _selectedCategory = '';
  bool _isLoading = false;
  String _searchQuery = '';

  List<Product> get products => _products;
  List<model.Category> get categories => _categories;
  List<Product> get featuredProducts => _featuredProducts;
  List<Product> get searchResults => _searchResults;
  String get selectedCategory => _selectedCategory;
  bool get isLoading => _isLoading;
  String get searchQuery => _searchQuery;

  List<Product> get filteredProducts {
    if (_selectedCategory.isEmpty) return _products;
    return _products
        .where((product) => product.category == _selectedCategory)
        .toList();
  }

  ProductProvider() {
    loadInitialData();
  }

  Future<void> loadInitialData() async {
    _isLoading = true;
    notifyListeners();

    try {
      await Future.wait([
        loadCategories(),
        loadProducts(),
        loadFeaturedProducts(),
      ]);
    } catch (e) {
      print('Error loading initial data: $e');
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> loadCategories() async {
    try {
      final response = await ApiService.getCategories();
      _categories = (response['categories'] as List)
          .map((json) => model.Category.fromJson(json))
          .toList();
      notifyListeners();
    } catch (e) {
      print('Error loading categories: $e');
      // Fallback to mock data
      _categories = _getMockCategories();
      notifyListeners();
    }
  }

  Future<void> loadProducts() async {
    try {
      final response = await ApiService.getProducts();
      _products = (response['products'] as List)
          .map((json) => Product.fromJson(json))
          .toList();
      notifyListeners();
    } catch (e) {
      print('Error loading products: $e');
      // Fallback to mock data
      _products = _getMockProducts();
      notifyListeners();
    }
  }

  Future<void> loadFeaturedProducts() async {
    try {
      final response = await ApiService.getFeaturedProducts();
      _featuredProducts = (response['products'] as List)
          .map((json) => Product.fromJson(json))
          .toList();
      notifyListeners();
    } catch (e) {
      print('Error loading featured products: $e');
      // Fallback to mock data
      _featuredProducts = _products.take(6).toList();
      notifyListeners();
    }
  }

  Future<void> searchProducts(String query) async {
    _searchQuery = query;

    if (query.isEmpty) {
      _searchResults.clear();
      notifyListeners();
      return;
    }

    try {
      final response = await ApiService.searchProducts(query);
      _searchResults = (response['products'] as List)
          .map((json) => Product.fromJson(json))
          .toList();
      notifyListeners();
    } catch (e) {
      print('Error searching products: $e');
      // Fallback to local search
      _searchResults = _products
          .where(
            (product) =>
                product.name.toLowerCase().contains(query.toLowerCase()) ||
                product.description.toLowerCase().contains(query.toLowerCase()),
          )
          .toList();
      notifyListeners();
    }
  }

  void selectCategory(String categoryId) {
    _selectedCategory = categoryId;
    notifyListeners();
  }

  void clearCategory() {
    _selectedCategory = '';
    notifyListeners();
  }

  Product? getProductById(String id) {
    try {
      return _products.firstWhere((product) => product.id == id);
    } catch (e) {
      return null;
    }
  }

  List<model.Category> _getMockCategories() {
    return [
      model.Category(
        id: '1',
        name: 'Fruits',
        imageUrl: 'https://via.placeholder.com/150',
        productCount: 25,
      ),
      model.Category(
        id: '2',
        name: 'Vegetables',
        imageUrl: 'https://via.placeholder.com/150',
        productCount: 30,
      ),
      model.Category(
        id: '3',
        name: 'Dairy',
        imageUrl: 'https://via.placeholder.com/150',
        productCount: 15,
      ),
      model.Category(
        id: '4',
        name: 'Snacks',
        imageUrl: 'https://via.placeholder.com/150',
        productCount: 40,
      ),
      model.Category(
        id: '5',
        name: 'Beverages',
        imageUrl: 'https://via.placeholder.com/150',
        productCount: 20,
      ),
      model.Category(
        id: '6',
        name: 'Bakery',
        imageUrl: 'https://via.placeholder.com/150',
        productCount: 12,
      ),
    ];
  }

  List<Product> _getMockProducts() {
    return [
      Product(
        id: '1',
        name: 'Fresh Bananas',
        description: 'Fresh yellow bananas, perfect for breakfast',
        price: 40.0,
        discountPrice: 35.0,
        imageUrl: 'https://via.placeholder.com/300',
        category: 'Fruits',
        unit: 'per dozen',
        stockQuantity: 50,
        rating: 4.5,
        reviewCount: 120,
      ),
      Product(
        id: '2',
        name: 'Organic Apples',
        description: 'Fresh organic red apples',
        price: 120.0,
        imageUrl: 'https://via.placeholder.com/300',
        category: 'Fruits',
        unit: 'per kg',
        stockQuantity: 30,
        rating: 4.8,
        reviewCount: 85,
      ),
      Product(
        id: '3',
        name: 'Fresh Milk',
        description: 'Pure cow milk, 500ml pack',
        price: 25.0,
        imageUrl: 'https://via.placeholder.com/300',
        category: 'Dairy',
        unit: 'per pack',
        stockQuantity: 100,
        rating: 4.6,
        reviewCount: 200,
      ),
      Product(
        id: '4',
        name: 'Potato Chips',
        description: 'Crispy salted potato chips',
        price: 20.0,
        discountPrice: 18.0,
        imageUrl: 'https://via.placeholder.com/300',
        category: 'Snacks',
        unit: 'per pack',
        stockQuantity: 75,
        rating: 4.2,
        reviewCount: 150,
      ),
    ];
  }
}
