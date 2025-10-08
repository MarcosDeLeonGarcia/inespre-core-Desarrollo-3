using System;
using System.Windows.Forms;
using INESPRE.Desktop.UI.Forms;  // <-- IMPORTANTE

namespace INESPRE.Desktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());   // <-- ya ve LoginForm por el using de arriba
        }
    }
}
