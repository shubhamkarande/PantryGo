class AppConstants {
  // App Info
  static const String appName = 'PantryGo';
  static const String appTagline = 'Stock Your Kitchen in a Few Taps';

  // API Configuration
  static const String baseUrl = 'https://your-api-url.azurewebsites.net/api';
  static const int apiTimeout = 30; // seconds

  // Payment Configuration
  static const String razorpayKeyId =
      'rzp_test_1DP5mmOlF5G5ag'; // Replace with your key
  static const double maxOrderAmount = 500000.0;
  static const double minOrderAmount = 1.0;
  static const double freeDeliveryThreshold = 500.0;
  static const double deliveryCharge = 40.0;
  static const double taxRate = 0.05; // 5%

  // UI Constants
  static const double defaultPadding = 16.0;
  static const double smallPadding = 8.0;
  static const double largePadding = 24.0;
  static const double borderRadius = 8.0;
  static const double cardElevation = 2.0;

  // Image Placeholders
  static const String placeholderImage =
      'https://via.placeholder.com/300x300?text=No+Image';
  static const String categoryPlaceholder =
      'https://via.placeholder.com/150x150?text=Category';

  // Delivery Slots
  static const List<String> deliverySlots = [
    '9:00 AM - 12:00 PM',
    '12:00 PM - 3:00 PM',
    '3:00 PM - 6:00 PM',
    '6:00 PM - 9:00 PM',
  ];

  // Order Status Messages
  static const Map<String, String> orderStatusMessages = {
    'placed': 'Your order has been placed successfully! 🎉',
    'confirmed': 'Your order has been confirmed and is being prepared.',
    'packed': 'Your order has been packed and ready for delivery.',
    'outForDelivery': 'Your order is out for delivery! 🚚',
    'delivered': 'Your order has been delivered successfully! ✅',
    'cancelled': 'Your order has been cancelled.',
  };

  // Promo Codes (for demo)
  static const Map<String, Map<String, dynamic>> promoCodes = {
    'SAVE10': {
      'description': '10% off on all orders',
      'type': 'percentage',
      'value': 0.1,
      'minAmount': 200.0,
    },
    'FLAT50': {
      'description': 'Flat ₹50 off',
      'type': 'fixed',
      'value': 50.0,
      'minAmount': 300.0,
    },
    'FIRST20': {
      'description': '20% off on first order',
      'type': 'percentage',
      'value': 0.2,
      'minAmount': 100.0,
    },
  };

  // Categories
  static const List<Map<String, String>> categories = [
    {'id': '1', 'name': 'Fruits', 'icon': '🍎'},
    {'id': '2', 'name': 'Vegetables', 'icon': '🥕'},
    {'id': '3', 'name': 'Dairy', 'icon': '🥛'},
    {'id': '4', 'name': 'Snacks', 'icon': '🍿'},
    {'id': '5', 'name': 'Beverages', 'icon': '🥤'},
    {'id': '6', 'name': 'Bakery', 'icon': '🍞'},
    {'id': '7', 'name': 'Meat', 'icon': '🍖'},
    {'id': '8', 'name': 'Frozen', 'icon': '🧊'},
  ];

  // Error Messages
  static const String networkError =
      'Please check your internet connection and try again.';
  static const String serverError =
      'Something went wrong. Please try again later.';
  static const String authError = 'Please login to continue.';
  static const String paymentError = 'Payment failed. Please try again.';
  static const String locationError =
      'Unable to get your location. Please enable location services.';

  // Success Messages
  static const String orderPlacedSuccess = 'Order placed successfully!';
  static const String paymentSuccess = 'Payment completed successfully!';
  static const String addressAddedSuccess = 'Address added successfully!';
  static const String profileUpdatedSuccess = 'Profile updated successfully!';

  // Validation Messages
  static const String emailValidation = 'Please enter a valid email address';
  static const String phoneValidation = 'Please enter a valid phone number';
  static const String passwordValidation =
      'Password must be at least 6 characters';
  static const String nameValidation = 'Please enter your name';
  static const String addressValidation = 'Please enter a valid address';
  static const String pincodeValidation = 'Please enter a valid pincode';
}
