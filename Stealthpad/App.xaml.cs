using System.Configuration;
using System.Data;
using System.Windows;
using System.IO;

namespace Stealthpad
{
    
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = new MainWindow(); // Create an instance of MainWindow first

            if (e.Args.Length > 0)
            {
                string filePath = e.Args[0];
                Console.WriteLine(e.Args);
                if (File.Exists(filePath) && Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    mainWindow.OpenFile(filePath); // Call a method to open the file
                }
                else
                {
                    MessageBox.Show("The selected file does not exist or is not a valid text file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            mainWindow.Show(); // Show the main window only once
        }
    }
}
