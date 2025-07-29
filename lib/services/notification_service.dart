// Mock Notification Service for Frontend Testing
// This will be replaced with actual Firebase FCM integration later

class NotificationService {
  static Future<void> initialize() async {
    print('NotificationService initialized (mock)');
  }

  static Future<void> showLocalNotification({
    required String title,
    required String body,
    String? payload,
  }) async {
    print('Mock Notification: $title - $body');
  }

  static Future<void> showOrderStatusNotification({
    required String orderId,
    required String status,
    required String message,
  }) async {
    print('Mock Order Notification: Order $orderId - $message');
  }

  static Future<void> showOfferNotification({
    required String title,
    required String message,
    String? offerId,
  }) async {
    print('Mock Offer Notification: $title - $message');
  }

  static Future<void> showDeliveryNotification({
    required String orderId,
    required String message,
  }) async {
    print('Mock Delivery Notification: Order $orderId - $message');
  }

  static Future<String?> getFCMToken() async {
    return 'mock_fcm_token_${DateTime.now().millisecondsSinceEpoch}';
  }

  static Future<void> subscribeToTopic(String topic) async {
    print('Mock: Subscribed to topic $topic');
  }

  static Future<void> unsubscribeFromTopic(String topic) async {
    print('Mock: Unsubscribed from topic $topic');
  }
}