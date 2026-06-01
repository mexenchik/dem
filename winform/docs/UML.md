# UML-шпаргалка для проекта

## ERD

```text
Roles 1 --- * Users
Users 1 --- * Orders
Orders 1 --- * OrderItems
Products 1 --- * OrderItems
```

Поля:

```text
Roles: Id PK, RoleName
Users: Id PK, Login, PasswordHash, FullName, RoleId FK
Products: Id PK, Name, Article, Price, Size, Brand, Quantity, DiscountPercent
Orders: Id PK, UserId FK, OrderDate, Status, TotalAmount
OrderItems: Id PK, OrderId FK, ProductId FK, Count, PriceAtMoment
```

## Use Case

Акторы:

- Гость;
- Клиент;
- Менеджер;
- Администратор.

Варианты использования:

- войти в систему;
- войти как гость;
- просмотреть каталог товаров;
- выполнить поиск/фильтрацию/сортировку;
- просмотреть заказы;
- просмотреть состав заказа;
- управлять товарами;
- управлять заказами.

## Activity авторизации

```text
Старт
Показать форму входа
Пользователь выбрал гостя?
  Да -> открыть каталог с ролью Guest -> конец
  Нет -> получить логин и пароль
Проверить данные в БД
Данные верные?
  Нет -> показать ошибку -> вернуться к форме входа
  Да -> определить роль -> открыть главное окно -> конец
```

## Class diagram

```text
LoginForm
+ LoginButton_Click()
+ GuestButton_Click()

MainForm
- currentUser
- role
+ ShowCatalog()
+ ShowOrders()

ProductCatalogControl
+ RefreshGrid()
+ Add/Edit/Delete product

OrdersControl
+ RefreshGrid()
+ Add/Edit/Delete order

DemoDataStore
+ Authenticate()
+ GetProductRows()
+ AddProduct()
+ UpdateProduct()
+ DeleteProduct()
+ GetOrderRows()
+ AddOrder()
+ UpdateOrder()
+ DeleteOrder()

Product
- Id
- Name
- Article
- Price
- Quantity

Order
- Id
- UserId
- OrderDate
- Status
- TotalAmount
```
