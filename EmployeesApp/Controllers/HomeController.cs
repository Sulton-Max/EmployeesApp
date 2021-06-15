using EmployeesApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _appEnvironment;
        private EmployeesAM _empAM;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment appEnvironment, EmployeesAM empAM)
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
            _empAM = empAM;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<EmployeeVM> employees = _empAM.GetAll();

            // For Sorting
            ViewData["SortAscending"] = true;
            ViewData["SearchValue"] = "";
            ViewData["SortColumn"] = "";

            // For pagination
            ViewData["CurrentDataIndex"] = (employees.Count() > 10) ? 10 : employees.Count();
            ViewData["TotalDataCount"] = employees.Count();

            return View(employees);
        }

        [HttpPost]
        public IActionResult AddFile()
        {
            // Getting input file
            IFormFileCollection files = Request.Form.Files;
            if (files.Count() > 0)
            {
                IFormFile uploadedFile = files[0];
                // Checking the file extension
                string fileExtension = uploadedFile.FileName.Substring(uploadedFile.FileName.IndexOf('.')).ToLower();
                if (fileExtension.Equals(".csv"))
                {
                    string fileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
                    string path = Path.Combine(_appEnvironment.WebRootPath, "temp", fileName);
                    using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        uploadedFile.CopyToAsync(fileStream);
                        fileStream.Flush();
                    }
                    List<EmployeeDM> employees = CSVEmployeeModelParser.ParseFrom(path);
                    int rowsCount = _empAM.Add(employees);
                    ViewData["Message"] = $"{rowsCount} Rows affected";
                    ViewData["MessageType"] = "success";
                    return View();
                }
                else
                {
                    ViewData["Message"] = "File extension is not acceptable";
                    ViewData["MessageType"] = "alert";
                }
            }
            else
            {
                ViewData["Message"] = "Please upload a file";
                ViewData["MessageType"] = "alert";
            }
            return View();
        }

        // Editing 
        [HttpGet]
        public 

        // Sorting
        [HttpGet]
        public IActionResult Sort(string searchValue, string sortColumn, bool sortAscending)
        {
            return View("Index", SortingFunction(searchValue, sortColumn, sortAscending));
        }


        [HttpGet]
        public IActionResult GetNext()
        {
            return View("Index");
        }

        private List<EmployeeVM> SortingFunction(string searchValue, string sortColumn, bool sortAscending)
        {
            // Getting all employees
            List<EmployeeVM> employees = _empAM.GetAll();

            // Filtering
            if (searchValue != null && searchValue != string.Empty)
            {
                string searchWord = searchValue.ToLower();
                employees = employees.Where(employee =>
                (employee.PayrollNumber.ToLower().Contains(searchWord) ||
                    employee.Forename.ToLower().Contains(searchWord) ||
                    employee.Surname.ToLower().Contains(searchWord) ||
                    employee.Telephone.ToLower().Contains(searchWord))).ToList();
            }

            // Sorting by Column
            if (sortAscending)
            {
                employees = sortColumn.ToLower() switch
                {
                    "payrollnumber" => employees.OrderBy(employee => employee.PayrollNumber).ToList(),
                    "forename" => employees.OrderBy(employee => employee.Forename).ToList(),
                    "surname" => employees.OrderBy(employee => employee.Surname).ToList(),
                    "dateofbirth" => employees.OrderBy(employee => employee.DateOfBirth).ToList(),
                    "telephone" => employees.OrderBy(employee => employee.Telephone).ToList(),
                    "emailhome" => employees.OrderBy(employee => employee.EmailHome).ToList(),
                    _ => employees.OrderBy(employee => employee.PayrollNumber).ToList()
                };
            }
            else
            {
                employees = sortColumn.ToLower() switch
                {
                    "payrollnumber" => employees.OrderByDescending(employee => employee.PayrollNumber).ToList(),
                    "forename" => employees.OrderByDescending(employee => employee.Forename).ToList(),
                    "surname" => employees.OrderByDescending(employee => employee.Surname).ToList(),
                    "dateofbirth" => employees.OrderByDescending(employee => employee.DateOfBirth).ToList(),
                    "telephone" => employees.OrderByDescending(employee => employee.Telephone).ToList(),
                    "emailhome" => employees.OrderByDescending(employee => employee.EmailHome).ToList(),
                    _ => employees.OrderByDescending(employee => employee.PayrollNumber).ToList()
                };
            }

            // Saving searching and sorting values
            ViewData["SortAscending"] = !sortAscending;
            ViewData["SearchValue"] = (searchValue != null) ? searchValue : "";
            ViewData["SortColumn"] = sortColumn;

            return employees;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
