using LightStepWinForms.App;
using LightStepWinForms.Data;
using LightStepWinForms.Models;

namespace LightStepWinForms.Forms;

internal partial class OrderEditorForm : Form
{
    private readonly DemoDataStore _store;
    private readonly int? _orderId;
    private readonly List<OrderItem> _items = new();

    public OrderEditorForm(DemoDataStore store, Order? order)
    {
        _store = store;
        _orderId = order?.Id;
        Order = order?.Clone();

        InitializeComponent();
        AppTheme.ApplyForm(this);
        ApplyTheme();
        FillCombos();

        if (order != null)
        {
            FillFields(order);
        }

        RefreshItemsGrid();
    }

    public Order? Order { get; private set; }
    public List<OrderItem> Items => _items.Select(item => item.Clone()).ToList();

    private void ApplyTheme()
    {
        Text = _orderId == null ? "Создание заказа" : "Редактирование заказа";
        headerLabel.BackColor = AppTheme.SecondaryBackground;
        headerLabel.Font = AppTheme.HeaderFont;
        AppTheme.ApplyPrimaryButton(saveButton);
        AppTheme.ApplySecondaryButton(cancelButton);
        AppTheme.ApplyPrimaryButton(addItemButton);
        AppTheme.ApplyDangerButton(removeItemButton);
    }

    private void FillCombos()
    {
        clientComboBox.DataSource = _store.GetClientUsers();
        clientComboBox.DisplayMember = nameof(AppUser.FullName);
        clientComboBox.ValueMember = nameof(AppUser.Id);

        statusComboBox.Items.AddRange(new object[] { "Новый", "В работе", "Готов к выдаче", "Выдан", "Отменен" });
        statusComboBox.SelectedIndex = 0;

        productComboBox.DataSource = _store.Products.Select(product => product.Clone()).ToList();
        productComboBox.DisplayMember = nameof(Product.Name);
        productComboBox.ValueMember = nameof(Product.Id);
    }

    private void FillFields(Order order)
    {
        clientComboBox.SelectedValue = order.UserId;
        orderDatePicker.Value = order.OrderDate;
        statusComboBox.Text = order.Status;

        _items.Clear();
        _items.AddRange(_store.GetOrderItems(order.Id));
    }

    private void AddItemButton_Click(object? sender, EventArgs e)
    {
        if (productComboBox.SelectedItem is not Product product)
        {
            MessageBox.Show("Выберите товар.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var count = (int)countNumeric.Value;
        var existing = _items.FirstOrDefault(item => item.ProductId == product.Id);
        if (existing != null)
        {
            existing.Count += count;
        }
        else
        {
            _items.Add(new OrderItem
            {
                ProductId = product.Id,
                Count = count,
                PriceAtMoment = product.FinalPrice
            });
        }

        RefreshItemsGrid();
    }

    private void RemoveItemButton_Click(object? sender, EventArgs e)
    {
        if (itemsGrid.CurrentRow?.DataBoundItem is not OrderItemRow row)
        {
            MessageBox.Show("Выберите строку состава.", "Нет выбранной записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var existing = _items.FirstOrDefault(item => item.ProductId == row.ProductId);
        if (existing != null)
        {
            _items.Remove(existing);
            RefreshItemsGrid();
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (clientComboBox.SelectedValue is not int userId)
        {
            MessageBox.Show("Выберите клиента.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_items.Count == 0)
        {
            MessageBox.Show("Добавьте хотя бы один товар в заказ.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Order = new Order
        {
            Id = _orderId ?? 0,
            UserId = userId,
            OrderDate = orderDatePicker.Value,
            Status = statusComboBox.Text,
            TotalAmount = _items.Sum(item => item.Sum)
        };

        DialogResult = DialogResult.OK;
        Close();
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void RefreshItemsGrid()
    {
        var rows = _items.Select(_store.ToOrderItemRow).ToList();
        itemsGrid.DataSource = rows;
        totalLabel.Text = $"Итого: {rows.Sum(row => row.Sum):N2} руб.";
    }
}
