using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLib.Attributes
{
  public class CSVColumnName : Attribute
  {
    public readonly string CsvColumName; 

    public CSVColumnName(string csvColumnName)
    {
      this.CsvColumName = csvColumnName;
    }
  }
}
