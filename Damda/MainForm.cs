using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Damda
{
    public partial class MainForm : Form
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader reader;
        string sql;
        public MainForm()
        {
            InitializeComponent();
            //SqlInitialize();
            //InsertSQL();
        }
        private void SqlInitialize()
        {
            string conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\DAMDA\DB\damda.mdf;Integrated Security=True";
            con = new SqlConnection(conStr);
            sql = "SELECT * FROM tbCustomer ORDER BY id DESC";
            cmd = new SqlCommand(sql, con);
        }

        private void InsertSQL()
        {

            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string name = reader["name"].ToString();
                int spent = Convert.ToInt32(reader["totalSpent"]);
                // 리스트뷰나 DataGridView에 추가
            }
            reader.Close();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            btnHome_Click(sender, e);
        }

        private void ChangePanel(object _form)
        {
            if (containerPanel.Controls.Count > 0) containerPanel.Controls.Clear();
            Form fm = _form as Form;                          // 외부에서 전달된 Form 인스턴스를 형변환
            fm.TopLevel = false;                              // 최상위 폼이 아님 (다른 폼 내에 포함될 수 있도록 설정)
            fm.FormBorderStyle = FormBorderStyle.None;        // 테두리 제거 (내부 폼처럼 보이게)
            fm.Dock = DockStyle.Fill;                         // 부모 컨테이너에 꽉 차도록

            containerPanel.Controls.Add(fm);            // containerPanel에 추가
            containerPanel.Tag = fm;                    // 참조 보관 (나중에 찾기 위해)
            fm.Show();
        }

        #region "[     버튼 패널 구현     ]"
        private void btnHome_Click(object sender, EventArgs e) => ChangePanel(new MenuHome());
        private void btnSale_Click(object sender, EventArgs e) => ChangePanel(new MenuSale());
        private void btnCustomer_Click(object sender, EventArgs e) => ChangePanel(new MenuCustomer());
        private void btnStat_Click(object sender, EventArgs e) => ChangePanel(new MenuStat());
        private void btnDeal_Click(object sender, EventArgs e) => ChangePanel(new MenuHistory());
        private void btnExport_Click(object sender, EventArgs e) => ChangePanel(new MenuExport());
        private void guna2Button1_Click(object sender, EventArgs e) => ChangePanel(new MenuSettings());
        #endregion
    }
}
