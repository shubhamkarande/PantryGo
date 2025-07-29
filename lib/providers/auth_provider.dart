import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/address.dart';
import '../services/api_service.dart';

class AuthProvider with ChangeNotifier {
  bool _isAuthenticated = false;
  String? _userId;
  String? _userName;
  String? _userEmail;
  String? _userPhone;
  List<Address> _addresses = [];
  Address? _selectedAddress;

  bool get isAuthenticated => _isAuthenticated;
  String? get userId => _userId;
  String? get userName => _userName;
  String? get userEmail => _userEmail;
  String? get userPhone => _userPhone;
  List<Address> get addresses => _addresses;
  Address? get selectedAddress => _selectedAddress;

  AuthProvider() {
    _loadAuthState();
  }

  Future<void> _loadAuthState() async {
    final prefs = await SharedPreferences.getInstance();
    _isAuthenticated = prefs.getBool('isAuthenticated') ?? false;
    _userId = prefs.getString('userId');
    _userName = prefs.getString('userName');
    _userEmail = prefs.getString('userEmail');
    _userPhone = prefs.getString('userPhone');

    if (_isAuthenticated) {
      await loadAddresses();
    }

    notifyListeners();
  }

  Future<bool> login(String email, String password) async {
    try {
      final response = await ApiService.login(email, password);

      if (response['success']) {
        _isAuthenticated = true;
        _userId = response['user']['id'];
        _userName = response['user']['name'];
        _userEmail = response['user']['email'];
        _userPhone = response['user']['phone'];

        final prefs = await SharedPreferences.getInstance();
        await prefs.setBool('isAuthenticated', true);
        await prefs.setString('userId', _userId!);
        await prefs.setString('userName', _userName!);
        await prefs.setString('userEmail', _userEmail!);
        await prefs.setString('userPhone', _userPhone!);
        await prefs.setString('authToken', response['token']);

        await loadAddresses();
        notifyListeners();
        return true;
      }
      return false;
    } catch (e) {
      print('Login error: $e');
      return false;
    }
  }

  Future<bool> register(
    String name,
    String email,
    String phone,
    String password,
  ) async {
    try {
      final response = await ApiService.register(name, email, phone, password);

      if (response['success']) {
        return await login(email, password);
      }
      return false;
    } catch (e) {
      print('Registration error: $e');
      return false;
    }
  }

  Future<void> logout() async {
    _isAuthenticated = false;
    _userId = null;
    _userName = null;
    _userEmail = null;
    _userPhone = null;
    _addresses.clear();
    _selectedAddress = null;

    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();

    notifyListeners();
  }

  Future<void> loadAddresses() async {
    if (!_isAuthenticated) return;

    try {
      final response = await ApiService.getUserAddresses(_userId!);
      _addresses = (response['addresses'] as List)
          .map((json) => Address.fromJson(json))
          .toList();

      _selectedAddress = _addresses.isNotEmpty
          ? _addresses.firstWhere(
              (addr) => addr.isDefault,
              orElse: () => _addresses.first,
            )
          : null;

      notifyListeners();
    } catch (e) {
      print('Error loading addresses: $e');
    }
  }

  Future<void> addAddress(Address address) async {
    try {
      final response = await ApiService.addAddress(_userId!, address);
      if (response['success']) {
        await loadAddresses();
      }
    } catch (e) {
      print('Error adding address: $e');
    }
  }

  Future<void> updateAddress(Address address) async {
    try {
      final response = await ApiService.updateAddress(_userId!, address);
      if (response['success']) {
        await loadAddresses();
      }
    } catch (e) {
      print('Error updating address: $e');
    }
  }

  Future<void> deleteAddress(String addressId) async {
    try {
      final response = await ApiService.deleteAddress(_userId!, addressId);
      if (response['success']) {
        await loadAddresses();
      }
    } catch (e) {
      print('Error deleting address: $e');
    }
  }

  void selectAddress(Address address) {
    _selectedAddress = address;
    notifyListeners();
  }
}
