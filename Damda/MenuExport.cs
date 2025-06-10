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
    public partial class MenuExport : Form
    {
        public MenuExport()
        {
            InitializeComponent();
            SetUIFont();
        }
        private void SetUIFont()
        {
            menuLabel.Font = FontManager.CookieRun_22;
            guna2GroupBox1.Font = FontManager.Kookmin_9;
            guna2GroupBox2.Font = FontManager.Kookmin_9;
            label5.Font = FontManager.Kookmin_16;
            guna2Button1.Font = FontManager.CookieRun_12;
            guna2Button2.Font = FontManager.CookieRun_12;
            label1.Font = FontManager.Kookmin_16;
            label2.Font = FontManager.Kookmin_12;
            guna2ComboBox2.Font = FontManager.Kookmin_10;
            label3.Font = FontManager.Kookmin_12;
            guna2ComboBox1.Font = FontManager.Kookmin_10;
            itemText.Font = FontManager.Kookmin_9;
            label4.Font = FontManager.Kookmin_12;
            guna2Button3.Font = FontManager.CookieRun_12;
            guna2Button4.Font = FontManager.CookieRun_12;
            guna2Button5.Font = FontManager.CookieRun_12;
            guna2Button6.Font = FontManager.CookieRun_12;
        }
    }
}
