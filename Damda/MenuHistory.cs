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
    public partial class MenuHistory : Form
    {
        public MenuHistory()
        {
            InitializeComponent();
            SetUIFont();
        }
        private void SetUIFont()
        {
            menuLabel.Font = FontManager.CookieRun_22;
            guna2GroupBox1.Font = FontManager.Kookmin_9;
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.Font = FontManager.Kookmin_9;
            guna2DataGridView1.DefaultCellStyle.Font = FontManager.Kookmin_9;
            guna2DataGridView1.ThemeStyle.HeaderStyle.Font = FontManager.Kookmin_9;
            guna2DataGridView1.ThemeStyle.RowsStyle.Font = FontManager.Kookmin_9;
            guna2ComboBox1.Font = FontManager.CookieRun_14;
            guna2ComboBox2.Font = FontManager.CookieRun_14;
            guna2TextBox1.Font = FontManager.CookieRun_14;
        }

    }
}
