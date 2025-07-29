class Category {
  final String id;
  final String name;
  final String imageUrl;
  final String description;
  final int productCount;
  final bool isActive;

  Category({
    required this.id,
    required this.name,
    required this.imageUrl,
    this.description = '',
    this.productCount = 0,
    this.isActive = true,
  });

  factory Category.fromJson(Map<String, dynamic> json) {
    return Category(
      id: json['id'],
      name: json['name'],
      imageUrl: json['imageUrl'],
      description: json['description'] ?? '',
      productCount: json['productCount'] ?? 0,
      isActive: json['isActive'] ?? true,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'imageUrl': imageUrl,
      'description': description,
      'productCount': productCount,
      'isActive': isActive,
    };
  }
}
