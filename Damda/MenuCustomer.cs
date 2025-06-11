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
    public partial class MenuCustomer : Form
    {
        public MenuCustomer()
        {
            InitializeComponent();
            SetUIFont();
        }
        private void SetUIFont()
        {
            menuLabel.Font = FontManager.CookieRun_22;
            guna2GroupBox1.Font = FontManager.Kookmin_9;
            dgvCustomerList.ColumnHeadersDefaultCellStyle.Font = FontManager.Kookmin_9;
            dgvCustomerList.DefaultCellStyle.Font = FontManager.Kookmin_9;
            dgvCustomerList.RowHeadersDefaultCellStyle.Font = FontManager.Kookmin_9;
            dgvCustomerList.ThemeStyle.HeaderStyle.Font = FontManager.Kookmin_9;
            dgvCustomerList.ThemeStyle.RowsStyle.Font = FontManager.Kookmin_9;
            cmbGradeFilter.Font = FontManager.CookieRun_14;
            btnDelete.Font = FontManager.CookieRun_12;
        }
        private void cmbGradeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedGrade = cmbGradeFilter.SelectedItem?.ToString() ?? "전체";
            string sql;
            Dictionary<string, object> param = new Dictionary<string, object>();

            if (selectedGrade == "전체")
            {
                sql = @"
                    SELECT name AS 이름, phone AS 연락처, registerDate AS 등록일, visitCount AS '방문 횟수', memo AS 메모
                    FROM tbCustomer";
            }
            else
            {
                sql = @"
                    SELECT name AS 이름, phone AS 연락처, registerDate AS 등록일, visitCount AS '방문 횟수', memo AS 메모
                    FROM tbCustomer
                    WHERE grade = @grade";
                param.Add("@grade", selectedGrade);
            }

            dgvCustomerList.DataSource = DBHelper.ExecuteDataTable(sql, param);
        }

        private void MenuCustomer_Load(object sender, EventArgs e)
        {
            cmbGradeFilter.SelectedIndex = 0;
            cmbGradeFilter_SelectedIndexChanged(sender, e);
        }

        private void dgvCustomerList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Step 1 : 메모 컬럼 인덱스인지 확인
            if (dgvCustomerList.Columns[e.ColumnIndex].Name == "메모")
            {
                DataGridViewRow row = dgvCustomerList.Rows[e.RowIndex];

                // Step 1-1 : id, memo 값 추출
                string customerName = row.Cells["이름"].Value?.ToString();
                string memo = row.Cells["메모"].Value?.ToString();

                // Step 1-2 : DB 반영
                string sql = @"
                            UPDATE tbCustomer
                            SET memo = @memo
                            WHERE name = @name";

                Dictionary<string, object> paramMemo = new Dictionary<string, object>();
                paramMemo.Add("@memo", memo);
                paramMemo.Add("@name", customerName);

                DBHelper.ExecuteNonQuery(sql, paramMemo);
            }

            // Step 2 : 연락처 컬럼 인덱스인지 확인
            if (dgvCustomerList.Columns[e.ColumnIndex].Name == "연락처")
            {
                DataGridViewRow row = dgvCustomerList.Rows[e.RowIndex];

                // Step 2-1 : id, memo 값 추출
                string customerName = row.Cells["이름"].Value?.ToString();
                string phone = row.Cells["연락처"].Value?.ToString();

                // Step 2-2 : DB 반영
                string sql = @"
                            UPDATE tbCustomer
                            SET phone = @phone
                            WHERE name = @name";

                Dictionary<string, object> paramPhone = new Dictionary<string, object>();
                paramPhone.Add("@phone", phone);
                paramPhone.Add("@name", customerName);

                DBHelper.ExecuteNonQuery(sql, paramPhone);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Step 1 : 현재 선택한 고객의 이름 구하기
            if (dgvCustomerList.SelectedRows.Count == 0) return;
            int rowIndex = dgvCustomerList.CurrentCell.RowIndex;
            int columnIndex = dgvCustomerList.CurrentCell.ColumnIndex;
            string selectedName = dgvCustomerList.Rows[rowIndex].Cells[columnIndex].Value.ToString();

            // Step 2 : 외래키 참조 제약조건에 의해 tbSale에 등록된 고객의 ID부터 제거를 위해 ID 서치
            string sql = @"
                        SELECT id
                        FROM tbCustomer
                        WHERE name = @name";
            Dictionary<string, object> paramName = new Dictionary<string, object>();
            paramName.Add("@name", selectedName);

            int customerId = (int)DBHelper.ExecuteScalar(sql, paramName);

            // Step 3 : tbSale에 등록된 고객의 id 제거
            sql = @"
                DELETE FROM tbSale
                WHERE customerId = @customerId";
            Dictionary<string, object> paramId = new Dictionary<string, object>();
            paramId.Add("@customerId", customerId);
            DBHelper.ExecuteNonQuery(sql, paramId);

            // Step 4 : tbCustomer에 등록된 고객 제거
            sql = @"
                DELETE FROM tbCustomer
                WHERE id = @customerId";
            DBHelper.ExecuteNonQuery(sql, paramId);

            // Step 5 : dgv에서도 제거
            dgvCustomerList.Rows.RemoveAt(rowIndex);
        }
    }
}
