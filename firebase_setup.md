# Firebase Setup Guide for PantryGo

## 1. Create Firebase Project

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Click "Create a project"
3. Enter project name: `pantrygo`
4. Enable Google Analytics (optional)
5. Create project

## 2. Add Android App

1. Click "Add app" → Android
2. Enter package name: `com.example.pantrygo` (or your package name)
3. Enter app nickname: `PantryGo Android`
4. Download `google-services.json`
5. Place it in `android/app/` directory

## 3. Add iOS App

1. Click "Add app" → iOS
2. Enter bundle ID: `com.example.pantrygo` (or your bundle ID)
3. Enter app nickname: `PantryGo iOS`
4. Download `GoogleService-Info.plist`
5. Place it in `ios/Runner/` directory

## 4. Enable Cloud Messaging

1. Go to Project Settings → Cloud Messaging
2. Note down the Server Key (for backend)
3. Configure APNs for iOS (upload APNs certificate)

## 5. Android Configuration

Add to `android/build.gradle`:
```gradle
dependencies {
    classpath 'com.google.gms:google-services:4.3.15'
}
```

Add to `android/app/build.gradle`:
```gradle
apply plugin: 'com.google.gms.google-services'

dependencies {
    implementation 'com.google.firebase:firebase-messaging:23.2.1'
}
```

## 6. iOS Configuration

Add to `ios/Runner/Info.plist`:
```xml
<key>FirebaseAppDelegateProxyEnabled</key>
<false/>
```

## 7. Flutter Configuration

The app already includes Firebase dependencies in `pubspec.yaml`:
```yaml
dependencies:
  firebase_core: ^2.31.0
  firebase_messaging: ^14.9.2
  flutter_local_notifications: ^17.2.1+2
```

## 8. Test Configuration

Run the app and check console for:
- Firebase initialization success
- FCM token generation
- Test notification delivery

## 9. Backend Integration

Use the Server Key in your ASP.NET Core backend to send notifications:

```csharp
// Example C# code for sending FCM notifications
public class FirebaseService
{
    private readonly string _serverKey = "YOUR_SERVER_KEY_HERE";
    
    public async Task SendNotificationAsync(string token, string title, string body)
    {
        // Implementation for sending FCM notifications
    }
}
```

## 10. Testing Notifications

1. Get FCM token from app logs
2. Use Firebase Console → Cloud Messaging → Send test message
3. Enter the token and send notification
4. Verify notification is received on device

## Troubleshooting

### Android Issues
- Ensure `google-services.json` is in correct location
- Check package name matches Firebase configuration
- Verify Google Services plugin is applied

### iOS Issues
- Ensure `GoogleService-Info.plist` is added to Xcode project
- Check bundle ID matches Firebase configuration
- Configure APNs certificates properly

### Common Issues
- Network connectivity
- Incorrect configuration files
- Missing permissions
- Outdated dependencies

## Security Notes

- Keep Server Key secure (use environment variables)
- Restrict API keys to specific apps/domains
- Use Firebase Security Rules if using Firestore
- Regularly rotate keys and certificates