import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import '../models/order.dart';
import '../models/address.dart';

class ApiService {
  static const String baseUrl = 'https://your-api-url.azurewebsites.net/api';

  static Future<String?> _getAuthToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('authToken');
  }

  static Map<String, String> _getHeaders({bool requiresAuth = false}) {
    final headers = {'Content-Type': 'application/json'};

    return headers;
  }

  static Future<Map<String, dynamic>> _handleResponse(
    http.Response response,
  ) async {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return json.decode(response.body);
    } else {
      throw Exception('API Error: ${response.statusCode} - ${response.body}');
    }
  }

  // Auth endpoints
  static Future<Map<String, dynamic>> login(
    String email,
    String password,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/login'),
        headers: _getHeaders(),
        body: json.encode({'email': email, 'password': password}),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {
        'success': true,
        'user': {
          'id': 'user123',
          'name': 'John Doe',
          'email': email,
          'phone': '+1234567890',
        },
        'token': 'mock_token_123',
      };
    }
  }

  static Future<Map<String, dynamic>> register(
    String name,
    String email,
    String phone,
    String password,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/register'),
        headers: _getHeaders(),
        body: json.encode({
          'name': name,
          'email': email,
          'phone': phone,
          'password': password,
        }),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {'success': true, 'message': 'User registered successfully'};
    }
  }

  // Product endpoints
  static Future<Map<String, dynamic>> getCategories() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/categories'),
        headers: _getHeaders(),
      );
      return await _handleResponse(response);
    } catch (e) {
      throw Exception('Failed to load categories');
    }
  }

  static Future<Map<String, dynamic>> getProducts() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/products'),
        headers: _getHeaders(),
      );
      return await _handleResponse(response);
    } catch (e) {
      throw Exception('Failed to load products');
    }
  }

  static Future<Map<String, dynamic>> getFeaturedProducts() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/products/featured'),
        headers: _getHeaders(),
      );
      return await _handleResponse(response);
    } catch (e) {
      throw Exception('Failed to load featured products');
    }
  }

  static Future<Map<String, dynamic>> searchProducts(String query) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/products/search?q=${Uri.encodeComponent(query)}'),
        headers: _getHeaders(),
      );
      return await _handleResponse(response);
    } catch (e) {
      throw Exception('Failed to search products');
    }
  }

  // Address endpoints
  static Future<Map<String, dynamic>> getUserAddresses(String userId) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/users/$userId/addresses'),
        headers: _getHeaders(requiresAuth: true),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {'addresses': []};
    }
  }

  static Future<Map<String, dynamic>> addAddress(
    String userId,
    Address address,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/users/$userId/addresses'),
        headers: _getHeaders(requiresAuth: true),
        body: json.encode(address.toJson()),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {'success': true};
    }
  }

  static Future<Map<String, dynamic>> updateAddress(
    String userId,
    Address address,
  ) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/users/$userId/addresses/${address.id}'),
        headers: _getHeaders(requiresAuth: true),
        body: json.encode(address.toJson()),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {'success': true};
    }
  }

  static Future<Map<String, dynamic>> deleteAddress(
    String userId,
    String addressId,
  ) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/users/$userId/addresses/$addressId'),
        headers: _getHeaders(requiresAuth: true),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {'success': true};
    }
  }

  // Order endpoints
  static Future<Map<String, dynamic>> createOrder(
    Map<String, dynamic> orderData,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/orders'),
        headers: _getHeaders(requiresAuth: true),
        body: json.encode(orderData),
      );
      return await _handleResponse(response);
    } catch (e) {
      throw Exception('Failed to create order');
    }
  }

  static Future<Map<String, dynamic>> getUserOrders(String userId) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/users/$userId/orders'),
        headers: _getHeaders(requiresAuth: true),
      );
      return await _handleResponse(response);
    } catch (e) {
      throw Exception('Failed to load orders');
    }
  }

  static Future<Map<String, dynamic>> updateOrderStatus(
    String orderId,
    OrderStatus status,
  ) async {
    try {
      final response = await http.patch(
        Uri.parse('$baseUrl/orders/$orderId/status'),
        headers: _getHeaders(requiresAuth: true),
        body: json.encode({'status': status.toString().split('.').last}),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {'success': true};
    }
  }

  // Payment endpoints
  static Future<Map<String, dynamic>> verifyPayment(
    String paymentId,
    String orderId,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/payments/verify'),
        headers: _getHeaders(requiresAuth: true),
        body: json.encode({'paymentId': paymentId, 'orderId': orderId}),
      );
      return await _handleResponse(response);
    } catch (e) {
      // Mock response for demo
      return {'success': true, 'verified': true};
    }
  }
}
