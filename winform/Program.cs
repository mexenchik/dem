using LightStepWinForms.Data;
using LightStepWinForms.Forms;

namespace LightStepWinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        var store = DemoDataStore.CreateSeeded();
        Application.Run(new LoginForm(store));
    }
}
