namespace LightStepWinForms.Forms;

internal partial class ProductEditorForm
{
    private Label headerLabel = null!;
    private TableLayoutPanel layout = null!;
    private TextBox nameTextBox = null!;
    private TextBox articleTextBox = null!;
    private TextBox categoryTextBox = null!;
    private TextBox brandTextBox = null!;
    private TextBox supplierTextBox = null!;
    private TextBox sizeTextBox = null!;
    private TextBox unitTextBox = null!;
    private TextBox descriptionTextBox = null!;
    private NumericUpDown priceNumeric = null!;
    private NumericUpDown quantityNumeric = null!;
    private NumericUpDown discountNumeric = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    private void InitializeComponent()
    {
        headerLabel = new Label();
        layout = new TableLayoutPanel();
        nameTextBox = new TextBox();
        articleTextBox = new TextBox();
        categoryTextBox = new TextBox();
        brandTextBox = new TextBox();
        supplierTextBox = new TextBox();
        sizeTextBox = new TextBox();
        unitTextBox = new TextBox();
        descriptionTextBox = new TextBox();
        priceNumeric = new NumericUpDown();
        quantityNumeric = new NumericUpDown();
        discountNumeric = new NumericUpDown();
        saveButton = new Button();
        cancelButton = new Button();

        SuspendLayout();
        layout.SuspendLayout();

        ClientSize = new Size(620, 570);
        MinimumSize = new Size(580, 540);
        MaximizeBox = false;

        headerLabel.Dock = DockStyle.Top;
        headerLabel.Height = 48;
        headerLabel.Text = "Карточка товара";
        headerLabel.TextAlign = ContentAlignment.MiddleCenter;

        layout.Dock = DockStyle.Fill;
        layout.Padding = new Padding(18);
        layout.ColumnCount = 2;
        layout.RowCount = 12;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (var i = 0; i < 11; i++)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 7 ? 86 : 38));
        }
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        AddRow(0, "Название*", nameTextBox);
        AddRow(1, "Артикул*", articleTextBox);
        AddRow(2, "Категория*", categoryTextBox);
        AddRow(3, "Бренд*", brandTextBox);
        AddRow(4, "Поставщик*", supplierTextBox);
        AddRow(5, "Размер*", sizeTextBox);
        AddRow(6, "Ед. измерения*", unitTextBox);

        descriptionTextBox.Multiline = true;
        descriptionTextBox.ScrollBars = ScrollBars.Vertical;
        AddRow(7, "Описание", descriptionTextBox);

        priceNumeric.DecimalPlaces = 2;
        priceNumeric.Maximum = 1_000_000;
        priceNumeric.Minimum = 0;
        priceNumeric.ThousandsSeparator = true;
        AddRow(8, "Цена*", priceNumeric);

        quantityNumeric.Maximum = 1_000_000;
        quantityNumeric.Minimum = 0;
        AddRow(9, "Количество*", quantityNumeric);

        discountNumeric.Maximum = 100;
        discountNumeric.Minimum = 0;
        AddRow(10, "Скидка %", discountNumeric);

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };
        saveButton.Text = "Сохранить";
        saveButton.Width = 120;
        saveButton.Height = 36;
        saveButton.Click += SaveButton_Click;

        cancelButton.Text = "Отмена";
        cancelButton.Width = 120;
        cancelButton.Height = 36;
        cancelButton.Click += CancelButton_Click;

        buttonsPanel.Controls.Add(saveButton);
        buttonsPanel.Controls.Add(cancelButton);
        layout.Controls.Add(buttonsPanel, 0, 11);
        layout.SetColumnSpan(buttonsPanel, 2);

        Controls.Add(layout);
        Controls.Add(headerLabel);
        AcceptButton = saveButton;
        CancelButton = cancelButton;

        layout.ResumeLayout(false);
        layout.PerformLayout();
        ResumeLayout(false);
    }

    private void AddRow(int row, string labelText, Control control)
    {
        var label = new Label
        {
            Text = labelText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(4, 4, 4, 4);
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(control, 1, row);
    }
}
