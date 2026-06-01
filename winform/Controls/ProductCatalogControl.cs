using LightStepWinForms.App;
using LightStepWinForms.Data;
using LightStepWinForms.Forms;
using LightStepWinForms.Models;

namespace LightStepWinForms.Controls;

internal sealed class ProductCatalogControl : UserControl
{
    private readonly DemoDataStore _store;
    private readonly UserRole _role;
    private readonly bool _advancedCatalog;
    private readonly bool _canEditProducts;

    private readonly TextBox _searchTextBox = new();
    private readonly ComboBox _brandComboBox = new();
    private readonly ComboBox _sizeComboBox = new();
    private readonly ComboBox _sortComboBox = new();
    private readonly Label _countLabel = new();
    private readonly DataGridView _grid = new();
    private readonly Button _addButton = new();
    private readonly Button _editButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _refreshButton = new();

    public ProductCatalogControl(DemoDataStore store, UserRole role)
    {
        _store = store;
        _role = role;
        _advancedCatalog = role is UserRole.Manager or UserRole.Admin;
        _canEditProducts = role == UserRole.Admin;

        BuildLayout();
        FillFilters();
        RefreshGrid();
    }

    private void BuildLayout()
    {
        BackColor = AppTheme.MainBackground;
        Font = AppTheme.RegularFont;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var tools = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            Padding = new Padding(0, 4, 0, 4)
        };

        _searchTextBox.Width = 220;
        _searchTextBox.PlaceholderText = "Поиск по названию или артикулу";
        _searchTextBox.TextChanged += (_, _) => RefreshGrid();

        _brandComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _brandComboBox.Width = 150;
        _brandComboBox.SelectedIndexChanged += (_, _) => RefreshGrid();

        _sizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _sizeComboBox.Width = 110;
        _sizeComboBox.SelectedIndexChanged += (_, _) => RefreshGrid();

        _sortComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _sortComboBox.Width = 140;
        _sortComboBox.Items.AddRange(new object[] { "Название А-Я", "Цена ↑", "Цена ↓", "Скидка ↓" });
        _sortComboBox.SelectedIndex = 0;
        _sortComboBox.SelectedIndexChanged += (_, _) => RefreshGrid();

        _refreshButton.Text = "Обновить";
        _refreshButton.Width = 100;
        _refreshButton.Height = 34;
        _refreshButton.Click += (_, _) => RefreshGrid();
        AppTheme.ApplySecondaryButton(_refreshButton);

        _addButton.Text = "Добавить";
        _addButton.Width = 100;
        _addButton.Height = 34;
        _addButton.Click += AddButton_Click;
        AppTheme.ApplyPrimaryButton(_addButton);

        _editButton.Text = "Изменить";
        _editButton.Width = 100;
        _editButton.Height = 34;
        _editButton.Click += EditButton_Click;
        AppTheme.ApplySecondaryButton(_editButton);

        _deleteButton.Text = "Удалить";
        _deleteButton.Width = 100;
        _deleteButton.Height = 34;
        _deleteButton.Click += DeleteButton_Click;
        AppTheme.ApplyDangerButton(_deleteButton);

        if (_advancedCatalog)
        {
            tools.Controls.Add(new Label { Text = "Поиск:", AutoSize = true, Padding = new Padding(0, 8, 2, 0) });
            tools.Controls.Add(_searchTextBox);
            tools.Controls.Add(new Label { Text = "Бренд:", AutoSize = true, Padding = new Padding(10, 8, 2, 0) });
            tools.Controls.Add(_brandComboBox);
            tools.Controls.Add(new Label { Text = "Размер:", AutoSize = true, Padding = new Padding(10, 8, 2, 0) });
            tools.Controls.Add(_sizeComboBox);
            tools.Controls.Add(new Label { Text = "Сортировка:", AutoSize = true, Padding = new Padding(10, 8, 2, 0) });
            tools.Controls.Add(_sortComboBox);
        }

        tools.Controls.Add(_refreshButton);

        if (_canEditProducts)
        {
            tools.Controls.Add(_addButton);
            tools.Controls.Add(_editButton);
            tools.Controls.Add(_deleteButton);
        }

        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.White;
        _grid.RowPrePaint += Grid_RowPrePaint;
        _grid.CellDoubleClick += (_, _) =>
        {
            if (_canEditProducts)
            {
                EditSelectedProduct();
            }
        };

