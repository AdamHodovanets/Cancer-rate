using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Wpf;
using LiveCharts;
using System.Xml.Linq;

namespace DGTask
{
    public partial class MainWindow : Window
    {
        private IEnumerable<XElement> result { get; set; }
        private Axis ax { get; set; }
        private ColumnSeries col { get; set; }
        private XDocument dx { get; set; }
        private IEnumerable<string> selectedValues { get; set; }
        private List<string> banned { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            col = new ColumnSeries() { DataLabels = true, Values = new ChartValues<double>(), LabelPoint = point => point.Y.ToString() };
            ax = new Axis() { Separator = new LiveCharts.Wpf.Separator() { Step = 1, IsEnabled = false } };
            ax.Labels = new List<string>();
            dx = XDocument.Load(@"http://91.232.241.66:92/api/v1/study/89b4effb-40f8-44d9-ba25-c9cbe7602a95/measurements/tasks/3ba2733b-6274-4bee-93ac-6af4eab90e72/?authorization=Basic%20dGVzdF92YWNhbnRpb246dGVzdF92YWNhbnRpb24=");
            result = dx.Descendants(XName.Get("sessiondate"));
            selectedValues = null;
            banned = new List<string>();


            ChartInfo(dx, result, selectedValues, col, ax);

            for (int i = 1; i < 32; i++)
            {
                if (i < 10) { comboBox1.Items.Add("0" + i.ToString()); }
                else { comboBox1.Items.Add(i.ToString()); }
            }
            for (int i = 1; i < 13; i++)
            {
                if (i < 10) { comboBox.Items.Add("0" + i.ToString()); }
                else { comboBox.Items.Add(i.ToString()); }

            }





        }

        private void ChartInfo(XDocument dx, IEnumerable<XElement> result,
            IEnumerable<string> selectedValues, ColumnSeries col, Axis ax)
        {
            col.Values.Clear();
            ax.Labels.Clear();
            selectedValues = null;

            List<double> ar = new List<double>();
            for (int i = 0; i < result.Count(); i++)
            {
                ar.Clear();
                selectedValues =
                        dx.Descendants("session")
                        .Where(a => a.Element("sessiondate").Value != result.ElementAt(i).Value)
                            .SelectMany(a => a.Descendants("value")).Select(c => c.Value);
                for (int j = 0; j < selectedValues.Count(); j++)
                {
                    ar.Add(Convert.ToDouble(selectedValues.ElementAt(j).Replace('.', ',')));
                }
                if (!banned.Contains(result.ElementAt(i).Value.Remove(5)))
                {
                    col.Values.Add(Math.Round(ar.Average(), 2));
                    ax.Labels.Add(result.ElementAt(i).Value.Remove(5));
                }
            }


            cancerChart.Series.Add(col);
            cancerChart.AxisX.Add(ax);
            cancerChart.AxisY.Add(new Axis
            {
                LabelFormatter = value => value.ToString(),
                Separator = new LiveCharts.Wpf.Separator()
            });
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            restartChart();
            banned.Clear();
            ChartInfo(dx, result, selectedValues, col, ax);
        }
        private void restartChart()
        {
            cancerChart.Series.Clear();
            cancerChart.AxisX.Clear();
            cancerChart.AxisY.Clear();
            col = new ColumnSeries() { DataLabels = true, Values = new ChartValues<double>(), LabelPoint = point => point.Y.ToString() };
            ax = new Axis() { Separator = new LiveCharts.Wpf.Separator() { Step = 1, IsEnabled = false } };
            ax.Labels = new List<string>();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!ax.Labels.Contains(comboBox.SelectedValue + @"/" + comboBox1.SelectedValue)) { return; }
            banned.Add(comboBox.SelectedValue + @"/" + comboBox1.SelectedValue);
            restartChart();
            ChartInfo(dx, result, selectedValues, col, ax);
            listBox.Items.Add(comboBox.SelectedValue + @"/" + comboBox1.SelectedValue);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(!banned.Contains(comboBox.SelectedValue + @"/" + comboBox1.SelectedValue)) { return; }
            banned.Remove(comboBox.SelectedValue + @"/" + comboBox1.SelectedValue);
            restartChart();
            ChartInfo(dx, result, selectedValues, col, ax);
            listBox.Items.Remove(comboBox.SelectedValue + @"/" + comboBox1.SelectedValue);
        }
    }
}

