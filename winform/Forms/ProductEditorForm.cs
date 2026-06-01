using LightStepWinForms.App;
using LightStepWinForms.Data;
using LightStepWinForms.Models;

namespace LightStepWinForms.Forms;

internal partial class ProductEditorForm : Form
{
    private readonly DemoDataStore _store;
    private readonly int? _productId;

    public ProductEditorForm(DemoDataStore store, Product? product)
    {
        _store = store;
        _productId = product?.Id;
        Product = product?.Clone();

        InitializeComponent();
        AppTheme.ApplyForm(this);
        ApplyTheme();

        if (product != null)
        {
            FillFields(product);
        }
    }

    public Product? Product { get; private set; }

    private void ApplyTheme()
    {
        Text = _productId == null ? "Добавление товара" : "Редактирование товара";
        headerLabel.BackColor = AppTheme.SecondaryBackground;
        headerLabel.Font = AppTheme.HeaderFont;
        AppTheme.ApplyPrimaryButton(saveButton);
        AppTheme.ApplySecondaryButton(cancelButton);
    }

    private void FillFields(Product product)
    {
        nameTextBox.Text = product.Name;
        articleTextBox.Text = product.Article;
        categoryTextBox.Text = product.Category;
        brandTextBox.Text = product.Brand;
        supplierTextBox.Text = product.Supplier;
        sizeTextBox.Text = product.Size;
        unitTextBox.Text = product.Unit;
        descriptionTextBox.Text = product.Description;
        priceNumeric.Value = product.Price;
        quantityNumeric.Value = product.Quantity;
        discountNumeric.Value = product.DiscountPercent;
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (!ValidateFields())
        {
            return;
        }

        Product = new Product
        {
            Id = _productId ?? 0,
            Name = nameTextBox.Text.Trim(),
            Article = articleTextBox.Text.Trim(),
            Category = categoryTextBox.Text.Trim(),
            Brand = brandTextBox.Text.Trim(),
            Supplier = supplierTextBox.Text.Trim(),
            Size = sizeTextBox.Text.Trim(),
            Unit = unitTextBox.Text.Trim(),
            Description = descriptionTextBox.Text.Trim(),
            Price = priceNumeric.Value,
            Quantity = (int)quantityNumeric.Value,
            DiscountPercent = (int)discountNumeric.Value
        };

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateFields()
    {
        if (string.IsNullOrWhiteSpace(nameTextBox.Text)
            || string.IsNullOrWhiteSpace(articleTextBox.Text)
            || string.IsNullOrWhiteSpace(categoryTextBox.Text)
            || string.IsNullOrWhiteSpace(brandTextBox.Text)
            || string.IsNullOrWhiteSpace(supplierTextBox.Text)
            || string.IsNullOrWhiteSpace(sizeTextBox.Text)
            || string.IsNullOrWhiteSpace(unitTextBox.Text))
        {
            MessageBox.Show("Заполните все обязательные поля.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (_store.ArticleExists(articleTextBox.Text.Trim(), _productId))
        {
            MessageBox.Show("Артикул должен быть уникальным.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            articleTextBox.Focus();
            return false;
        }

        if (priceNumeric.Value <= 0)
        {
            MessageBox.Show("Цена должна быть больше 0.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            priceNumeric.Focus();
            return false;
        }

        return true;
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
