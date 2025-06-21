using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Linq;
using System.Data.SqlClient;

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
            SetUIFont();
        }
        private void SetUIFont()
        {
            menuLabel.Font = FontManager.CookieRun_22;

            label5.Font = FontManager.Kookmin_16;
            label1.Font = FontManager.Kookmin_16;
            label2.Font = FontManager.Kookmin_16;
            label6.Font = FontManager.Kookmin_16;
            label3.Font = FontManager.Kookmin_16;

            txtGradeSilver.Font = FontManager.Kookmin_9;
            txtGradeGold.Font = FontManager.Kookmin_9;

            btnResetData.Font = FontManager.CookieRun_12;
            btnRestore.Font = FontManager.CookieRun_12;

            label8.Font = FontManager.Kookmin_16;
            label6.Font = FontManager.Kookmin_12;
            label7.Font = FontManager.Kookmin_12;
            cmbExportDateRange.Font = FontManager.Kookmin_10;
            label4.Font = FontManager.Kookmin_12;
            txtExportName.Font = FontManager.Kookmin_9;
            btnExport.Font = FontManager.CookieRun_12;
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

        private void btnResetData_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("정말 모든 데이터를 삭제하시겠습니까?", "데이터 초기화", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string sql = @"
                                DELETE FROM tbSale;
                                DELETE FROM tbCustomer;";
                    DBHelper.ExecuteNonQuery(sql);
                    MessageBox.Show("데이터가 초기화되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("초기화 중 오류 발생: " + ex.Message);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Step 1: 내보낼 테이블 선택
            string table = cmbExportList.SelectedItem.ToString();
            string sql = "";
            string dateColumn = "";

            if (table == "고객 목록")
            {
                sql = "SELECT id, name, phone, registerDate, visitCount, totalSpent, grade, memo FROM tbCustomer WHERE 1 = 1";
                dateColumn = "registerDate";
            }
            else if (table == "판매 내역")
            {
                sql = "SELECT id, customerId, saleDate, item, price, quantity, totalPrice, paymentMethod, memo FROM tbSale WHERE 1 = 1";
                dateColumn = "saleDate";
            }
            else
            {
                MessageBox.Show("내보낼 목록을 선택해주세요.");
                return;
            }

            // Step 2: 기간 조건
            string range = cmbExportDateRange.SelectedItem.ToString();
            if (range == "1주일")
            {
                sql += $" AND {dateColumn} >= DATEADD(DAY, -7, GETDATE())";
            }
            else if (range == "1개월")
            {
                sql += $" AND {dateColumn} >= DATEADD(MONTH, -1, GETDATE())";
            }

            sql += $" ORDER BY {dateColumn} DESC";

            // Step 3: 파일명 확인
            string fileName = txtExportName.Text.Trim();
            if (fileName == "")
            {
                MessageBox.Show("파일 이름을 입력해주세요.");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV 파일|*.csv";
            dialog.FileName = fileName + ".csv";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;

                try
                {
                    DataTable dt = DBHelper.ExecuteDataTable(sql);

                    using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        // Step 4: 컬럼명 출력
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            writer.Write(dt.Columns[i].ColumnName);
                            if (i < dt.Columns.Count - 1)
                                writer.Write(",");
                        }
                        writer.WriteLine();

                        // Step 5: 데이터 출력
                        for (int r = 0; r < dt.Rows.Count; r++)
                        {
                            for (int c = 0; c < dt.Columns.Count; c++)
                            {
                                string value = dt.Rows[r][c].ToString().Replace("\"", "\"\"");
                                writer.Write("\"" + value + "\"");

                                if (c < dt.Columns.Count - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();
                        }
                    }

                    MessageBox.Show("CSV 파일이 성공적으로 저장되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("내보내기 중 오류 발생: " + ex.Message);
                }
            }
        }
        private void btnRestore_Click(object sender, EventArgs e)
        {
            MessageBox.Show("미구현 기능.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
