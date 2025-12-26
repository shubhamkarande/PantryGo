# PantryGo – Grocery E-Commerce App

🛒 **Stock Your Kitchen in a Few Taps**

A full-stack grocery e-commerce mobile application built with .NET MAUI and ASP.NET Core.

---

## 🎯 Features

### Customer Features

- 📱 **Product Browsing** - Browse products by category with search & filters
- 🛒 **Shopping Cart** - Add items, adjust quantities, view totals
- 💳 **Secure Checkout** - Razorpay payment integration (test mode)
- 📦 **Order Tracking** - Real-time order status updates
- 📍 **Address Management** - Save multiple delivery addresses
- 👤 **User Profile** - Manage account and view order history

### Admin Features

- 📝 Product management (CRUD)
- 📊 Order management
- 🚚 Delivery partner assignment

### Technical Features

- 🔐 JWT authentication with refresh tokens
- 🏗️ Clean MVVM architecture
- 📱 Cross-platform (Android, iOS, Windows)
- 🔔 Push notification ready (FCM placeholder)

---

## 🧱 Tech Stack

| Layer | Technology |
|-------|------------|
| **Mobile App** | .NET MAUI, C#, MVVM, CommunityToolkit.Mvvm |
| **Backend API** | ASP.NET Core 9.0, Entity Framework Core |
| **Database** | PostgreSQL |
| **Payments** | Razorpay (Test Mode) |
| **Notifications** | Firebase Cloud Messaging (Ready to integrate) |

---

## 📁 Project Structure

```
PantryGo/
├── backend/
│   └── PantryGo.Api/
│       ├── Controllers/     # API endpoints
│       ├── Services/        # Business logic
│       ├── Models/          # Entities & DTOs
│       ├── Data/            # DbContext & migrations
│       └── Program.cs       # App configuration
│
└── mobile/
    └── PantryGo/
        ├── Views/           # XAML pages
        ├── ViewModels/      # MVVM view models
        ├── Services/        # API & local services
        ├── Models/          # Data models
        ├── Helpers/         # Utilities & converters
        └── AppShell.xaml    # Navigation
```

---

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 14+
- Visual Studio 2022 or VS Code with .NET MAUI workload
- Android SDK (for Android builds)

### Backend Setup

1. **Configure Database**

   Edit `backend/PantryGo.Api/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=pantrygo_dev;Username=postgres;Password=YOUR_PASSWORD"
     }
   }
   ```

2. **Create Database & Apply Migrations**

   ```bash
   cd backend/PantryGo.Api
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Run the API**

   ```bash
   dotnet run
   ```

   API will be available at `http://localhost:5000`<br>
   Swagger UI: `http://localhost:5000/swagger`

### Mobile App Setup

1. **Update API URL** (if needed)

   Edit `mobile/PantryGo/Helpers/Constants.cs`:

   ```csharp
   public static string ApiBaseUrl => "http://YOUR_API_URL:5000/api";
   ```

2. **Build & Run**

   ```bash
   cd mobile/PantryGo
   
   # Windows
   dotnet build -t:Run -f net8.0-windows10.0.19041.0
   
   # Android
   dotnet build -t:Run -f net8.0-android
   ```

---

## 🔌 API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login |
| POST | `/api/auth/refresh` | Refresh JWT token |
| GET | `/api/auth/me` | Get current user |

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | List products (with filters) |
| GET | `/api/products/{id}` | Get product details |
| GET | `/api/products/categories` | Get categories |
| POST | `/api/products` | Create product (Admin) |
| PUT | `/api/products/{id}` | Update product (Admin) |

### Orders

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders` | Create order |
| GET | `/api/orders` | Get user's orders |
| GET | `/api/orders/{id}` | Get order details |
| PUT | `/api/orders/{id}/status` | Update status (Admin/Delivery) |

### Payments

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/payments/create` | Create payment order |
| POST | `/api/payments/verify` | Verify payment |

---

## 🔐 Environment Variables

Create `.env` or configure in `appsettings.json`:

```
DB_CONNECTION_STRING=Host=localhost;Database=pantrygo;...
JWT_SECRET=YourSecureJwtSecretKey32CharactersLong
RAZORPAY_KEY_ID=rzp_test_xxx
RAZORPAY_KEY_SECRET=xxx
FCM_SERVER_KEY=xxx (optional)
```

---

## 📱 Screenshots

| Home | Product Detail | Cart |
|------|----------------|------|
| Product grid with categories | Quantity selection | Cart summary |

| Checkout | Orders | Profile |
|----------|--------|---------|
| Address selection | Order history | User menu |

---

## 🧪 Testing

### Backend

```bash
cd backend/PantryGo.Api
dotnet test
```

### Test Credentials (Razorpay Test Mode)

- Card: `4111 1111 1111 1111`
- Expiry: Any future date
- CVV: Any 3 digits

---

## 📦 Deployment

### Backend

- **Azure App Service** - Free tier available
- **Railway** - PostgreSQL hosting

### Mobile

- **Android**: Generate APK with `dotnet publish`
- **iOS**: Requires Mac with Xcode

---

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

---

## 👤 Author

**PantryGo Team**

---

*Built with ❤️ using .NET MAUI and ASP.NET Core*
