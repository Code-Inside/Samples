using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebHistory.Model;
using WebHistory.Service;

namespace WebHistory.WinApp
{
    public partial class Form1 : Form
    {
        public Website SearchResult = new Website();
        public int DisplayCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IArchiveService service = new WebArchiveService();
            this.SearchResult = service.Load(this.textBox1.Text);
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
            if (this.SearchResult.ArchiveWebsites.Count > 0)
            {
                this.ShowNextWebsite(0);
            }
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.DisplayCount++;
            this.ShowNextWebsite(this.DisplayCount);
        }

        private void ShowNextWebsite(int id)
        {
            this.webBrowser1.Navigate(this.SearchResult.ArchiveWebsites[id].ArchiveUrl);
            this.label2.Text = this.SearchResult.ArchiveWebsites[id].Date.ToShortDateString();
        }


    }
}
