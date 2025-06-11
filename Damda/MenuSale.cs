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
    public partial class MenuSale : Form
    {
        public MenuSale()
        {
            InitializeComponent();
            SetUIFont();
        }
        private void SetUIFont()
        {
            // Labels
            menuLabel.Font = FontManager.CookieRun_22; // CookieRun Regular, 22.2F
            label1.Font = FontManager.Kookmin_16; // 국민연금체 Regular, 15.75F
            label4.Font = FontManager.Kookmin_16; // 국민연금체 Regular, 15.75F
            label5.Font = FontManager.Kookmin_16; // 국민연금체 Regular, 15.75F
            lblTotalPrice.Font = FontManager.Kookmin_12; // 국민연금체 Regular, 12F

            // Buttons
            btnRegister.Font = FontManager.CookieRun_12; // CookieRun Regular, 12F
            btnItemAdd.Font = FontManager.CookieRun_12; // CookieRun Regular, 12F

            // DataGridView
            dgvSalesHistory.ColumnHeadersDefaultCellStyle.Font = FontManager.Kookmin_11; // 국민연금체 Regular, 11F
            dgvSalesHistory.DefaultCellStyle.Font = FontManager.Kookmin_11; // 국민연금체 Regular, 11F
            dgvSalesHistory.RowHeadersDefaultCellStyle.Font = FontManager.Kookmin_11; // 국민연금체 Regular, 11F
            dgvSalesHistory.ThemeStyle.HeaderStyle.Font = FontManager.Kookmin_11; // 국민연금체 Regular, 11F
            dgvSalesHistory.ThemeStyle.RowsStyle.Font = FontManager.Kookmin_9; // 국민연금체 Regular, 9F

            // Other controls
            dtpSaleDate.Font = FontManager.Kookmin_9_75; // 국민연금체 Regular, 9.75F
            txtCustomerName.Font = FontManager.Kookmin_9; // 국민연금체 Regular, 9F
            cmbPaymentMethod.Font = FontManager.Kookmin_10; // 국민연금체 Regular, 10F
            guna2GroupBox1.Font = FontManager.Kookmin_9; // 국민연금체 Regular, 9F
            guna2GroupBox2.Font = FontManager.Kookmin_9; // 국민연금체 Regular, 9F
        }

        private void RefreshCustomerGrade(int customerId)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            Dictionary<string, object> paramUpdate = new Dictionary<string, object>();

            // Step 1 : 설정된 기준값 읽기 (실버, 골드)
            int silverLimit = MenuSettings.GradeSilver;
            int goldLimit = MenuSettings.GradeGold;

            // Step 2 : 고객의 totalSpent 가져오기
            string sql = @"
                        SELECT totalSpent 
                        FROM tbCustomer
                        WHERE id = @id";
            
            param.Add("@id", customerId);
            object result = DBHelper.ExecuteScalar(sql, param);
            int totalSpent = Convert.ToInt32(result);

            // Step 3 : 등급 판단
            string newGrade = "일반";
            if (totalSpent >= goldLimit) newGrade = "골드";
            else if (totalSpent >= silverLimit) newGrade = "실버";

            // Step 4 :등급 업데이트
            sql = @"
                UPDATE tbCustomer 
                SET grade = @grade 
                WHERE id = @id";
            paramUpdate.Add("@grade", newGrade);
            paramUpdate.Add("@id", customerId);
            DBHelper.ExecuteNonQuery(sql, paramUpdate);
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            object result;
            string sql;

            // Step 1 : 입력값 전처리
            int customerId;
            DateTime saleDate = dtpSaleDate.Value.Date;
            string customerName = txtCustomerName.Text.Trim();
            string paymentMethod = cmbPaymentMethod.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrEmpty(customerName)) customerName = "비회원";

            if (dgvSalesHistory.Rows.Count == 0 || dgvSalesHistory.Rows.Cast<DataGridViewRow>().All(r => r.IsNewRow))
            {
                MessageBox.Show("하나 이상의 품목을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Step 2 : 고객 ID 조회 및 신규 등록
            sql = @"
                SELECT id 
                FROM tbCustomer 
                WHERE name = @name";

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("@name", customerName);
            result = DBHelper.ExecuteScalar(sql, param);


            if ((result == null) || (result == DBNull.Value)) //기존 고객이 아닌 경우
            {
                // 신규 고객 등록
                DateTime registerDate = DateTime.Now.Date;
                sql = @"
                    INSERT INTO tbCustomer 
                        (name, registerDate, visitCount, totalSpent, grade, memo) 
                    VALUES 
                        (@name, @registerDate, 1, 0, '일반', '')";
                Dictionary<string, object> paramInsert = new Dictionary<string, object>();
                paramInsert.Add("@name", customerName);
                paramInsert.Add("@registerDate", registerDate);
                DBHelper.ExecuteNonQuery(sql, paramInsert);

                // INSERT 직후 SELECT로 ID 재조회 (name + registerDate => 동명이인 방지)
                sql = @"
                    SELECT id 
                    FROM tbCustomer 
                    WHERE name = @name AND registerDate = @registerDate";
                Dictionary<string, object> paramSelect = new Dictionary<string, object>();
                paramSelect.Add("@name", customerName);
                paramSelect.Add("@registerDate", registerDate);
                result = DBHelper.ExecuteScalar(sql, paramSelect);

                if (result == null)
                {
                    MessageBox.Show("고객 등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            customerId = Convert.ToInt32(result);

            // Step 3 : 품목별 매출 등록
            int insertCountCheck = 0;
            int totalSpentToAdd = 0;
            foreach (DataGridViewRow row in dgvSalesHistory.Rows)
            {
                if (row.IsNewRow) continue; //마지막 줄(새 행 추가용) 무시

                string item = row.Cells["colItem"].Value?.ToString(); //품목 저장
                if (!int.TryParse(row.Cells["colQuantity"].Value?.ToString(), out int quantity)) continue; //수량 저장
                if (!int.TryParse(row.Cells["colPrice"].Value?.ToString(), out int price)) continue; //단가 저장

                sql = @"
                    INSERT INTO tbSale 
                        (customerId, saleDate, item, price, quantity, paymentMethod, memo) 
                    VALUES 
                        (@customerId, @saleDate, @item, @price, @quantity, @paymentMethod, '')";
                
                Dictionary<string, object> paramInsert = new Dictionary<string, object>();
                paramInsert.Add("@customerId", customerId);
                paramInsert.Add("@saleDate", saleDate);
                paramInsert.Add("@item", item);
                paramInsert.Add("@price", price);
                paramInsert.Add("@quantity", quantity);
                paramInsert.Add("@paymentMethod", paymentMethod);

                using (SqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    foreach (var p in paramInsert)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                    insertCountCheck += cmd.ExecuteNonQuery();
                    totalSpentToAdd += price * quantity;
                }
            }

            // Step 4: 누적 금액 및 방문 수 반영
            if (insertCountCheck > 0)
            {
                // 업데이트 => 고객 누적 금액 + 방문 수
                sql = @"
                    UPDATE tbCustomer
                    SET 
                        totalSpent = totalSpent + @plus,
                        visitCount = visitCount + 1
                    WHERE id = @id";

                Dictionary<string, object> paramSpent = new Dictionary<string, object>();
                paramSpent.Add("@plus", totalSpentToAdd);
                paramSpent.Add("@id", customerId);
                DBHelper.ExecuteNonQuery(sql, paramSpent);

            }

            // Step 5 : 완료 처리
            if (insertCountCheck > 0)
            {
                MessageBox.Show("매출이 성공적으로 등록되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshCustomerGrade(customerId);
                dgvSalesHistory.Rows.Clear();
                lblTotalPrice.Text = "0";
            }
            else
            {
                MessageBox.Show("등록된 매출이 없습니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvSalesHistory_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Step 1 : 그리드뷰 총액 자동 계산
            if (e.ColumnIndex == dgvSalesHistory.Columns["colQuantity"].Index || e.ColumnIndex == dgvSalesHistory.Columns["colPrice"].Index) //수량 또는 단가 수정 시
            {
                DataGridViewRow row = dgvSalesHistory.Rows[e.RowIndex]; //현재 수정한 셀
                if (row.IsNewRow) return; //마지막 줄(새 행 추가용) 무시

                bool validQuantity = int.TryParse(row.Cells["colQuantity"].Value?.ToString(), out int quantity);
                bool validPrice = int.TryParse(row.Cells["colPrice"].Value?.ToString(), out int price);

                if (validQuantity && validPrice) row.Cells["colTotalPrice"].Value = (quantity * price).ToString();
                else row.Cells["colTotalPrice"].Value = "0";
            }

            // Step 2 : 전체 합계 갱신
            int totalSum = 0;
            foreach (DataGridViewRow row in dgvSalesHistory.Rows)
            {
                if (row.IsNewRow) continue; //마지막 줄(새 행 추가용) 무시
                if (int.TryParse(row.Cells["colTotalPrice"].Value?.ToString(), out int rowTotal)) totalSum += rowTotal;
            }
            lblTotalPrice.Text = totalSum.ToString("#,##0") + "원";
        }

        private void btnItemAdd_Click(object sender, EventArgs e)
        {
            dgvSalesHistory.Rows.Add(); // 새 행 추가
        }

        private void MenuSale_Load(object sender, EventArgs e)
        {
            dtpSaleDate.Value = DateTime.Today;
        }
    }
}
