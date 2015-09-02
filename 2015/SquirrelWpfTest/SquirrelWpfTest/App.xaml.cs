using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;

namespace SquirrelWpfTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {
                using (var mgr = new UpdateManager(ConfigurationManager.AppSettings["UpdatePath"]))
                {
                    // Note, in most of these scenarios, the app exits after this method
                    // completes!
                    
                    SquirrelAwareApp.HandleEvents(
                        onInitialInstall: v => mgr.CreateShortcutForThisExe(),
                        onAppUpdate: v => mgr.CreateShortcutForThisExe(),
                        onAppUninstall: v => mgr.RemoveShortcutForThisExe());
                }
            }
            catch (Exception)
            {
                
            }
           
        }
    }
}
