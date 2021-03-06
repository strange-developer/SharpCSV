﻿using CSVLib.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSVLib
{
  public class CSVWriter : IDisposable
  {
    #region Properties and Fields
    private bool _disposed;

    private MemoryStream _csvBuffer { get; set; }
    private StreamWriter _csvWriter { get; set; }
    private ICollection<string> _fileHeaderList { get; set; }
    private bool _isFirstColumn { get; set; }

    private string _dateTimeFormat = "ddd d MMM hh:mm:ss";
    private string _nullValueText = "NULL";
    private string _separator = ",";

    private const string _ignoreAttributeValue = "csv-ignore-column";
    private readonly Type _dateTimeType = typeof(DateTime);
    private readonly Type _nullableDateTimeType = typeof(DateTime?);

    #endregion

    public CSVWriter(string separator = ",", string dateTimeFormat = "ddd d MMM hh:mm:ss tt")
    {
      _dateTimeFormat = dateTimeFormat;
      _separator = separator;
      _disposed = false;
      _csvBuffer = new MemoryStream();
      _csvWriter = new StreamWriter(_csvBuffer, Encoding.UTF8);
      _fileHeaderList = new List<string>();
      _isFirstColumn = true;
    }

    #region Set Methods

    public void SetSeparator(string separator = ",")
    {
      _separator = separator;
    }

    //dateTimeFormat must confirm to the MSDN documentation at on formatting DateTimes
    public void SetDateTimeFormat(string dateTimeFormat = "ddd d MMM hh:mm:ss tt")
    {
      _dateTimeFormat = dateTimeFormat;
    }

    public void SetNullValueText(string nullValueText = "NULL")
    {
      _nullValueText = nullValueText;
    }

    #endregion

    #region CSV Append Methods

    public void AppendLine(IEnumerable<string> values)
    {
      if (values != null)
      {
        string csvEscapedLine = string.Join(_separator, values.Select(x => CSVEscaper.Escape(x)));
        _csvWriter.WriteLine(csvEscapedLine);
        //set _isFirstColumn to true. This ensures that if AppendColumn is called, we know that we will be appending the first column
        _isFirstColumn = true;
      }
    }

    public void AppendLine()
    {
      //set _isFirstColumn to true. This ensures that if AppendColumn is called, we know that we will be appending the first column
      _isFirstColumn = true;
      _csvWriter.WriteLine();
    }

    public void AppendColumn(string value)
    {
      if (!string.IsNullOrWhiteSpace(value))
      {
        //if the first line is encountered, we do not want to add a column separator. Set the _isFirstColumn to false so that this will not skip commas
        if (_isFirstColumn)
        {
          _isFirstColumn = false;
        }
        else
        {
          //if it is not the first column being appended to a line, we need to split the columns by adding the comma separator
          _csvWriter.Write(_separator);
        }

        string columnToAppend = CSVEscaper.Escape(value);
        _csvWriter.Write(columnToAppend);
      }
    }

    #endregion

    public void PopulateCSV<T>(bool firstLineContainsColumnNames, IEnumerable<T> rowList)
    {
      if (rowList != null && rowList.Count() > 0)
      {
        if (_fileHeaderList.Count > 0)
        {
          _fileHeaderList.Clear();
        }

        //get all the properties whilst honoring the ignore attributes
        ReadOnlyCollection<PropertyInfo> propList = ParseColumnAttributes(typeof(T).GetProperties());

        if (firstLineContainsColumnNames)
        {
          AppendLine(_fileHeaderList);
        }

        foreach (var row in rowList)
        {
          ICollection<string> rowContent = new Collection<string>();
          foreach (PropertyInfo prop in propList)
          {
            //propVal is the value of the property in the variable row
            string propVal = GetValue(prop, row);
            rowContent.Add(propVal);
          }
          AppendLine(rowContent);
        }
      }
    }

    public string GetValue<T>(PropertyInfo prop, T row)
    {
      string propVal = _nullValueText;
      object propObj = prop.GetValue(row);
      if (propObj != null)
      {
        if (prop.PropertyType == _dateTimeType || prop.PropertyType == _nullableDateTimeType)
        {
          propVal = Convert.ToDateTime(propObj).ToString(_dateTimeFormat);
        }
        else
        {
          //propVal is the value of the property in the variable row
          propVal = Convert.ToString(propObj);
        }
      }
      return propVal;
    }

    private ReadOnlyCollection<PropertyInfo> ParseColumnAttributes(PropertyInfo[] propList)
    {
      List<PropertyInfo> columnNameList = new List<PropertyInfo>();

      if (propList != null && propList.Length > 0)
      {
        foreach (PropertyInfo prop in propList)
        {
          //gets the PDF column name and takes the PDFColumnName attribute into account
          string csvColumnName = GetCSVColumnName(prop);
          //make sure that the string is not empty and does not match the ignore attribute value
          if (!string.IsNullOrWhiteSpace(csvColumnName) && csvColumnName.ToLower() != _ignoreAttributeValue)
          {
            columnNameList.Add(prop);
            _fileHeaderList.Add(csvColumnName);
          }
        }
      }

      return columnNameList.AsReadOnly();
    }

    private string GetCSVColumnName(PropertyInfo propInfo)
    {
      string csvColumnName = propInfo.Name;
      //check if a custom attribute is set in order to allow the caller to override the property name when creating the CSV table columns
      Attribute csvColumnNameAttrib = propInfo.GetCustomAttribute(typeof(CSVColumnName), false);
      if (csvColumnNameAttrib != null)
      {
        //only perform the typecast if the property was decorated with the PDFColumnName attribute
        csvColumnName = ((CSVColumnName)csvColumnNameAttrib).CsvColumName;
      }
      return csvColumnName;
    }

    public byte[] Save()
    {
      _csvWriter.Flush();
      //return a copy of the memorystream
      return _csvBuffer.ToArray();
    }

    #region Dispose Implementation

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          if (_csvWriter != null)
          {
            _csvWriter.Dispose();
          }

          if (_csvBuffer != null)
          {
            _csvBuffer.Dispose();
          }
        }
      }
    }

    #endregion
  }
}
