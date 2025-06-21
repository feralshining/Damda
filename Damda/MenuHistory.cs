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
            InitFilter();
        }
        private void SetUIFont()
        {
            menuLabel.Font = FontManager.CookieRun_22;
            guna2GroupBox1.Font = FontManager.Kookmin_9;
            dgvTransaction.ColumnHeadersDefaultCellStyle.Font = FontManager.Kookmin_9;
            dgvTransaction.DefaultCellStyle.Font = FontManager.Kookmin_9;
            dgvTransaction.ThemeStyle.HeaderStyle.Font = FontManager.Kookmin_9;
            dgvTransaction.ThemeStyle.RowsStyle.Font = FontManager.Kookmin_9;
            cmbDateRange.Font = FontManager.CookieRun_14;
            cmbPayment.Font = FontManager.CookieRun_14;
            txtSearch.Font = FontManager.CookieRun_14;
        }

        private void InitFilter()
        {
            cmbDateRange.Items.AddRange(new string[] { "전체", "1주일", "1개월" });
            cmbPayment.Items.AddRange(new string[] { "전체", "카드", "현금", "계좌이체" });
            cmbDateRange.SelectedIndex = 0;
            cmbPayment.SelectedIndex = 0;
        }

        private void LoadTransactionData()
        {
            string keyword = txtSearch.Text.Trim();
            string payment = cmbPayment.SelectedItem.ToString();
            string dateRange = cmbDateRange.SelectedItem.ToString();

            // Step 1: 기본 SELECT 쿼리 구성
            string sql = @"
                        SELECT 
                            FORMAT(s.saleDate, 'yy.MM.dd') AS 날짜,                                          -- 날짜 형식
                            s.item AS 품목,                                                                  -- 판매 품목
                            FORMAT(s.price * s.quantity, '#,##0') AS 금액,                                   -- 금액 계산
                            s.paymentMethod AS 결제방식,                                                     -- 결제 방식
                            ISNULL(c.name, N'비회원') AS 고객                                                -- 고객명, 없으면 비회원 표시
                        FROM tbSale s
                        LEFT JOIN tbCustomer c ON s.customerId = c.id
                        WHERE 1 = 1"; // 동적 WHERE 절 생성을 위한 기본조건

            Dictionary<string, object> param = new Dictionary<string, object>();

            // Step 2: 검색어 필터 - 품목 또는 고객명에 키워드가 포함된 경우
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += @"
                        AND (
                            s.item LIKE @keyword 
                            OR ISNULL(c.name, '') LIKE @keyword
                        )";
                param.Add("@keyword", "%" + keyword + "%");
            }

            // Step 3: 결제 방식 필터
            if (payment != "전체")
            {
                sql += " AND s.paymentMethod = @payment";
                param.Add("@payment", payment);
            }

            // Step 4: 날짜 필터 - 최근 1주일 또는 1개월
            if (dateRange == "1주일") sql += " AND s.saleDate >= DATEADD(DAY, -7, GETDATE())";
            else if (dateRange == "1개월") sql += " AND s.saleDate >= DATEADD(MONTH, -1, GETDATE())";

            // Step 5: 최신 거래일순 정렬
            sql += " ORDER BY s.saleDate DESC";

            // Step 6: 결과 바인딩
            dgvTransaction.DataSource = DBHelper.ExecuteDataTable(sql, param);
        }


        private void MenuHistory_Load(object sender, EventArgs e) => LoadTransactionData();

        private void txtSearch_TextChanged(object sender, EventArgs e) => LoadTransactionData();

        private void cmbDateRange_SelectedIndexChanged(object sender, EventArgs e) => LoadTransactionData();

        private void cmbPayment_SelectedIndexChanged(object sender, EventArgs e) => LoadTransactionData();

        private void dgvTransaction_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvTransaction.Columns[e.ColumnIndex].Name == "금액" && e.Value != null)
            {
                // 금액 칼럼에 ₩ 접두어 붙이기
                e.Value = "₩" + e.Value.ToString();
                e.FormattingApplied = true;
            }
        }
    }
}
