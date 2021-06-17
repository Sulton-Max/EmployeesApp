using CsvHelper;
using CsvHelper.Configuration;
using EmployeesApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesApp.Controllers
{
    /// <summary>
    /// Defines static method to parse CSV data to object
    /// </summary>
    public static class CSVEmployeeModelParser
    {
        /// <summary>
        /// Parses data in the specified path to Employee Domain Models
        /// </summary>
        /// <param name="path">Path to the .csv file</param>
        /// <returns>List of EmployeeDM</returns>
        public static List<EmployeeDM> ParseFrom(string path)
        {
            // Check if file realls exists
            if (File.Exists(path))
            {
                // Config the CSV Stream to prevent to mapping by header 
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false
                };

                // Stream to read the file
                using var fileStream = File.OpenText(path);
                using var csvStream = new CsvReader(fileStream, config);
                IEnumerable<EmployeeCSBM> records = csvStream.GetRecords<EmployeeCSBM>();
                var employeesCSVMs = records?.ToList();

                // Check if parsing was successfull and are there any objects parsed
                if (employeesCSVMs != null && employeesCSVMs.Count() > 1)
                {
                    // Remove the header object
                    employeesCSVMs.RemoveAt(0);

                    // Convert only valid CVS models to Domain models
                    List<EmployeeDM> employees = new List<EmployeeDM>();
                    foreach (EmployeeCSBM cvsm in employeesCSVMs)
                        if (cvsm.IsValid())
                            employees.Add(new EmployeeDM(cvsm));

                    return employees;
                }

                // Delete the file after processing
                File.Delete(path);
            }

            return null;
        }
    }
}