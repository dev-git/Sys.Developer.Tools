using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sys.Developer.Tools.Wpf
{
    /// <summary>
    /// Interaction logic for TableData.xaml
    /// </summary>
    public partial class TableData : Window
    {
        private Fcd.FacadeService FacadeService;
        private string ServerName = String.Empty;
        private string DbName = String.Empty;
        private string TableName = String.Empty;
        private ObservableCollection<Record> records = new ObservableCollection<Record>();
        IList<object> td = new List<object>();
 
        public TableData()
        {
            InitializeComponent();
        }

        public TableData(Fcd.FacadeService fcdService)
        {
            // TODO: Complete member initialization
            FacadeService = fcdService;
        }

        public TableData(Fcd.FacadeService fcdService, string serverName, string dbName, string tableName, string filter, bool lookForJoin)
        {
            InitializeComponent();

            Title = String.Format("{0}.{1}.{2}", serverName, dbName, tableName);

            records.Clear();
            td.Clear();
            dgData.Items.Clear();
            //dgTableData.AutoGenerateColumns = false;

            // TODO: Complete member initialization
            FacadeService = fcdService;
            ServerName = serverName;
            DbName = dbName;
            TableName = tableName;

            //facadeService.GetFieldList(serverName, dbName, tableName);
            IList<string> fieldList = FacadeService.GetFieldList(serverName, dbName, tableName);
            td = FacadeService.GetTableData(serverName, dbName, tableName, fieldList, filter, lookForJoin);


            // http://paulstovell.com/blog/dynamic-datagrid
            if (td.Count > 0)
            {
                //string[,] dataArr = new string[td.Count, ((Array)td[0]).Length];
                
                IList<string[]> dataList = new List<string[]>();
                for (int xx = 0; xx < td.Count; xx++)
                {
                    string[] dataArr = new string[((Array)td[0]).Length];
                    Property[] propList = new Property[((Array)td[0]).Length];
                    for (int yy = 0; yy < ((Array)td[xx]).Length; yy++)
                    {
                        //string colName = String.Format("col{0}", yy);
                        string colName = fieldList[yy];
                        string rowVal = ((Array)td[xx]).GetValue(yy).ToString();
                        Property prop = new Property(colName, rowVal);
                        propList.SetValue(prop, yy);
                        
                    }
                    records.Add(new Record(propList));
                }

                var columns = records.First().Properties.Select((x, i) => new { Name = x.Name, Index = i }).ToArray();

                foreach (var column in columns)
                {
                    var binding = new Binding(string.Format("Properties[{0}]", column.Index));
                    dgData.Columns.Add(new CustomBoundColumn()
                    {
                        Header = column.Name,
                        Binding = binding,
                        TemplateName = "CustomTemplate"
                    });
                }

                dgData.ItemsSource = records;
             }
        }

        private void dgData_CurrentCellChanged(object sender, EventArgs e)
        {
            //DataGridCell cell = sender as DataGridCell;
            
            if (dgData.SelectedItem != null)
            {
                try
                {
                    int colIndex = dgData.CurrentColumn.DisplayIndex != 0 ? dgData.CurrentColumn.DisplayIndex - 1 : dgData.Columns.Count - 1;

                    string oldVal = ((object[])(td[dgData.SelectedIndex]))[colIndex].ToString();
                    string newVal = ((Record)dgData.SelectedItem).Properties[colIndex].Value.ToString();

                    if (oldVal != newVal)
                    {
                        int updateSuccess = FacadeService.UpdateRecord(ServerName, DbName, TableName, dgData.Columns[colIndex].Header.ToString(), newVal,
                            ((Record)dgData.SelectedItem).Properties[0].Name, ((Record)dgData.SelectedItem).Properties[0].Value.ToString());

                        if (updateSuccess >= 0)
                        {
                            DataGridRow row = (DataGridRow)dgData.ItemContainerGenerator.ContainerFromItem(dgData.SelectedItem);
                            
                            row.Background = Brushes.DarkRed;
                        }
                        /*MessageBox.Show(String.Format("update {0} set {1} = {2} where {3} = {4}", TableName, dgData.Columns[dgData.CurrentColumn.DisplayIndex - 1].Header.ToString(),
                           ((Record)dgData.SelectedItem).Properties[dgData.CurrentColumn.DisplayIndex - 1].Value,
                           ((Record)dgData.SelectedItem).Properties[0].Name, ((Record)dgData.SelectedItem).Properties[0].Value));*/
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dgData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (records.Count < 45)
            {
                // Resize the form
                this.Height = (records.Count * 25) + 50;

            }

            if (dgData.Columns.Count < 5)
            {
                // Todo: better smarts here
                this.Width = dgData.Columns.Sum(c => c.ActualWidth) + 50;
            }
            else
            {
                dgData.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            records.Clear();
            td.Clear();
        }
    }

    public class CustomBoundColumn : DataGridBoundColumn
    {
        public string TemplateName { get; set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var binding = new Binding(((Binding)Binding).Path.Path);
            binding.Source = dataItem;

            var content = new ContentControl();
            content.ContentTemplate = (DataTemplate)cell.FindResource(TemplateName);
            content.SetBinding(ContentControl.ContentProperty, binding);
            return content;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return GenerateElement(cell, dataItem);
        }
    }

    public class Record
    {
        private readonly ObservableCollection<Property> properties = new ObservableCollection<Property>();

        public Record(params Property[] properties)
        {
            foreach (var property in properties)
                Properties.Add(property);
        }

        public ObservableCollection<Property> Properties
        {
            get { return properties; }
        }
    }

    public class Property : INotifyPropertyChanged
    {
        public Property(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
