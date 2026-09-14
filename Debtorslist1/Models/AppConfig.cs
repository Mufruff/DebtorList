using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Debtorslist1.Models;

public class AppConfig
{
    public ConnectionStringsConfig ConnectionStrings { get; set; } = new();
}

public class ConnectionStringsConfig
{
    public string MariaDB { get; set; } = "";
}
