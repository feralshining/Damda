using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            LoadCircleChart();          
        }
        private void LoadCircleChart()
        {
            // 1. WPF PieChart 생성
            LiveCharts.WinForms.PieChart circleChart = pieChart1;
            {
                circleChart.Series = new SeriesCollection
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
                    },
                    new PieSeries
                    {
                        Title = "기타",
                        Values = new ChartValues<double> { 12 },
                        DataLabels = true
                    }
                };
            };

            guna2GroupBox1.Controls.Clear();
            guna2GroupBox1.Controls.Add(circleChart);
        }
    }
}
