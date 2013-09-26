using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using Sys.Developer.Tools.Fcd;

namespace Sys.Developer.Tools.Wpf.ViewModel
{
    
    public class SqlServerTreeViewModel
    {
        readonly ObservableCollection<SqlServerViewModel> _firstGeneration;
        readonly SqlServerViewModel _rootPerson;

        public SqlServerTreeViewModel(SqlServerInstanceDto rootPerson)
        {
            _rootPerson = new SqlServerViewModel(rootPerson);

            _firstGeneration = new ObservableCollection<SqlServerViewModel>(
                new SqlServerViewModel[] 
                { 
                    _rootPerson 
                });
        }


        #region FirstGeneration

        /// <summary>
        /// Returns a read-only collection containing the first person 
        /// in the family tree, to which the TreeView can bind.
        /// </summary>
        public ObservableCollection<SqlServerViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        #endregion // FirstGeneration
    }
}
