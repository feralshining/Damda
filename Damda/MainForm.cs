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
            SqlInitialize();
            InsertSQL();
        }
        private void SqlInitialize()
        {
            string conStr = "Data Source=(LocalDB)\\mylocaldb;AttachDbFilename=C:\\CSharp_SQL\\myDB.mdf;Integrated Security=True;"; //DB경로 연결
            con = new SqlConnection(conStr);
            sql = "SELECT * FROM tbMenu";
            cmd = new SqlCommand(sql, con);
        }

        private void InsertSQL()
        {
            string insertCustomerQuery = @"
            INSERT INTO tbCustomer (name, phone, registerDate, visitCount, totalSpent, grade, memo) VALUES
            (N'김다정', '010-1234-5678', '2024-12-10', 3, 150000, N'실버', N'아메리카노 단골'),
            (N'이지훈', '010-2345-6789', '2024-11-01', 5, 520000, N'골드', N'스탬프 쿠폰 사용 중'),
            (N'박세영', '010-3456-7890', '2024-10-18', 1, 12000, N'일반', N'처음 방문한 손님'),
            (N'최유나', '010-1111-2222', '2024-12-20', 2, 78000, N'일반', N'쿠키 자주 구매'),
            (N'한정민', '010-9999-8888', '2025-01-02', 8, 990000, N'플래티넘', N'VIP, 생일쿠폰 발급 대상');
            ";
            string insertSaleQuery = @"
            INSERT INTO tbSale (customerId, saleDate, item, price, quantity, totalPrice, paymentMethod, memo) VALUES
            (1, '2025-01-01', N'아메리카노', 5000, 1, 5000, N'카드', N'오전 9시 방문'),
            (2, '2025-01-02', N'샌드위치 세트', 12000, 2, 24000, N'현금', N'테이크아웃'),
            (2, '2025-01-04', N'케이크', 28000, 1, 28000, N'카드', N'친구 생일'),
            (4, '2025-01-03', N'쿠키 3종세트', 9000, 1, 9000, N'계좌이체', NULL),
            (NULL, '2025-01-03', N'비회원 결제', 8500, 1, 8500, N'현금', N'영수증 요청 안함'),
            (5, '2025-01-05', N'원두 200g', 25000, 2, 50000, N'카드', N'정기배송 신청자');
            ";

            if (con.State == ConnectionState.Closed) con.Close();
            if (con.State != ConnectionState.Open) con.Open();

            cmd = new SqlCommand(insertCustomerQuery, con);
            cmd.ExecuteNonQuery();

            cmd = new SqlCommand(insertSaleQuery, con);
            cmd.ExecuteNonQuery();  


            //sql = @"INSERT INTO tbMenu (id, name, price)
            //        VALUES (@id, @name, @price)";
            //if (con.State != ConnectionState.Open) con.Open();
            //using (cmd = new SqlCommand(sql, con))
            //{
            //    cmd.Parameters.AddWithValue("@id", value1);
            //    cmd.Parameters.AddWithValue("@name", value2);
            //    cmd.Parameters.AddWithValue("@price", value3);
            //}
            //try
            //{
            //    cmd.ExecuteNonQuery();
            //}
            //catch (SqlException ex)
            //{
            //    if (ex.Number == 2027 || ex.Number == 2601)
            //        MessageBox.Show(ex.Message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

            //MessageBox.Show("정상적으로 등록했습니다", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
