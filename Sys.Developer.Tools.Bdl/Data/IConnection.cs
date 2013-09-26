using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Bdl.Data
{
    public interface IConnection
    {
        bool Connect(string connectionString);

        void Disconnect();
    }
}
