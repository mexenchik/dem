namespace LightStepWinForms.Forms;

internal partial class LoginForm
{
    private Panel headerPanel = null!;
    private PictureBox logoPictureBox = null!;
    private Label logoLabel = null!;
    private Label titleLabel = null!;
    private Label subtitleLabel = null!;
    private TableLayoutPanel loginLayout = null!;
    private TextBox loginTextBox = null!;
    private TextBox passwordTextBox = null!;
    private Button loginButton = null!;
    private Button guestButton = null!;
    private Button exitButton = null!;
    private Label accountsLabel = null!;

    private void InitializeComponent()
    {
        headerPanel = new Panel();
        logoPictureBox = new PictureBox();
        logoLabel = new Label();
        titleLabel = new Label();
        subtitleLabel = new Label();
        loginLayout = new TableLayoutPanel();
        loginTextBox = new TextBox();
        passwordTextBox = new TextBox();
        loginButton = new Button();
        guestButton = new Button();
        exitButton = new Button();
        accountsLabel = new Label();

        SuspendLayout();
        headerPanel.SuspendLayout();
        loginLayout.SuspendLayout();

        Text = "ООО «Легкий шаг» - вход";
        ClientSize = new Size(520, 430);
        MinimumSize = new Size(500, 420);
        MaximizeBox = false;

        headerPanel.Dock = DockStyle.Top;
        headerPanel.Height = 110;
        headerPanel.Padding = new Padding(20, 14, 20, 14);

        logoPictureBox.Location = new Point(20, 18);
        logoPictureBox.Size = new Size(72, 72);
        logoPictureBox.TabStop = false;

        logoLabel.Location = new Point(20, 18);
        logoLabel.Size = new Size(72, 72);
        logoLabel.Text = "ЛШ";
        logoLabel.TextAlign = ContentAlignment.MiddleCenter;
        logoLabel.Font = new Font("Times New Roman", 20F, FontStyle.Bold);
        logoLabel.BorderStyle = BorderStyle.FixedSingle;

        titleLabel.AutoSize = false;
        titleLabel.Location = new Point(110, 20);
        titleLabel.Size = new Size(370, 36);
        titleLabel.Text = "ООО «Легкий шаг»";
        titleLabel.TextAlign = ContentAlignment.MiddleLeft;

        subtitleLabel.AutoSize = false;
        subtitleLabel.Location = new Point(112, 58);
        subtitleLabel.Size = new Size(360, 34);
        subtitleLabel.Text = "Автоматизированное рабочее место";
        subtitleLabel.TextAlign = ContentAlignment.MiddleLeft;

        headerPanel.Controls.Add(logoPictureBox);
        headerPanel.Controls.Add(logoLabel);
        headerPanel.Controls.Add(titleLabel);
        headerPanel.Controls.Add(subtitleLabel);

        loginLayout.Dock = DockStyle.Fill;
        loginLayout.Padding = new Padding(34, 28, 34, 24);
        loginLayout.ColumnCount = 2;
        loginLayout.RowCount = 7;
        loginLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        loginLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        loginLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        loginLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        loginLayout.Controls.Add(new Label
        {
            Text = "Логин",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        loginTextBox.Dock = DockStyle.Fill;
        loginTextBox.Margin = new Padding(4, 5, 4, 5);
        loginTextBox.PlaceholderText = "admin";
        loginLayout.Controls.Add(loginTextBox, 1, 0);

        loginLayout.Controls.Add(new Label
        {
            Text = "Пароль",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);
        passwordTextBox.Dock = DockStyle.Fill;
        passwordTextBox.Margin = new Padding(4, 5, 4, 5);
        passwordTextBox.UseSystemPasswordChar = true;
        passwordTextBox.PlaceholderText = "admin123";
        loginLayout.Controls.Add(passwordTextBox, 1, 1);

        loginButton.Dock = DockStyle.Fill;
        loginButton.Text = "Войти";
        loginButton.Click += LoginButton_Click;
        loginLayout.Controls.Add(loginButton, 0, 2);
        loginLayout.SetColumnSpan(loginButton, 2);

        guestButton.Dock = DockStyle.Fill;
        guestButton.Text = "Войти как гость";
        guestButton.Click += GuestButton_Click;
        loginLayout.Controls.Add(guestButton, 0, 3);
        loginLayout.SetColumnSpan(guestButton, 2);

        exitButton.Dock = DockStyle.Fill;
        exitButton.Text = "Выход";
        exitButton.Click += ExitButton_Click;
        loginLayout.Controls.Add(exitButton, 0, 4);
        loginLayout.SetColumnSpan(exitButton, 2);

        accountsLabel.Dock = DockStyle.Fill;
        accountsLabel.ForeColor = Color.FromArgb(80, 90, 80);
        accountsLabel.Text = "Тестовые учетные записи:\r\nclient / client123\r\nmanager / manager123\r\nadmin / admin123";
        accountsLabel.TextAlign = ContentAlignment.TopLeft;
        loginLayout.Controls.Add(accountsLabel, 0, 5);
        loginLayout.SetColumnSpan(accountsLabel, 2);

        Controls.Add(loginLayout);
        Controls.Add(headerPanel);
        AcceptButton = loginButton;
        CancelButton = exitButton;

        headerPanel.ResumeLayout(false);
        loginLayout.ResumeLayout(false);
        loginLayout.PerformLayout();
        ResumeLayout(false);
    }
}
