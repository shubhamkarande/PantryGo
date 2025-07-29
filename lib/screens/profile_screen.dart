import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';

import '../providers/auth_provider.dart';
import '../utils/app_theme.dart';
import '../utils/helpers.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Profile')),
      body: Consumer<AuthProvider>(
        builder: (context, authProvider, child) {
          if (!authProvider.isAuthenticated) {
            return _buildNotLoggedIn(context);
          }

          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                _buildProfileHeader(authProvider),
                const SizedBox(height: 24),
                _buildMenuSection(context, authProvider),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildNotLoggedIn(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.person_outline, size: 80, color: Colors.grey[400]),
          const SizedBox(height: 16),
          Text(
            'Please login to access your profile',
            style: AppTheme.headingMedium.copyWith(color: Colors.grey[600]),
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: () => _showLoginDialog(context),
            child: const Text('Login'),
          ),
        ],
      ),
    );
  }

  Widget _buildProfileHeader(AuthProvider authProvider) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(20),
        child: Row(
          children: [
            CircleAvatar(
              radius: 30,
              backgroundColor: AppTheme.primaryColor,
              child: Text(
                authProvider.userName?.substring(0, 1).toUpperCase() ?? 'U',
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    authProvider.userName ?? 'User',
                    style: AppTheme.headingMedium,
                  ),
                  const SizedBox(height: 4),
                  Text(
                    authProvider.userEmail ?? '',
                    style: AppTheme.bodyMedium.copyWith(
                      color: Colors.grey[600],
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    authProvider.userPhone ?? '',
                    style: AppTheme.bodyMedium.copyWith(
                      color: Colors.grey[600],
                    ),
                  ),
                ],
              ),
            ),
            IconButton(
              onPressed: () {
                // Edit profile
              },
              icon: const Icon(Icons.edit),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildMenuSection(BuildContext context, AuthProvider authProvider) {
    return Column(
      children: [
        _buildMenuItem(
          context,
          icon: Icons.history,
          title: 'Order History',
          subtitle: 'View your past orders',
          onTap: () => context.push('/order-history'),
        ),
        _buildMenuItem(
          context,
          icon: Icons.location_on,
          title: 'Delivery Addresses',
          subtitle: 'Manage your addresses',
          onTap: () {
            // Navigate to addresses screen
          },
        ),
        _buildMenuItem(
          context,
          icon: Icons.payment,
          title: 'Payment Methods',
          subtitle: 'Manage payment options',
          onTap: () {
            // Navigate to payment methods screen
          },
        ),
        _buildMenuItem(
          context,
          icon: Icons.notifications,
          title: 'Notifications',
          subtitle: 'Notification preferences',
          onTap: () {
            // Navigate to notification settings
          },
        ),
        _buildMenuItem(
          context,
          icon: Icons.help,
          title: 'Help & Support',
          subtitle: 'Get help and contact us',
          onTap: () {
            // Navigate to help screen
          },
        ),
        _buildMenuItem(
          context,
          icon: Icons.info,
          title: 'About',
          subtitle: 'App version and info',
          onTap: () {
            // Show about dialog
          },
        ),
        const SizedBox(height: 20),
        _buildMenuItem(
          context,
          icon: Icons.logout,
          title: 'Logout',
          subtitle: 'Sign out of your account',
          onTap: () => _showLogoutDialog(context, authProvider),
          isDestructive: true,
        ),
      ],
    );
  }

  Widget _buildMenuItem(
    BuildContext context, {
    required IconData icon,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
    bool isDestructive = false,
  }) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: Icon(
          icon,
          color: isDestructive ? Colors.red : AppTheme.primaryColor,
        ),
        title: Text(
          title,
          style: AppTheme.bodyMedium.copyWith(
            fontWeight: FontWeight.w600,
            color: isDestructive ? Colors.red : null,
          ),
        ),
        subtitle: Text(subtitle, style: AppTheme.bodySmall),
        trailing: const Icon(Icons.chevron_right),
        onTap: onTap,
      ),
    );
  }

  Future<void> _showLoginDialog(BuildContext context) async {
    final emailController = TextEditingController();
    final passwordController = TextEditingController();
    bool isLoading = false;

    await showDialog(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setState) => AlertDialog(
          title: const Text('Login'),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: emailController,
                decoration: const InputDecoration(
                  labelText: 'Email',
                  border: OutlineInputBorder(),
                ),
                keyboardType: TextInputType.emailAddress,
              ),
              const SizedBox(height: 16),
              TextField(
                controller: passwordController,
                decoration: const InputDecoration(
                  labelText: 'Password',
                  border: OutlineInputBorder(),
                ),
                obscureText: true,
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: isLoading ? null : () => Navigator.of(context).pop(),
              child: const Text('Cancel'),
            ),
            ElevatedButton(
              onPressed: isLoading
                  ? null
                  : () async {
                      setState(() => isLoading = true);

                      final success = await context.read<AuthProvider>().login(
                        emailController.text.trim(),
                        passwordController.text,
                      );

                      setState(() => isLoading = false);

                      if (success) {
                        Navigator.of(context).pop();
                        Helpers.showSnackBar(context, 'Login successful!');
                      } else {
                        Helpers.showSnackBar(
                          context,
                          'Login failed',
                          isError: true,
                        );
                      }
                    },
              child: isLoading
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Text('Login'),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _showLogoutDialog(
    BuildContext context,
    AuthProvider authProvider,
  ) async {
    final confirmed = await Helpers.showConfirmationDialog(
      context,
      title: 'Logout',
      message: 'Are you sure you want to logout?',
      confirmText: 'Logout',
      cancelText: 'Cancel',
    );

    if (confirmed) {
      await authProvider.logout();
      Helpers.showSnackBar(context, 'Logged out successfully');
    }
  }
}
