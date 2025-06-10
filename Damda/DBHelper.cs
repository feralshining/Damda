using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damda
{
    public class DBHelper
    {
        //DB 경로 지정 (데스크탑용)
        public static string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\s4chd\source\repos\Damda-main\Damda\DB\DamdaFinalDB.mdf;Integrated Security=True";
        //DB 경로 지정 (노트북용)
        //public static string connStr = @"
        //                                Data Source=desktop-h9f8umg\mssqlserver01;
        //                                Initial Catalog=DamdaFinalDB;
        //                                Integrated Security=True";


        /// <summary>
        /// 지정된 연결 문자열을 사용하여 SqlConnection 객체를 생성합니다.
        /// </summary>
        public static SqlConnection GetConnection() => new SqlConnection(connStr);

        /// <summary>
        /// SQL 쿼리를 실행하고, 영향을 받은 행 수를 반환합니다.
        /// 주로 INSERT, UPDATE, DELETE 명령일 때 사용합니다.
        /// </summary>
        /// <param name="sql">실행할 SQL 문자열</param>
        /// <param name="param">쿼리에 사용할 매개변수(옵션)</param>
        /// <returns>실행된 쿼리로 인해 영향을 받은 행 수(int)</returns>
        public static int ExecuteNonQuery(string sql, Dictionary<string, object> param = null)
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                if (param != null)
                {
                    foreach (var p in param)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                return cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// SQL 쿼리를 실행하고 단일 값을 반환합니다.
        /// 주로 COUNT, SUM, MAX 등의 단일 결과를 얻을 때 사용합니다.
        /// </summary>
        /// <param name="sql">실행할 SQL 문자열</param>
        /// <param name="param">쿼리에 사용할 매개변수(옵션)</param>
        /// <returns>(object)첫 번째 행의 첫 번째 열 값</returns>
        public static object ExecuteScalar(string sql, Dictionary<string, object> param = null)
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                if (param != null)
                {
                    foreach (var p in param)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// SQL SELECT 쿼리를 실행하고 결과를 DataTable로 반환합니다.
        /// 복수 레코드 조회로 dataGridView 같이 UI 바인딩에 쓰입니다.
        /// </summary>
        /// <param name="sql">실행할 SQL 문자열</param>
        /// <param name="param">쿼리에 사용할 매개변수(옵션)</param>
        /// <returns>조회된 결과가 담긴 DataTable 객체</returns>
        public static DataTable ExecuteDataTable(string sql, Dictionary<string, object> param = null)
        {
            using (SqlConnection con = GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                if (param != null)
                {
                    foreach (var p in param)
                        cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd); //DataSet을 메모리에 캐시로 저장하고 꺼낼 수 있는 클래스.
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

}
