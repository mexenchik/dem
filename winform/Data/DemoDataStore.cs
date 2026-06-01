using LightStepWinForms.Models;

namespace LightStepWinForms.Data;

internal sealed class DemoDataStore
{
    private readonly List<AppUser> _users = new();
    private readonly List<Product> _products = new();
    private readonly List<Order> _orders = new();
    private readonly List<OrderItem> _orderItems = new();

    private DemoDataStore()
    {
    }

    public IReadOnlyList<AppUser> Users => _users;
    public IReadOnlyList<Product> Products => _products;

    public static DemoDataStore CreateSeeded()
    {
        var store = new DemoDataStore();

        store._users.AddRange(new[]
        {
            new AppUser { Id = 1, Login = "client", PasswordHash = PasswordHasher.Hash("client123"), FullName = "Иван Клиентов", Role = UserRole.Client },
            new AppUser { Id = 2, Login = "manager", PasswordHash = PasswordHasher.Hash("manager123"), FullName = "Мария Менеджерова", Role = UserRole.Manager },
            new AppUser { Id = 3, Login = "admin", PasswordHash = PasswordHasher.Hash("admin123"), FullName = "Алексей Админов", Role = UserRole.Admin }
        });

        store._products.AddRange(new[]
        {
            new Product
            {
                Id = 1,
                Name = "Кроссовки городские",
                Article = "SH-1001",
                Category = "Кроссовки",
                Description = "Повседневная обувь для города",
                Brand = "EasyStep",
                Supplier = "Поставка Север",
                Price = 5200m,
                Size = "42",
                Unit = "пара",
                Quantity = 12,
                DiscountPercent = 5
            },
            new Product
            {
                Id = 2,
                Name = "Ботинки зимние",
                Article = "SH-1002",
                Category = "Ботинки",
                Description = "Утепленная зимняя модель",
                Brand = "NordWay",
                Supplier = "Поставка Север",
                Price = 8900m,
                Size = "43",
                Unit = "пара",
                Quantity = 4,
                DiscountPercent = 20
            },
            new Product
            {
                Id = 3,
                Name = "Туфли классические",
                Article = "SH-1003",
                Category = "Туфли",
                Description = "Офисная обувь",
                Brand = "FormalLine",
                Supplier = "ГородСклад",
                Price = 7400m,
                Size = "41",
                Unit = "пара",
                Quantity = 0,
                DiscountPercent = 0
            },
            new Product
            {
                Id = 4,
                Name = "Кеды текстильные",
                Article = "SH-1004",
                Category = "Кеды",
                Description = "Легкая летняя обувь",
                Brand = "StreetGo",
                Supplier = "ГородСклад",
                Price = 3100m,
                Size = "39",
                Unit = "пара",
                Quantity = 25,
                DiscountPercent = 10
            },
            new Product
            {
                Id = 5,
                Name = "Сапоги демисезонные",
                Article = "SH-1005",
                Category = "Сапоги",
                Description = "Женская демисезонная модель",
                Brand = "EasyStep",
                Supplier = "Мир обуви",
                Price = 6700m,
                Size = "38",
                Unit = "пара",
                Quantity = 7,
                DiscountPercent = 18
            }
        });

        store._orders.AddRange(new[]
        {
            new Order { Id = 1, UserId = 1, OrderDate = new DateTime(2026, 5, 20, 10, 0, 0), Status = "Новый", TotalAmount = 5200m },
            new Order { Id = 2, UserId = 1, OrderDate = new DateTime(2026, 5, 21, 12, 30, 0), Status = "Выдан", TotalAmount = 6200m }
        });

        store._orderItems.AddRange(new[]
        {
            new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Count = 1, PriceAtMoment = 5200m },
            new OrderItem { Id = 2, OrderId = 2, ProductId = 4, Count = 2, PriceAtMoment = 3100m }
        });