        AddColumn(nameof(ProductRow.Id), "ID", 55);
        AddColumn(nameof(ProductRow.Name), "Название", 170);
        AddColumn(nameof(ProductRow.Article), "Артикул", 95);
        AddColumn(nameof(ProductRow.Category), "Категория", 105);
        AddColumn(nameof(ProductRow.Brand), "Бренд", 105);
        AddColumn(nameof(ProductRow.Size), "Размер", 75);
        AddColumn(nameof(ProductRow.Price), "Цена", 90, "N2");
        AddColumn(nameof(ProductRow.DiscountPercent), "Скидка %", 80);
        AddColumn(nameof(ProductRow.FinalPrice), "Цена со скидкой", 110, "N2");
        AddColumn(nameof(ProductRow.Quantity), "Остаток", 80);
        AddColumn(nameof(ProductRow.Supplier), "Поставщик", 120);

        _countLabel.Dock = DockStyle.Fill;
        _countLabel.ForeColor = AppTheme.MutedText;
        _countLabel.TextAlign = ContentAlignment.MiddleLeft;

        root.Controls.Add(tools, 0, 0);
        root.Controls.Add(_grid, 0, 1);
        root.Controls.Add(_countLabel, 0, 2);

        Controls.Add(root);
    }

    private void AddColumn(string dataProperty, string header, int width, string? format = null)
    {
        var column = new DataGridViewTextBoxColumn
        {
            DataPropertyName = dataProperty,
            HeaderText = header,
            Name = dataProperty,
            FillWeight = width
        };

        if (!string.IsNullOrWhiteSpace(format))
        {
            column.DefaultCellStyle.Format = format;
        }

        _grid.Columns.Add(column);
    }

    private void FillFilters()
    {
        _brandComboBox.Items.Clear();
        _brandComboBox.Items.Add("Все");
        foreach (var brand in _store.GetBrands())
        {
            _brandComboBox.Items.Add(brand);
        }
        _brandComboBox.SelectedIndex = 0;

        _sizeComboBox.Items.Clear();
        _sizeComboBox.Items.Add("Все");
        foreach (var size in _store.GetSizes())
        {
            _sizeComboBox.Items.Add(size);
        }
        _sizeComboBox.SelectedIndex = 0;
    }

    private void RefreshGrid()
    {
        var brand = _brandComboBox.SelectedIndex > 0 ? _brandComboBox.Text : "";
        var size = _sizeComboBox.SelectedIndex > 0 ? _sizeComboBox.Text : "";
        var rows = _store.GetProductRows(
            _searchTextBox.Text.Trim(),
            brand,
            size,
            _sortComboBox.Text,
            _advancedCatalog);

        _grid.DataSource = rows;
        _countLabel.Text = _advancedCatalog
            ? $"Показано товаров: {rows.Count}. Строки со скидкой больше 15% выделяются цветом #2E8B57."
            : $"Показано товаров: {rows.Count}. Для роли {_role} поиск, фильтры и сортировка скрыты.";
    }

    private void Grid_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
    {
        if (_grid.Rows[e.RowIndex].DataBoundItem is not ProductRow row)
        {
            return;
        }

        var style = _grid.Rows[e.RowIndex].DefaultCellStyle;
        if (row.DiscountPercent > 15)
        {
            style.BackColor = AppTheme.DiscountBackground;
            style.ForeColor = Color.White;
            style.SelectionBackColor = Color.FromArgb(20, 100, 60);
            style.SelectionForeColor = Color.White;
        }
        else if (row.Quantity == 0)
        {
            style.BackColor = Color.FromArgb(225, 240, 255);
            style.ForeColor = AppTheme.Text;
        }
        else
        {
            style.BackColor = Color.White;
            style.ForeColor = AppTheme.Text;
        }
    }

    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var form = new ProductEditorForm(_store, null);
        if (form.ShowDialog(this) == DialogResult.OK && form.Product != null)
        {
            _store.AddProduct(form.Product);
            FillFilters();
            RefreshGrid();
        }
    }

    private void EditButton_Click(object? sender, EventArgs e)
    {
        EditSelectedProduct();
    }

    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        var productId = GetSelectedProductId();
        if (productId == null)
        {
            return;
        }

        var product = _store.GetProduct(productId.Value);
        if (product == null)
        {
            return;
        }

        var result = MessageBox.Show(
            $"Удалить товар «{product.Name}»?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _store.DeleteProduct(product.Id);
            FillFilters();
            RefreshGrid();
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Удаление невозможно", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void EditSelectedProduct()
    {
        var productId = GetSelectedProductId();
        if (productId == null)
        {
            return;
        }

        var product = _store.GetProduct(productId.Value);
        if (product == null)
        {
            MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var form = new ProductEditorForm(_store, product);
        if (form.ShowDialog(this) == DialogResult.OK && form.Product != null)
        {
            _store.UpdateProduct(form.Product);
            FillFilters();
            RefreshGrid();
        }
    }

    private int? GetSelectedProductId()
    {
        if (_grid.CurrentRow?.DataBoundItem is ProductRow row)
        {
            return row.Id;
        }

        MessageBox.Show("Выберите товар в таблице.", "Нет выбранной записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return null;
    }
}
