using System.Globalization;
using GalaSoft.MvvmLight.Ioc;
using Infralution.Localization.Wpf;
using MouseBuddy.Net.Model;
using MouseBuddy.Services;

namespace MouseBuddy.Views
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<SettingsService>();
            SimpleIoc.Default.Register<MainVM>();
            SimpleIoc.Default.Register<MouseService>(true);

            //Setting language for whole application
            CultureManager.UICulture = new CultureInfo(SimpleIoc.Default.GetInstance<SettingsService>().Settings.Language);
        }
        public MainVM MainVM => SimpleIoc.Default.GetInstance<MainVM>();

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            
        }
    }
}