using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BlogPosts.Buzzwords.Client
{
    public partial class ResultsGrid : Form
    {
        public ResultsGrid(object dataSource)
            : this()
        {
            this.source.DataSource = dataSource;
            this.dataResults.DataSource = this.source;
        }

        public ResultsGrid()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}