using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVLib
{
  public static class CSVEscaper
  {
    private const string _quote = "\"";
    private const string _escapedQuote = "\"\"";
    private static char[] _charsThatMustBeQuoted = { ',', '"', '\n' };

    public static string Escape(string valueToEscape)
    {
      if (valueToEscape.Contains(_quote))
        valueToEscape = valueToEscape.Replace(_quote, _escapedQuote);
      

      if (valueToEscape.IndexOfAny(_charsThatMustBeQuoted) > -1)
        valueToEscape = _quote + valueToEscape + _quote;

      return valueToEscape;
    }
  }
}
