using PageDesigner.Forms;

namespace PageDesigner
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (args.Length == 2)
            {
                Application.Run(new PageDesignerForm(args[0], args[1]));
            }
            else
            {
                Application.Run(new PageDesignerForm());
            }
        }
    }
}