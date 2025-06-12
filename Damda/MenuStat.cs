using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
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
            // 1. WPF PieChart 생성
            var pieChart = new LiveCharts.Wpf.PieChart
            {
                Width = guna2GroupBox1.Width - 20,
                Height = guna2GroupBox1.Height - 20,
                LegendLocation = LegendLocation.Right,
                Background = System.Windows.Media.Brushes.White,
                Series = new SeriesCollection
        {
            new PieSeries
            {
                Title = "카드",
                Values = new ChartValues<double> { 53 },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "계좌이체",
                Values = new ChartValues<double> { 35 },
                DataLabels = true
            },
            new PieSeries
            {
                Title = "현금",
                Values = new ChartValues<double> { 12 },
                DataLabels = true
            }
        }
            };

            // 2. ElementHost로 감싸기
            var elementHost = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = pieChart,
                BackColor = System.Drawing.Color.Transparent
            };

            // 3. Guna2GroupBox에 삽입
            guna2GroupBox1.Controls.Clear(); // 기존 내용 초기화 (선택사항)
            guna2GroupBox1.Controls.Add(elementHost);
        }
    }
}
