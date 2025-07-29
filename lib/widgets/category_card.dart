import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';

import '../models/category.dart' as model;
import '../utils/app_theme.dart';

class CategoryCard extends StatelessWidget {
  final model.Category category;
  final VoidCallback onTap;

  const CategoryCard({super.key, required this.category, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        width: 80,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(12),
          boxShadow: [
            BoxShadow(
              color: Colors.grey.withOpacity(0.1),
              spreadRadius: 1,
              blurRadius: 4,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            ClipRRect(
              borderRadius: BorderRadius.circular(8),
              child: CachedNetworkImage(
                imageUrl: category.imageUrl,
                width: 40,
                height: 40,
                fit: BoxFit.cover,
                placeholder: (context, url) => Container(
                  width: 40,
                  height: 40,
                  color: Colors.grey[200],
                  child: const Icon(
                    Icons.category,
                    color: Colors.grey,
                    size: 20,
                  ),
                ),
                errorWidget: (context, url, error) => Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: AppTheme.primaryColor.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Icon(
                    Icons.category,
                    color: AppTheme.primaryColor,
                    size: 20,
                  ),
                ),
              ),
            ),
            const SizedBox(height: 8),
            Text(
              category.name,
              style: AppTheme.bodySmall.copyWith(fontWeight: FontWeight.w600),
              textAlign: TextAlign.center,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
            if (category.productCount > 0)
              Text(
                '${category.productCount} items',
                style: AppTheme.bodySmall.copyWith(
                  fontSize: 10,
                  color: Colors.grey,
                ),
              ),
          ],
        ),
      ),
    );
  }
}
