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
            // Show the whole list
            List<EmployeeVM> employees = GetData("", "", true, 10);
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
                    return View("State");
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
            return View("State");
        }

        [HttpGet]
        public IActionResult Edit(string id, string sVal, string sCol, bool sAsc, int cIdx)
        {
            // Get the data by id to edit
            EmployeeDM employee = _empAM.Get(id);
            ViewData["Employee"] = employee;

            // Show the whole list
            List<EmployeeVM> employees = GetData(sVal, sCol, sAsc, cIdx);
            SetViewDataFunction(sVal, sCol, sAsc, employees.Count(), cIdx);
            return View("Index", employees);
        }

        [HttpPost]
        public IActionResult Edit(string act, string sVal, string sCol, bool sAsc, int cIdx, EmployeeDM emp = null)
        {
            switch (act.ToLower())
            {
                case "save":
                    {
                        if (emp != null)
                            _empAM.Edit(emp);
                        ViewData["Message"] = $"{1} Row updated";
                        ViewData["MessageType"] = "success";
                        return View("State");
                    }
                case "delete":
                    {
                        if (emp != null)
                            _empAM.Delete(emp);
                        ViewData["Message"] = $"{1} Row affected";
                        ViewData["MessageType"] = "success";
                        return View("State");
                    }
                default: break;
            }

            // Show the whole list
            List<EmployeeVM> employees = GetData(sVal, sCol, sAsc, cIdx);
            SetViewDataFunction(sVal, sCol, sAsc, employees.Count(), cIdx);
            return View("Index", employees);
        }

        [HttpGet]
        public IActionResult Sort(string sVal, string sCol, bool sAsc, int cIdx)
        {
            // Show the whole list
            List<EmployeeVM> employees = GetData(sVal, sCol, sAsc, cIdx);
            SetViewDataFunction(sVal, sCol, sAsc, employees.Count(), cIdx);
            return View("Index", employees);
        }

        [HttpGet]
        public IActionResult Pagination(string act, string sVal, string sCol, bool sAsc, int cIdx)
        {
            List<EmployeeVM> employees = GetDataSortedByKey(sVal);  // Get all searched data sorted by key
            employees = PaginationFunction(cIdx, employees);        // Get the data with current Index                         
            employees = SortingFunction(sCol, sAsc, employees);     // Sort data by column

            SetViewDataFunction(sVal, sCol, sAsc, employees.Count(), cIdx);
            return View("Index");
        }

        public List<EmployeeVM> GetData(string sVal, string sCol, bool sAsc, int cIdx)
        {
            if (cIdx == 0)
                cIdx = 10;
            List<EmployeeVM> employees = GetDataSortedByKey(sVal);  // Get all searched data sorted by key
            employees = PaginationFunction(cIdx, employees);        // Get the data with current Index                         
            employees = SortingFunction(sCol, sAsc, employees);     // Sort data by column
            SetViewDataFunction(sVal, sCol, sAsc, employees.Count(), ((employees.Count > 10) ? cIdx : employees.Count()));
            return employees;
        }

        private List<EmployeeVM> GetDataSortedByKey(string sVal)
        {
            // Get searched data if search value is not null
            List<EmployeeVM> employees = _empAM.GetAll();
            if (sVal != null && sVal != string.Empty)
                employees = _empAM.GetAll().Where(employee =>
                    (employee.PayrollNumber.ToLower().Contains(sVal) ||
                        employee.Forename.ToLower().Contains(sVal) ||
                        employee.Surname.ToLower().Contains(sVal) ||
                        employee.Telephone.ToLower().Contains(sVal))).ToList();
            // Get data sorted by key
            employees = employees.OrderBy(employee => employee.PayrollNumber).ToList();
            return employees;
        }

        private List<EmployeeVM> SortingFunction(string sCol, bool sAsc, List<EmployeeVM> employees)
        {
            // Correcting the sort column value
            sCol = (sCol == null) ? "payrollnumber" : sCol;

            // Sorting by Column
            if (sAsc)
            {
                employees = sCol.ToLower() switch
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
                employees = sCol.ToLower() switch
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
            return employees;
        }

        private List<EmployeeVM> PaginationFunction(int cIdx, List<EmployeeVM> employees)
        {
            int skippedCount = (cIdx > 10) ? (cIdx - 10) : 0;
            employees.Skip(skippedCount).Take(10);
            return employees;
        }

        private void SetViewDataFunction(string sVal, string sCol, bool sAsc, int tCnt, int cIdx, bool iEdit = false)
        {
            // For Sorting
            ViewData["SearchValue"] = sVal;
            ViewData["SortColumn"] = sCol;
            ViewData["SortAscending"] = sAsc;

            // For pagination
            ViewData["TotalDataCount"] = tCnt;
            ViewData["CurrentDataIndex"] = cIdx;

            // For editing 
            ViewData["IsEditMode"] = iEdit;
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
