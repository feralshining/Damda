using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Damda
{
    public partial class MenuSettings : Form
    {
        //Default 값
        public static int GradeSilver { get; set; } = 300000;
        public static int GradeGold { get; set; } = 600000;
        public MenuSettings()
        {
            InitializeComponent();
            txtGradeSilver.Text = GradeSilver.ToString();
            txtGradeGold.Text = GradeGold.ToString();
        }

        private void txtGradeSilver_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(txtGradeSilver?.Text, out int silver);
            GradeSilver = silver;
        }

        private void txtGradeGold_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(txtGradeGold?.Text, out int gold);
            GradeGold = gold;
        }
    }
}
