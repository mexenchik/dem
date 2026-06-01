using LightStepWinForms.App;
using LightStepWinForms.Controls;
using LightStepWinForms.Data;
using LightStepWinForms.Models;

namespace LightStepWinForms.Forms;

internal partial class MainForm : Form
{
    private readonly DemoDataStore _store;
    private readonly AppUser? _currentUser;
    private readonly UserRole _role;
    private ProductCatalogControl? _productCatalogControl;
    private OrdersControl? _ordersControl;

    public MainForm(DemoDataStore store, AppUser? currentUser)
    {
        _store = store;
        _currentUser = currentUser;
        _role = currentUser?.Role ?? UserRole.Guest;

        InitializeComponent();
        AppTheme.ApplyForm(this);
        ApplyTheme();
        ConfigureAccess();
        ShowCatalog();
    }

    private void ApplyTheme()
    {
        headerPanel.BackColor = AppTheme.SecondaryBackground;
        sidePanel.BackColor = Color.FromArgb(239, 250, 239);
        userLabel.Font = AppTheme.HeaderFont;
        titleLabel.Font = AppTheme.TitleFont;
        contentPanel.BackColor = AppTheme.MainBackground;

        AppTheme.ApplyPrimaryButton(catalogButton);
        AppTheme.ApplySecondaryButton(ordersButton);
        AppTheme.ApplySecondaryButton(helpButton);
        AppTheme.ApplySecondaryButton(logoutButton);

        var logo = ImageLoader.TryLoadLogo();
        if (logo != null)
        {
            logoPictureBox.Image = logo;
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoTextLabel.Visible = false;
        }
    }

    private void ConfigureAccess()
    {
        var displayName = _currentUser?.FullName ?? "Гость";
        userLabel.Text = $"{displayName} | роль: {_role}";
        ordersButton.Visible = _role is UserRole.Manager or UserRole.Admin;
    }

    private void CatalogButton_Click(object? sender, EventArgs e)
    {
        ShowCatalog();
    }

    private void OrdersButton_Click(object? sender, EventArgs e)
    {
        ShowOrders();
    }

    private void HelpButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Роли:\n" +
            "Гость и клиент: просмотр каталога без поиска и фильтров.\n" +
            "Менеджер: каталог с поиском/фильтрами и просмотр заказов.\n" +
            "Администратор: полный CRUD товаров и заказов.",
            "Справка по системе",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void LogoutButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void ShowCatalog()
    {
        contentPanel.Controls.Clear();
        _productCatalogControl ??= new ProductCatalogControl(_store, _role);
        _productCatalogControl.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(_productCatalogControl);
        titleLabel.Text = "Каталог товаров";
    }

    private void ShowOrders()
    {
        contentPanel.Controls.Clear();
        _ordersControl ??= new OrdersControl(_store, _role);
        _ordersControl.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(_ordersControl);
        titleLabel.Text = "Управление заказами";
    }
}
