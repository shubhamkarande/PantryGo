# PantryGo - Stock Your Kitchen in a Few Taps

A modern, full-featured mobile grocery shopping app built with Flutter, featuring cart management, real-time order tracking, secure payments, and push notifications.

## Features

### 🛒 Product Catalog
- Browse products by categories (Fruits, Vegetables, Dairy, Snacks, etc.)
- Product search with real-time results
- Product details with images, pricing, and reviews
- Discount and offer management

### 🛍️ Shopping Cart
- Add/remove items with quantity management
- Real-time price calculations
- Promo code support
- Persistent cart storage

### 💳 Secure Checkout
- Multiple delivery address management
- Delivery slot selection
- Razorpay payment integration
- Order confirmation

### 📦 Order Management
- Real-time order tracking
- Order status updates (Placed → Packed → Out for Delivery → Delivered)
- Order history with reorder functionality
- Order cancellation support

### 🔔 Push Notifications
- Firebase Cloud Messaging integration
- Order status notifications
- Promotional offers
- Delivery updates

### 👤 User Management
- User authentication
- Profile management
- Address book
- Order history

## Tech Stack

- **Frontend**: Flutter (Dart)
- **Backend**: ASP.NET Core Web API (Azure App Service)
- **Database**: PostgreSQL (Azure Database for PostgreSQL)
- **Push Notifications**: Firebase Cloud Messaging (FCM)
- **Payments**: Razorpay API
- **Maps**: Google Maps Flutter
- **State Management**: Provider
- **Navigation**: GoRouter

## Getting Started

### Prerequisites

- Flutter SDK (3.8.1 or higher)
- Dart SDK
- Android Studio / VS Code
- Firebase account
- Razorpay account

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd pantrygo
   ```

2. **Install dependencies**
   ```bash
   flutter pub get
   ```

3. **Firebase Setup**
   - Create a new Firebase project
   - Add Android/iOS apps to your Firebase project
   - Download `google-services.json` (Android) and `GoogleService-Info.plist` (iOS)
   - Place them in the respective platform folders
   - Enable Firebase Cloud Messaging

4. **Razorpay Setup**
   - Create a Razorpay account
   - Get your API keys from the dashboard
   - Update the key in `lib/services/payment_service.dart`

5. **Google Maps Setup** (Optional)
   - Get Google Maps API key
   - Add it to Android and iOS configurations

6. **Run the app**
   ```bash
   flutter run
   ```

## Project Structure

```
lib/
├── main.dart                 # App entry point
├── models/                   # Data models
│   ├── product.dart
│   ├── cart_item.dart
│   ├── order.dart
│   ├── address.dart
│   └── category.dart
├── providers/                # State management
│   ├── auth_provider.dart
│   ├── product_provider.dart
│   ├── cart_provider.dart
│   └── order_provider.dart
├── screens/                  # UI screens
│   ├── home_screen.dart
│   ├── product_detail_screen.dart
│   ├── cart_screen.dart
│   ├── checkout_screen.dart
│   ├── order_tracking_screen.dart
│   ├── order_history_screen.dart
│   └── profile_screen.dart
├── widgets/                  # Reusable widgets
│   ├── product_card.dart
│   ├── category_card.dart
│   └── search_bar.dart
├── services/                 # External services
│   ├── api_service.dart
│   ├── notification_service.dart
│   └── payment_service.dart
└── utils/                    # Utilities
    ├── app_theme.dart
    ├── constants.dart
    └── helpers.dart
```

## Configuration

### API Configuration
Update the base URL in `lib/services/api_service.dart`:
```dart
static const String baseUrl = 'https://your-api-url.azurewebsites.net/api';
```

### Payment Configuration
Update Razorpay key in `lib/services/payment_service.dart`:
```dart
'key': 'rzp_live_your_key_here', // Replace with your live key
```

### Firebase Configuration
Ensure Firebase is properly configured with:
- Authentication (optional)
- Cloud Messaging for notifications
- Proper platform-specific setup files

## Backend API Endpoints

The app expects the following API endpoints:

### Authentication
- `POST /api/auth/login`
- `POST /api/auth/register`

### Products
- `GET /api/categories`
- `GET /api/products`
- `GET /api/products/featured`
- `GET /api/products/search?q={query}`

### Orders
- `POST /api/orders`
- `GET /api/users/{userId}/orders`
- `PATCH /api/orders/{orderId}/status`

### Addresses
- `GET /api/users/{userId}/addresses`
- `POST /api/users/{userId}/addresses`
- `PUT /api/users/{userId}/addresses/{addressId}`
- `DELETE /api/users/{userId}/addresses/{addressId}`

### Payments
- `POST /api/payments/verify`

## Features in Detail

### State Management
The app uses Provider for state management with the following providers:
- `AuthProvider`: User authentication and profile management
- `ProductProvider`: Product catalog and search functionality
- `CartProvider`: Shopping cart management
- `OrderProvider`: Order placement and tracking

### Payment Integration
Razorpay integration includes:
- Secure payment processing
- Multiple payment methods (Cards, UPI, Net Banking, Wallets)
- Payment verification with backend
- Error handling and retry mechanisms

### Push Notifications
Firebase Cloud Messaging provides:
- Order status updates
- Promotional notifications
- Delivery alerts
- Background message handling

### Offline Support
- Cart persistence using SharedPreferences
- Cached product images
- Graceful error handling for network issues

## Testing

Run tests using:
```bash
flutter test
```

## Building for Production

### Android
```bash
flutter build apk --release
# or
flutter build appbundle --release
```

### iOS
```bash
flutter build ios --release
```

## Deployment

### Backend Deployment (Azure)
1. Deploy ASP.NET Core Web API to Azure App Service
2. Configure PostgreSQL database on Azure
3. Set up environment variables for API keys
4. Configure CORS for mobile app access

### Mobile App Deployment
1. **Android**: Upload to Google Play Store
2. **iOS**: Upload to Apple App Store

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions:
- Create an issue in the repository
- Contact: support@pantrygo.com

## Roadmap

- [ ] Social login integration
- [ ] Wishlist functionality
- [ ] Product reviews and ratings
- [ ] Live chat support
- [ ] Loyalty program
- [ ] Multi-language support
- [ ] Dark mode theme

## Getting Started

This project is a starting point for a Flutter application.

A few resources to get you started if this is your first Flutter project:

- [Lab: Write your first Flutter app](https://docs.flutter.dev/get-started/codelab)
- [Cookbook: Useful Flutter samples](https://docs.flutter.dev/cookbook)

For help getting started with Flutter development, view the
[online documentation](https://docs.flutter.dev/), which offers tutorials,
samples, guidance on mobile development, and a full API reference.
