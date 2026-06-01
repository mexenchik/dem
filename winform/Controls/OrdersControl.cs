using LightStepWinForms.App;
using LightStepWinForms.Data;
using LightStepWinForms.Forms;
using LightStepWinForms.Models;

namespace LightStepWinForms.Controls;

internal sealed class OrdersControl : UserControl
{
    private readonly DemoDataStore _store;
    private readonly UserRole _role;
    private readonly bool _canEditOrders;

    private readonly DataGridView _grid = new();
    private readonly Button _detailsButton = new();
    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _refreshButton = new();
    private readonly Label _hintLabel = new();

    public OrdersControl(DemoDataStore store, UserRole role)
    {
        _store = store;
        _role = role;
        _canEditOrders = role == UserRole.Admin;
        BuildLayout();
        RefreshGrid();
    }

    private void BuildLayout()
    {
        BackColor = AppTheme.MainBackground;
        Font = AppTheme.RegularFont;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var tools = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 6, 0, 6)
        };

        _detailsButton.Text = "Состав";
        _detailsButton.Width = 105;
        _detailsButton.Height = 34;
        _detailsButton.Click += DetailsButton_Click;
        AppTheme.ApplyPrimaryButton(_detailsButton);

        _refreshButton.Text = "Обновить";
        _refreshButton.Width = 105;
        _refreshButton.Height = 34;
        _refreshButton.Click += (_, _) => RefreshGrid();
        AppTheme.ApplySecondaryButton(_refreshButton);

        _addButton.Text = "Добавить";
        _addButton.Width = 105;
        _addButton.Height = 34;
        _addButton.Click += AddButton_Click;
        AppTheme.ApplyPrimaryButton(_addButton);

        _editButton.Text = "Изменить";
        _editButton.Width = 105;
        _editButton.Height = 34;
        _editButton.Click += EditButton_Click;
        AppTheme.ApplySecondaryButton(_editButton);

        _deleteButton.Text = "Удалить";
        _deleteButton.Width = 105;
        _deleteButton.Height = 34;
        _deleteButton.Click += DeleteButton_Click;
        AppTheme.ApplyDangerButton(_deleteButton);

        tools.Controls.Add(_detailsButton);
        tools.Controls.Add(_refreshButton);
        if (_canEditOrders)
        {
            tools.Controls.Add(_addButton);
            tools.Controls.Add(_editButton);
            tools.Controls.Add(_deleteButton);
        }

        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.MultiSelect = false;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.White;
        _grid.CellDoubleClick += (_, _) => ShowDetails();

        AddColumn(nameof(OrderRow.Id), "Номер", 70);
        AddColumn(nameof(OrderRow.OrderDate), "Дата", 135, "g");
        AddColumn(nameof(OrderRow.ClientName), "Клиент", 180);
        AddColumn(nameof(OrderRow.Status), "Статус", 100);
        AddColumn(nameof(OrderRow.ItemsCount), "Кол-во", 75);
        AddColumn(nameof(OrderRow.TotalAmount), "Сумма", 100, "N2");

        _hintLabel.Dock = DockStyle.Fill;
        _hintLabel.ForeColor = AppTheme.MutedText;
        _hintLabel.TextAlign = ContentAlignment.MiddleLeft;

        root.Controls.Add(tools, 0, 0);
        root.Controls.Add(_grid, 0, 1);
        root.Controls.Add(_hintLabel, 0, 2);
        Controls.Add(root);
    }

    private void AddColumn(string property, string header, int width, string? format = null)
    {
        var column = new DataGridViewTextBoxColumn
        {
            Name = property,
            DataPropertyName = property,
            HeaderText = header,
            FillWeight = width
        };
        if (format != null)
        {
            column.DefaultCellStyle.Format = format;
        }
        _grid.Columns.Add(column);
    }

    private void RefreshGrid()
    {
        var rows = _store.GetOrderRows();
        _grid.DataSource = rows;
        _hintLabel.Text = _role == UserRole.Admin
            ? $"Заказов: {rows.Count}. Администратор может создавать, изменять и удалять заказы."
            : $"Заказов: {rows.Count}. Менеджеру доступен просмотр заказов и состава.";
    }

    private void DetailsButton_Click(object? sender, EventArgs e)
    {
        ShowDetails();
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var form = new OrderEditorForm(_store, null);
        if (form.ShowDialog(this) == DialogResult.OK && form.Order != null)
        {
            _store.AddOrder(form.Order, form.Items);
            RefreshGrid();
        }
    }

    private void EditButton_Click(object? sender, EventArgs e)
    {
        var orderId = GetSelectedOrderId();
        if (orderId == null)
        {
            return;
        }

        var order = _store.GetOrder(orderId.Value);
        if (order == null)
        {
            MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var form = new OrderEditorForm(_store, order);
        if (form.ShowDialog(this) == DialogResult.OK && form.Order != null)
        {
            _store.UpdateOrder(form.Order, form.Items);
            RefreshGrid();
        }
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        var orderId = GetSelectedOrderId();
        if (orderId == null)
        {
            return;
        }

        var result = MessageBox.Show(
            $"Удалить заказ №{orderId.Value}?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _store.DeleteOrder(orderId.Value);
            RefreshGrid();
        }
    }

    private void ShowDetails()
    {
        var orderId = GetSelectedOrderId();
        if (orderId == null)
        {
            return;
        }

        using var form = new OrderDetailsForm(_store, orderId.Value);
        form.ShowDialog(this);
    }

    private int? GetSelectedOrderId()
    {
        if (_grid.CurrentRow?.DataBoundItem is OrderRow row)
        {
            return row.Id;
        }

        MessageBox.Show("Выберите заказ в таблице.", "Нет выбранной записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return null;
    }
}
