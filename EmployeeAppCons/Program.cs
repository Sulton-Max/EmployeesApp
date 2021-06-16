﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace EmployeeAppCons
{
    class Program
    {
        static void Main(string[] args)
        {
            //string path = @"C:\Users\User\source\repos\PROJECTS\13_PRACTICAL\JOB_APP_TASKS\EmployeesApp\EmployeesApp\wwwroot\temp\dataset.csv";

            //var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            //{
            //    HasHeaderRecord = false
            //};

            //using var fileStream = File.OpenText(path);
            //using var csvStream = new CsvReader(fileStream, config);
            //IEnumerable<EmployeeCSVM> records = csvStream.GetRecords<EmployeeCSVM>();
            //var employees = records.ToList();
            //employees.RemoveAt(0);

            //int initalCount = employees.Count();

            //// Get the number of successfully parsed models 
            //while(true)
            //{
            //    bool hasChanged = false;
            //    for (int index = 0; index < employees.Count(); index++)
            //        if (hasChanged = employees[index].Validate() == true)
            //            break;
            //    if (!hasChanged)
            //        break;
            //}

            //int latterCount = employees.Count();

            //foreach(EmployeeCSVM employee in employees)
            //    Console.WriteLine(employee.ToString());

            List<Data> data = new List<Data>
            { 
                new Data(12),
                new Data(1),
                new Data(122),
                new Data(89),
                new Data(23),
                new Data(202),
                new Data(77),
                new Data(45),
                new Data(15)
            };
            data = data.OrderBy(data => data.Value).ToList();
            foreach(var d in data)
                Console.WriteLine(d);
        }

        public class Data
        {
            public int Value { get; set; }
            public Data(int val) => Value = val;
        }

    }
}
