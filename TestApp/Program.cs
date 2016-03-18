using CSVLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
  class Program
  {
    static void Main(string[] args)
    {
      List<Person> personList = Person.GetPersonList();
      byte[] personCsvContent = null;
      Stopwatch s = new Stopwatch();
      s.Start();
      using (CSVWriter csvWriter = new CSVWriter())
      {
        csvWriter.PopulateCSV(true, personList);
        personCsvContent = csvWriter.Save();
      }
      s.Stop();
     
      File.WriteAllBytes(@"C:\Users\chamirb\Desktop\TEMP\TestCsv.csv", personCsvContent);
      Console.WriteLine("Time: {0}", s.ElapsedMilliseconds);
      Console.Read();
    }
  }
}
