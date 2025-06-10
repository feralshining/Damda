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
    public partial class MenuHome : Form
    {
        public MenuHome()
        {
            InitializeComponent();
            SetUIFont();
            LoadTodaySummary();
        }
        private void SetUIFont()
        {
            menuLabel.Font = FontManager.CookieRun_22;

            label1.Font = FontManager.Kookmin_16;
            label3.Font = FontManager.Kookmin_16;
            label4.Font = FontManager.Kookmin_16;
            label5.Font = FontManager.Kookmin_16;
            label2.Font = FontManager.Kookmin_16;

            lblTodayTotal.Font = FontManager.CookieRun_20;
            lblTodayCount.Font = FontManager.CookieRun_16;
            lblTopPayment.Font = FontManager.CookieRun_16; 
            lblBestCustomer.Font = FontManager.CookieRun_16;

            dgvTodaySalesHistory.ColumnHeadersDefaultCellStyle.Font = FontManager.Kookmin_9;
            dgvTodaySalesHistory.DefaultCellStyle.Font = FontManager.Kookmin_9;
            dgvTodaySalesHistory.RowHeadersDefaultCellStyle.Font = FontManager.Kookmin_9;
        }
    private void LoadTodaySummary()
        {
            object result;
            string sql;

            // Step 1: 오늘 매출 총액 계산
            sql = @"
                SELECT SUM(price * quantity) 
                FROM tbSale 
                WHERE saleDate = CONVERT(date, GETDATE())";
            result = DBHelper.ExecuteScalar(sql);
            string total = (result == null || result == DBNull.Value) ? "0" : Convert.ToInt32(result).ToString("#,##0");
            lblTodayTotal.Text = "\\" + total;

            // Step 2: 오늘 판매 건수 계산
            sql = @"
                SELECT COUNT(*)
                FROM tbSale
                WHERE saleDate = CONVERT(date, GETDATE())";
            result = DBHelper.ExecuteScalar(sql);
            string count = (result == null || result == DBNull.Value) ? "0" : Convert.ToInt32(result).ToString();
            lblTodayCount.Text = count;

            // Step 3: 오늘 최다 결제 수단
            // Today 기준 결제 수단을 그룹화 => 결제 수단 별로 사용 횟수 체크 => 내림차순 정렬해서 제일 위에 존재하는 결제수단 반환
            sql = @"
                SELECT TOP 1 paymentMethod
                FROM tbSale
                WHERE saleDate = CONVERT(date, GETDATE())
                GROUP BY paymentMethod
                ORDER BY COUNT(*) DESC";
            result = DBHelper.ExecuteScalar(sql);
            string payment = (result == null || result == DBNull.Value) ? "없음" : result.ToString();
            lblTopPayment.Text = payment;

            // Step 4: 누적 매출 기준 단골 고객 조회
            sql = @"
                SELECT TOP 1 name
                FROM tbCustomer
                ORDER BY totalSpent DESC";
            result = DBHelper.ExecuteScalar(sql);
            string customer = (result == null || result == DBNull.Value) ? "없음" : result.ToString();
            lblBestCustomer.Text = customer;

            // Step 5: 오늘 매출 리스트 출력
            sql = @"
                SELECT 
                    s.saleDate AS '날짜', 
                    s.item AS '품목', 
                    (s.price * s.quantity) AS '금액', 
                    s.paymentMethod AS '결제방식', 
                    c.name AS '고객명'
                FROM tbSale s
                    LEFT JOIN tbCustomer c 
                        ON s.customerId = c.id
                WHERE s.saleDate = CONVERT(date, GETDATE())";
            dgvTodaySalesHistory.DataSource = DBHelper.ExecuteDataTable(sql);
        }
    }
}
