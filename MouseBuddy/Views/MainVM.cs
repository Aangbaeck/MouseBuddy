using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MouseBuddy.Net.Helper;
using MouseBuddy.Services;
using WpfScreenHelper;

namespace MouseBuddy.Views
{
    public class MainVM : ObservableObject
    {
        public string Specs { get; set; } = "";

        public MainVM(MouseService mouseService)
        {
            MS = mouseService;
            foreach (var screen in Screen.AllScreens)
            {
                // For each screen, add the screen properties to a list box.
                Specs += "Device Name: " + screen.DeviceName + "\n";
                Specs += "Bounds: " + screen.Bounds.ToString() + "\n";
                Specs += "Type: " + screen.GetType().ToString() + "\n";
                Specs += "Working Area: " + screen.WorkingArea.ToString() + "\n";
                Specs += "Primary Screen: " + screen.Primary.ToString() + "\n\n";
            }
        }
        public RelayCommand OpenLogFile => new RelayCommand(() =>
        {
            var directory = Path.GetDirectoryName(Common.LogfilesPath);
            var files = Directory.GetFiles(directory, "*.log");
            if (files.Length > 0)
            {
                var filePath = files[^1];  //^1 is the same as files.Length-1
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }

        });

        public MouseService MS { get; }
    }
}