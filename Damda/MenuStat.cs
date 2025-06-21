using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;


namespace Damda
{
    public partial class MenuStat : Form
    {
        public MenuStat()
        {
            InitializeComponent();
            SetUIFont();
            LoadCircleChart();
            LoadTopCustomerBarChart();
            LoadDailySalesChart();
        }
        private void SetUIFont()
        {
            label1.Font = FontManager.Kookmin_12;
            label2.Font = FontManager.Kookmin_12;
        }
        private Dictionary<string, int> LoadPaymentDataFromDB()
        {
            string sql = @"
                        SELECT paymentMethod, COUNT(*) AS count
                        FROM tbSale
                        GROUP BY paymentMethod";

            DataTable dt = DBHelper.ExecuteDataTable(sql);
            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (DataRow row in dt.Rows)
            {
                string method = row["paymentMethod"].ToString();
                int count = Convert.ToInt32(row["count"]);
                result[method] = count;
            }

            return result;
        }
        private Dictionary<string, int> LoadSelectedCustomerSales(List<string> selectedNames)
        {
            if (selectedNames == null || selectedNames.Count == 0) return new Dictionary<string, int>();

            var paramNames = new List<string>();
            for (int i = 0; i < selectedNames.Count; i++)
            {
                paramNames.Add($"@name{i}");
            }
            string nameClause = string.Join(",", paramNames); // => @name0,@name1,@name2

            string sql = $@"
                        SELECT name, totalSpent
                        FROM tbCustomer
                        WHERE name IN ({nameClause})
                        ORDER BY totalSpent DESC";

            var param = new Dictionary<string, object>();
            for (int i = 0; i < selectedNames.Count; i++)
            {
                param.Add($"@name{i}", selectedNames[i]);
            }

            var result = new Dictionary<string, int>();
            DataTable dt = DBHelper.ExecuteDataTable(sql, param);
            foreach (DataRow row in dt.Rows)
            {
                string name = row["name"].ToString();
                int total = Convert.ToInt32(row["totalSpent"]);
                result[name] = total;
            }
            return result;
        }


        private void LoadCircleChart()
        {
            Dictionary<string, int> paymentCounts = LoadPaymentDataFromDB();

            pieChart1.Series = new SeriesCollection();
            foreach (KeyValuePair<string, int> kvp in paymentCounts)
            {
                PieSeries series = new PieSeries
                {
                    Title = kvp.Key, //범례
                    Values = new ChartValues<double> { kvp.Value }, //차트 값
                    LabelPoint = label => $"{kvp.Key}\n({label.Y}건)",
                    Foreground = new SolidColorBrush(System.Windows.Media.Colors.Black),
                    DataLabels = true //퍼센트 값 표시 여부
                };
                switch (kvp.Key)
                {
                    case "카드":
                        series.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 193, 7)); // 노랑
                        break;
                    case "현금":
                        series.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 152, 0)); // 주황
                        break;
                    case "계좌이체":
                        series.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 181, 246)); // 하늘
                        break;
                    default:
                        series.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(158, 158, 158)); // 회색
                        break;
                }
                pieChart1.Series.Add(series);
            }
            guna2GroupBox2.Controls.Add(pieChart1);
        }

        private void LoadTopCustomerBarChart()
        {
            List<string> selectedNames = CustomerSelection.SelectedCustomerNames;
            Dictionary<string, int> data = LoadSelectedCustomerSales(selectedNames);

            cartesianChart1.AxisY.Clear();
            cartesianChart1.AxisX.Clear();


            cartesianChart1.Series = new SeriesCollection
            {
                new RowSeries
                {
                    //Title = "총 매출",
                    Values = new ChartValues<int>(data.Values), //소비 금액
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 193, 7)),
                    DataLabels = true,
                    LabelPoint = point => point.X.ToString("##,##0") + "원"
                }
            };

            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "매출",
                LabelFormatter = value => value.ToString("##,##0") + "원",
            });

            cartesianChart1.AxisY.Add(new Axis
            {
                Labels = data.Keys.ToList(),
                //Separator = new Separator { Step = 1 }
            });

            guna2GroupBox3.Controls.Add(cartesianChart1);
        }

        private void LoadDailySalesChart()
        {
            string sql = @"
                        SELECT saleDate, SUM(totalPrice) AS total
                        FROM tbSale
                        GROUP BY saleDate
                        ORDER BY saleDate
                    ";

            DataTable dt = DBHelper.ExecuteDataTable(sql);
            ChartValues<double> sales = new ChartValues<double>();
            List<string> days = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                DateTime date = Convert.ToDateTime(row["saleDate"]);
                int total = Convert.ToInt32(row["total"]);

                days.Add(date.ToString("MM-dd")); // or "dd" if day only
                sales.Add(total);
            }

            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "일일 매출",
                    Values = sales,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    LineSmoothness = 0.3
                }
            };

            cartesianChart2.AxisX.Add(new Axis
            {
                Title = "날짜",
                Labels = days
            });

            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "총 매출",
                LabelFormatter = value => value.ToString("##,##0") + "원",
                MinValue = 0
            });

            guna2GroupBox4.Controls.Add(cartesianChart2); // <- 적절한 컨테이너에 추가
        }

    }
}
