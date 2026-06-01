namespace LightStepWinForms.Forms;

internal partial class OrderEditorForm
{
    private Label headerLabel = null!;
    private TableLayoutPanel rootLayout = null!;
    private ComboBox clientComboBox = null!;
    private DateTimePicker orderDatePicker = null!;
    private ComboBox statusComboBox = null!;
    private ComboBox productComboBox = null!;
    private NumericUpDown countNumeric = null!;
    private Button addItemButton = null!;
    private Button removeItemButton = null!;
    private DataGridView itemsGrid = null!;
    private Label totalLabel = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;

    private void InitializeComponent()
    {
        headerLabel = new Label();
        rootLayout = new TableLayoutPanel();
        clientComboBox = new ComboBox();
        orderDatePicker = new DateTimePicker();
        statusComboBox = new ComboBox();
        productComboBox = new ComboBox();
        countNumeric = new NumericUpDown();
        addItemButton = new Button();
        removeItemButton = new Button();
        itemsGrid = new DataGridView();
        totalLabel = new Label();
        saveButton = new Button();
        cancelButton = new Button();

        SuspendLayout();
        rootLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)itemsGrid).BeginInit();

        ClientSize = new Size(760, 620);
        MinimumSize = new Size(700, 560);
        MaximizeBox = false;

        headerLabel.Dock = DockStyle.Top;
        headerLabel.Height = 48;
        headerLabel.Text = "Карточка заказа";
        headerLabel.TextAlign = ContentAlignment.MiddleCenter;

        rootLayout.Dock = DockStyle.Fill;
        rootLayout.Padding = new Padding(16);
        rootLayout.ColumnCount = 4;
        rootLayout.RowCount = 8;
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 8));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 4));

        clientComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        statusComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        productComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        AddLabel("Клиент", 0, 0);
        clientComboBox.Dock = DockStyle.Fill;
        rootLayout.Controls.Add(clientComboBox, 1, 0);
        rootLayout.SetColumnSpan(clientComboBox, 3);

        AddLabel("Дата", 0, 1);
        orderDatePicker.Dock = DockStyle.Fill;
        orderDatePicker.Format = DateTimePickerFormat.Custom;
        orderDatePicker.CustomFormat = "dd.MM.yyyy HH:mm";
        rootLayout.Controls.Add(orderDatePicker, 1, 1);

        AddLabel("Статус", 2, 1);
        statusComboBox.Dock = DockStyle.Fill;
        rootLayout.Controls.Add(statusComboBox, 3, 1);

        AddLabel("Товар", 0, 2);
        productComboBox.Dock = DockStyle.Fill;
        rootLayout.Controls.Add(productComboBox, 1, 2);

        AddLabel("Кол-во", 2, 2);
        var itemTools = new FlowLayoutPanel { Dock = DockStyle.Fill };
        countNumeric.Minimum = 1;
        countNumeric.Maximum = 1000;
        countNumeric.Value = 1;
        countNumeric.Width = 70;
        addItemButton.Text = "Добавить";
        addItemButton.Width = 100;
        addItemButton.Click += AddItemButton_Click;
        removeItemButton.Text = "Удалить строку";
        removeItemButton.Width = 120;
        removeItemButton.Click += RemoveItemButton_Click;
        itemTools.Controls.Add(countNumeric);
        itemTools.Controls.Add(addItemButton);
        itemTools.Controls.Add(removeItemButton);
        rootLayout.Controls.Add(itemTools, 3, 2);

        itemsGrid.Dock = DockStyle.Fill;
        itemsGrid.ReadOnly = true;
        itemsGrid.AllowUserToAddRows = false;
        itemsGrid.AllowUserToDeleteRows = false;
        itemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        itemsGrid.MultiSelect = false;
        itemsGrid.AutoGenerateColumns = false;
        itemsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        itemsGrid.BackgroundColor = Color.White;
        AddGridColumn(nameof(LightStepWinForms.Models.OrderItemRow.ProductName), "Товар", 180);
        AddGridColumn(nameof(LightStepWinForms.Models.OrderItemRow.Article), "Артикул", 90);
        AddGridColumn(nameof(LightStepWinForms.Models.OrderItemRow.Count), "Кол-во", 70);
        AddGridColumn(nameof(LightStepWinForms.Models.OrderItemRow.PriceAtMoment), "Цена", 90, "N2");
        AddGridColumn(nameof(LightStepWinForms.Models.OrderItemRow.Sum), "Сумма", 90, "N2");
        rootLayout.Controls.Add(itemsGrid, 0, 3);
        rootLayout.SetColumnSpan(itemsGrid, 4);

        totalLabel.Dock = DockStyle.Fill;
        totalLabel.TextAlign = ContentAlignment.MiddleRight;
        totalLabel.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
        rootLayout.Controls.Add(totalLabel, 0, 4);
        rootLayout.SetColumnSpan(totalLabel, 4);

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
        rootLayout.Controls.Add(buttonsPanel, 0, 6);
        rootLayout.SetColumnSpan(buttonsPanel, 4);

        Controls.Add(rootLayout);
        Controls.Add(headerLabel);
        AcceptButton = saveButton;
        CancelButton = cancelButton;

        ((System.ComponentModel.ISupportInitialize)itemsGrid).EndInit();
        rootLayout.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void AddLabel(string text, int column, int row)
    {
        rootLayout.Controls.Add(new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, column, row);
    }

    private void AddGridColumn(string property, string header, int width, string format = "")
    {
        var column = new DataGridViewTextBoxColumn
        {
            Name = property,
            DataPropertyName = property,
            HeaderText = header,
            FillWeight = width
        };
        if (!string.IsNullOrWhiteSpace(format))
        {
            column.DefaultCellStyle.Format = format;
        }
        itemsGrid.Columns.Add(column);
    }
}
