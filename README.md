# SharpCSV
C# library for creating CSV files

Snippets of code have been borrowed from Harpo on the following StackOverflow page:
http://stackoverflow.com/questions/4685705/good-csv-writer-for-c

Usage:

Given the following Person class:

    public class Person
    {
      public long PersonID { get; set; }
      public string Name { get; set; }
      public string Surname { get; set; }
    }

SharpCSV will create a CSV file by using the property names as headings in the file.

To create a CSV file, simply call the PopulateCSV<> method as follows:
      
      public byte[] GetPersonCSV(List<Person> personList)
      {
        byte[] personCSVContent = null;
        using (CSVWriter csvWriter = new CSVWriter())
        {
          csvWriter.PopulateCSV<Person>(true, personList); //true value indicates first line contains column names
          personCSVContent = csvWriter.Save();
        }
        return personCSVContent;
      }
      
If you would like to define different property names, simply add the CSVColumnName attribute to the property:

    public class Person
    {
      
      public long PersonID { get; set; }
      [CSVColumnName("Person Name")]
      public string Name { get; set; }
      public string Surname { get; set; }
    }
    
If you would like to exclude a column, simply add the CSVColumnName attribute to the property with the value as depicted below:

    public class Person
    {
      [CSVColumnName("csv-ignore-column")]
      public long PersonID { get; set; }
      [CSVColumnName("Person Name")]
      public string Name { get; set; }
      public string Surname { get; set; }
    }
    
This is not a complete library, but simply one that has been created to make CSV creation slightly easier and painless. There are still many improvements that can be made.

Hope you like the library!
