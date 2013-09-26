using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Sys.Developer.Tools.Fcd;

namespace Sys.Developer.Tools.Wpf.ViewModel
{
    public class TableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> children;
        private readonly DatabaseViewModel parent;
        private readonly TableDto table;

        private bool _isExpanded;
        private bool _isSelected;


        public TableViewModel(TableDto tab)
            : this(tab, null)
        {

        }

        public TableViewModel(TableDto tab, DatabaseViewModel parentParam)
        {
            table = tab;
            parent = parentParam;
        }

        public ObservableCollection<string> Children
        {
            get { return children; }
        }

        public string Name
        {
            get { return table.TableName; }
        }

        public DatabaseViewModel Parent
        {
            get { return parent; }
        }

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && parent != null)
                    parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