        return store;
    }

    public AppUser? Authenticate(string login, string password)
    {
        var hash = PasswordHasher.Hash(password);
        return _users.FirstOrDefault(user =>
            string.Equals(user.Login, login, StringComparison.OrdinalIgnoreCase)
            && user.PasswordHash == hash);
    }

    public List<AppUser> GetClientUsers()
    {
        return _users
            .Where(user => user.Role == UserRole.Client)
            .OrderBy(user => user.FullName)
            .Select(CloneUser)
            .ToList();
    }

    public List<string> GetBrands()
    {
        return _products
            .Select(product => product.Brand)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(brand => brand)
            .ToList();
    }

    public List<string> GetSizes()
    {
        return _products
            .Select(product => product.Size)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(size => size)
            .ToList();
    }

    public List<ProductRow> GetProductRows(string search, string brand, string size, string sortMode, bool advancedCatalog)
    {
        IEnumerable<Product> query = _products;

        if (advancedCatalog)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(product =>
                    product.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || product.Article.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(product => string.Equals(product.Brand, brand, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(size))
            {
                query = query.Where(product => string.Equals(product.Size, size, StringComparison.OrdinalIgnoreCase));
            }

            query = sortMode switch
            {
                "Цена ↑" => query.OrderBy(product => product.Price),
                "Цена ↓" => query.OrderByDescending(product => product.Price),
                "Скидка ↓" => query.OrderByDescending(product => product.DiscountPercent),
                _ => query.OrderBy(product => product.Name)
            };
        }
        else
        {
            query = query.OrderBy(product => product.Name);
        }

        return query.Select(product => new ProductRow
        {
            Id = product.Id,
            Name = product.Name,
            Article = product.Article,
            Category = product.Category,
            Brand = product.Brand,
            Supplier = product.Supplier,
            Price = product.Price,
            DiscountPercent = product.DiscountPercent,
            FinalPrice = product.FinalPrice,
            Size = product.Size,
            Quantity = product.Quantity
        }).ToList();
    }

    public Product? GetProduct(int id)
    {
        return _products.FirstOrDefault(product => product.Id == id)?.Clone();
    }

    public bool ArticleExists(string article, int? exceptProductId = null)
    {
        return _products.Any(product =>
            string.Equals(product.Article, article, StringComparison.OrdinalIgnoreCase)
            && product.Id != exceptProductId);
    }

    public void AddProduct(Product product)
    {
        product.Id = NextProductId();
        _products.Add(product.Clone());
    }

    public void UpdateProduct(Product product)
    {
        var existing = _products.FirstOrDefault(item => item.Id == product.Id)
            ?? throw new InvalidOperationException("Товар не найден.");

        existing.Name = product.Name;
        existing.Article = product.Article;
        existing.Category = product.Category;
        existing.Description = product.Description;
        existing.Brand = product.Brand;
        existing.Supplier = product.Supplier;
        existing.Price = product.Price;
        existing.Size = product.Size;
        existing.Unit = product.Unit;
        existing.Quantity = product.Quantity;
        existing.DiscountPercent = product.DiscountPercent;
        existing.ImagePath = product.ImagePath;
    }

    public void DeleteProduct(int productId)
    {
        if (_orderItems.Any(item => item.ProductId == productId))
        {
            throw new InvalidOperationException("Нельзя удалить товар, который уже есть в заказах. Для экзамена можно заменить это логическим удалением.");
        }

        var existing = _products.FirstOrDefault(product => product.Id == productId);
        if (existing != null)
        {
            _products.Remove(existing);
        }
    }

    public List<OrderRow> GetOrderRows()
    {
        return _orders
            .OrderByDescending(order => order.OrderDate)
            .Select(order =>
            {
                var user = _users.FirstOrDefault(item => item.Id == order.UserId);
                return new OrderRow
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    ClientName = user?.FullName ?? "Неизвестный клиент",
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    ItemsCount = _orderItems.Where(item => item.OrderId == order.Id).Sum(item => item.Count)
                };
            })
            .ToList();
    }

    public Order? GetOrder(int orderId)
    {
        return _orders.FirstOrDefault(order => order.Id == orderId)?.Clone();
    }

    public List<OrderItem> GetOrderItems(int orderId)
    {
        return _orderItems
            .Where(item => item.OrderId == orderId)
            .Select(item => item.Clone())
            .ToList();
    }

    public List<OrderItemRow> GetOrderItemRows(int orderId)
    {
        return _orderItems
            .Where(item => item.OrderId == orderId)
            .Select(ToOrderItemRow)
            .ToList();
    }

    public void AddOrder(Order order, List<OrderItem> items)
    {
        order.Id = NextOrderId();
        order.TotalAmount = items.Sum(item => item.Count * item.PriceAtMoment);
        _orders.Add(order.Clone());

        foreach (var item in items)
        {
            item.Id = NextOrderItemId();
            item.OrderId = order.Id;
            _orderItems.Add(item.Clone());
        }
    }

    public void UpdateOrder(Order order, List<OrderItem> items)
    {
        var existing = _orders.FirstOrDefault(item => item.Id == order.Id)
            ?? throw new InvalidOperationException("Заказ не найден.");

        existing.UserId = order.UserId;
        existing.OrderDate = order.OrderDate;
        existing.Status = order.Status;
        existing.TotalAmount = items.Sum(item => item.Count * item.PriceAtMoment);

        _orderItems.RemoveAll(item => item.OrderId == order.Id);
        foreach (var item in items)
        {
            item.Id = NextOrderItemId();
            item.OrderId = order.Id;
            _orderItems.Add(item.Clone());
        }
    }

    public void DeleteOrder(int orderId)
    {
        _orderItems.RemoveAll(item => item.OrderId == orderId);
        _orders.RemoveAll(order => order.Id == orderId);
    }

    public OrderItemRow ToOrderItemRow(OrderItem item)
    {
        var product = _products.FirstOrDefault(product => product.Id == item.ProductId);
        return new OrderItemRow
        {
            ProductId = item.ProductId,
            ProductName = product?.Name ?? "Товар удален",
            Article = product?.Article ?? "-",
            Count = item.Count,
            PriceAtMoment = item.PriceAtMoment,
            Sum = item.Sum
        };
    }

    private int NextProductId()
    {
        return _products.Count == 0 ? 1 : _products.Max(product => product.Id) + 1;
    }

    private int NextOrderId()
    {
        return _orders.Count == 0 ? 1 : _orders.Max(order => order.Id) + 1;
    }

    private int NextOrderItemId()
    {
        return _orderItems.Count == 0 ? 1 : _orderItems.Max(item => item.Id) + 1;
    }

    private static AppUser CloneUser(AppUser user)
    {
        return new AppUser
        {
            Id = user.Id,
            Login = user.Login,
            PasswordHash = user.PasswordHash,
            FullName = user.FullName,
            Role = user.Role
        };
    }
}
