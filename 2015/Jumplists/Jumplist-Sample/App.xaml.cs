using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Shell;

namespace Jumplist_Sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void JumpList_JumpItemsRejected(object sender, JumpItemsRejectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void JumpList_JumpItemsRemovedByUser(object sender, JumpItemsRemovedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
