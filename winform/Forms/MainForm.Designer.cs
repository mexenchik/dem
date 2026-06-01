namespace LightStepWinForms.Forms;

internal partial class MainForm
{
    private Panel headerPanel = null!;
    private Panel sidePanel = null!;
    private Panel contentPanel = null!;
    private PictureBox logoPictureBox = null!;
    private Label logoTextLabel = null!;
    private Label titleLabel = null!;
    private Label userLabel = null!;
    private Button catalogButton = null!;
    private Button ordersButton = null!;
    private Button helpButton = null!;
    private Button logoutButton = null!;

    private void InitializeComponent()
    {
        headerPanel = new Panel();
        sidePanel = new Panel();
        contentPanel = new Panel();
        logoPictureBox = new PictureBox();
        logoTextLabel = new Label();
        titleLabel = new Label();
        userLabel = new Label();
        catalogButton = new Button();
        ordersButton = new Button();
        helpButton = new Button();
        logoutButton = new Button();

        SuspendLayout();
        headerPanel.SuspendLayout();
        sidePanel.SuspendLayout();

        Text = "ООО «Легкий шаг» - информационная система";
        ClientSize = new Size(1180, 720);
        MinimumSize = new Size(1000, 620);

        headerPanel.Dock = DockStyle.Top;
        headerPanel.Height = 78;
        headerPanel.Padding = new Padding(14, 10, 14, 10);

        logoPictureBox.Location = new Point(14, 10);
        logoPictureBox.Size = new Size(58, 58);
        logoPictureBox.TabStop = false;

        logoTextLabel.Location = new Point(14, 10);
        logoTextLabel.Size = new Size(58, 58);
        logoTextLabel.Text = "ЛШ";
        logoTextLabel.TextAlign = ContentAlignment.MiddleCenter;
        logoTextLabel.Font = new Font("Times New Roman", 18F, FontStyle.Bold);
        logoTextLabel.BorderStyle = BorderStyle.FixedSingle;

        titleLabel.AutoSize = false;
        titleLabel.Location = new Point(88, 13);
        titleLabel.Size = new Size(500, 30);
        titleLabel.Text = "Каталог товаров";

        userLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        userLabel.AutoSize = false;
        userLabel.Location = new Point(670, 17);
        userLabel.Size = new Size(360, 26);
        userLabel.TextAlign = ContentAlignment.MiddleRight;

        logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        logoutButton.Location = new Point(1046, 17);
        logoutButton.Size = new Size(110, 36);
        logoutButton.Text = "Выйти";
        logoutButton.Click += LogoutButton_Click;

        headerPanel.Controls.Add(logoPictureBox);
        headerPanel.Controls.Add(logoTextLabel);
        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(userLabel);
        headerPanel.Controls.Add(logoutButton);

        sidePanel.Dock = DockStyle.Left;
        sidePanel.Width = 210;
        sidePanel.Padding = new Padding(14);

        catalogButton.Dock = DockStyle.Top;
        catalogButton.Height = 48;
        catalogButton.Text = "Товары";
        catalogButton.Click += CatalogButton_Click;

        ordersButton.Dock = DockStyle.Top;
        ordersButton.Height = 48;
        ordersButton.Text = "Заказы";
        ordersButton.Margin = new Padding(0, 8, 0, 0);
        ordersButton.Click += OrdersButton_Click;

        helpButton.Dock = DockStyle.Bottom;
        helpButton.Height = 48;
        helpButton.Text = "Справка";
        helpButton.Click += HelpButton_Click;

        sidePanel.Controls.Add(helpButton);
        sidePanel.Controls.Add(ordersButton);
        sidePanel.Controls.Add(catalogButton);

        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Padding = new Padding(14);

        Controls.Add(contentPanel);
        Controls.Add(sidePanel);
        Controls.Add(headerPanel);

        headerPanel.ResumeLayout(false);
        sidePanel.ResumeLayout(false);
        ResumeLayout(false);
    }
}
