using CsvHelper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatCanIReadToday
{
    public static class BookTestCaseSource
    {
        public static IEnumerable<TestCaseData> GetTestCases()
        {
            using (var reader = new StreamReader("C:\\PersonalProjects\\WhatCanIReadToday\\Books.csv"))
            {
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    return csv.GetRecords<TestCaseData>().ToList();
                }
            }
        }
    }
}
