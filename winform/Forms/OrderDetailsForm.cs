using LightStepWinForms.App;
using LightStepWinForms.Data;

namespace LightStepWinForms.Forms;

internal sealed class OrderDetailsForm : Form
{
    public OrderDetailsForm(DemoDataStore store, int orderId)
    {
        AppTheme.ApplyForm(this);
        Text = $"Состав заказа №{orderId}";
        ClientSize = new Size(720, 420);
        MinimumSize = new Size(620, 360);

        var order = store.GetOrderRows().FirstOrDefault(row => row.Id == orderId);
        var header = new Label
        {
            Dock = DockStyle.Top,
            Height = 58,
            BackColor = AppTheme.SecondaryBackground,
            Font = AppTheme.HeaderFont,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = order == null
                ? $"Заказ №{orderId}"
                : $"Заказ №{order.Id} | {order.ClientName} | {order.Status} | {order.TotalAmount:N2} руб."
        };

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.White,
            DataSource = store.GetOrderItemRows(orderId)
        };

        AddColumn(grid, "ProductName", "Товар", 180);
        AddColumn(grid, "Article", "Артикул", 90);
        AddColumn(grid, "Count", "Кол-во", 70);
        AddColumn(grid, "PriceAtMoment", "Цена", 90, "N2");
        AddColumn(grid, "Sum", "Сумма", 90, "N2");

        Controls.Add(grid);
        Controls.Add(header);
    }

    private static void AddColumn(DataGridView grid, string property, string header, int width, string? format = null)
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
        grid.Columns.Add(column);
    }
}
