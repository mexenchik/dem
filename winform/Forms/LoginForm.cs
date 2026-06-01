using LightStepWinForms.App;
using LightStepWinForms.Data;
using LightStepWinForms.Models;

namespace LightStepWinForms.Forms;

internal partial class LoginForm : Form
{
    private readonly DemoDataStore _store;

    public LoginForm(DemoDataStore store)
    {
        _store = store;
        InitializeComponent();
        AppTheme.ApplyForm(this);
        LoadLogo();
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        headerPanel.BackColor = AppTheme.SecondaryBackground;
        titleLabel.Font = AppTheme.TitleFont;
        subtitleLabel.Font = AppTheme.RegularFont;
        loginButton.BackColor = AppTheme.Accent;
        AppTheme.ApplyPrimaryButton(loginButton);
        AppTheme.ApplySecondaryButton(guestButton);
        AppTheme.ApplySecondaryButton(exitButton);
    }

    private void LoadLogo()
    {
        var logo = ImageLoader.TryLoadLogo();
        if (logo == null)
        {
            logoLabel.Visible = true;
            logoPictureBox.Visible = false;
            return;
        }

        logoPictureBox.Image = logo;
        logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        logoLabel.Visible = false;
    }

    private void LoginButton_Click(object? sender, EventArgs e)
    {
        var user = _store.Authenticate(loginTextBox.Text.Trim(), passwordTextBox.Text);
        if (user == null)
        {
            MessageBox.Show(
                "Неверный логин или пароль.",
                "Ошибка авторизации",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            passwordTextBox.SelectAll();
            passwordTextBox.Focus();
            return;
        }

        OpenMain(user);
    }

    private void GuestButton_Click(object? sender, EventArgs e)
    {
        OpenMain(null);
    }

    private void ExitButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void OpenMain(AppUser? user)
    {
        Hide();
        using var mainForm = new MainForm(_store, user);
        mainForm.ShowDialog(this);
        passwordTextBox.Clear();
        Show();
        loginTextBox.Focus();
    }
}
