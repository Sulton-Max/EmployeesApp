using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesApp.Models
{
    /// <summary>
    /// Class for Employee Domain Model
    /// </summary>
    public class EmployeeDM
    {
        [Key]
        public string PayrollNumber { get; set; }
        [StringLength(20, ErrorMessage = "Forename can't be longer than 20 chars")]
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Postcode { get; set; }
        public string EmailHome { get; set; }
        public DateTime StartDate { get; set; }

        public EmployeeDM() { }

        public EmployeeDM(string pn, string fn, string sn, DateTime dob, string tp, string m, string add, string add2, string pc, string eh, DateTime sd)
        {
            PayrollNumber = pn;
            Forename = fn;
            Surname = sn;
            DateOfBirth = dob;
            Telephone = tp;
            Mobile = m;
            Address = add;
            Address2 = add2;
            Postcode = pc;
            EmailHome = eh;
            StartDate = sd;
        }

        public EmployeeDM(EmployeeCSBM csvm)
        {
            PayrollNumber = csvm.PayrollNumber;
            Forename = csvm.Forename;
            Surname = csvm.Surname;
            DateOfBirth = DateTime.Parse(csvm.DateOfBirth);
            Telephone = csvm.Telephone;
            Mobile = csvm.Mobile;
            Address = csvm.Address;
            Address2 = csvm.Address;
            Postcode = csvm.Postcode;
            EmailHome = csvm.EmailHome;
            StartDate = DateTime.Parse(csvm.StartDate);
        }

        public void Change(EmployeeDM emp)
        {
            Forename = emp.Forename;
            Surname = emp.Surname;
            DateOfBirth = emp.DateOfBirth;
            Telephone = emp.Telephone;
            Mobile = emp.Mobile;
            Address = emp.Address;
            Address2 = emp.Address2;
            Postcode = emp.Postcode;
            EmailHome = emp.EmailHome;
            StartDate = emp.StartDate;
        }
    }

    /// <summary>
    /// Class for Employee View Model
    /// </summary>
    public class EmployeeVM
    {
        public string PayrollNumber { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Telephone { get; set; }
        public string EmailHome { get; set; }

        public EmployeeVM() { }
        public EmployeeVM(string pn, string fn, string sn, DateTime dob, string tp, string eh)
        {
            PayrollNumber = pn;
            Forename = fn;
            Surname = sn;
            DateOfBirth = dob;
            Telephone = tp;
            EmailHome = eh;
        }

        public static explicit operator EmployeeVM(EmployeeDM emp)
        {
            return new EmployeeVM(emp.PayrollNumber, emp.Forename, emp.Surname, emp.DateOfBirth, emp.Telephone, emp.EmailHome);
        }
    }

    /// <summary>
    /// Class for Employee CSV Model, all properties are string
    /// </summary>
    public class EmployeeCSBM
    {
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string PayrollNumber { get; set; }
        [CsvHelper.Configuration.Attributes.Index(1)]
        public string Forename { get; set; }
        [CsvHelper.Configuration.Attributes.Index(2)]
        public string Surname { get; set; }
        [CsvHelper.Configuration.Attributes.Index(3)]
        public string DateOfBirth { get; set; }
        [CsvHelper.Configuration.Attributes.Index(4)]
        public string Telephone { get; set; }
        [CsvHelper.Configuration.Attributes.Index(5)]
        public string Mobile { get; set; }
        [CsvHelper.Configuration.Attributes.Index(6)]
        public string Address { get; set; }
        [CsvHelper.Configuration.Attributes.Index(7)]
        public string Address2 { get; set; }
        [CsvHelper.Configuration.Attributes.Index(8)]
        public string Postcode { get; set; }
        [CsvHelper.Configuration.Attributes.Index(9)]
        public string EmailHome { get; set; }
        [CsvHelper.Configuration.Attributes.Index(10)]
        public string StartDate { get; set; }

        /// <summary>
        /// Validates the properties of CSV Model, defines are they are valid and convertible
        /// </summary>
        /// <returns>True if validation successfully completed</returns>
        public bool IsValid()
        {
            string DateCorrector(string dateStr)
            {
                string[] date = dateStr.Split('/');
                if (!int.TryParse(date[0], out int test))
                    throw new FormatException("Data format is not acceptable");

                if (int.Parse(date[0]) > 12)
                {
                    var temp = date[0];
                    date[0] = date[1];
                    date[1] = temp;
                }

                dateStr = string.Join('/', date);
                return dateStr;
            }

            try
            {
                DateOfBirth = DateCorrector(DateOfBirth);
                StartDate = DateCorrector(StartDate);
            }
            catch(Exception)
            {
                return false;
            }

            return (!string.IsNullOrWhiteSpace(PayrollNumber) &&
                !string.IsNullOrWhiteSpace(Forename) &&
                !string.IsNullOrWhiteSpace(Surname) &&
                !string.IsNullOrWhiteSpace(DateOfBirth) &&
                !string.IsNullOrWhiteSpace(Telephone) &&
                !string.IsNullOrWhiteSpace(Mobile) &&
                !string.IsNullOrWhiteSpace(Address) &&
                !string.IsNullOrWhiteSpace(Address2) &&
                !string.IsNullOrWhiteSpace(Postcode) &&
                !string.IsNullOrWhiteSpace(EmailHome) &&
                !string.IsNullOrWhiteSpace(StartDate) &&
                DateTime.TryParse(DateOfBirth, out DateTime temp1) &&
                DateTime.TryParse(StartDate, out DateTime temp2));
        }
    }

    /// <summary>
    /// Class for Employee Application Model
    /// </summary>
    public class EmployeesAM
    {
        //public EmployeeDbContext EmployeesDB = new EmployeeDbContext();

        /// <summary>
        /// Returns all employees in EmployeeVM type
        /// </summary>
        /// <returns>List of EmployeeVM objects</returns>
        public List<EmployeeVM> GetAll()
        {
            using EmployeeDbContext EmployeesDB = new EmployeeDbContext();
            List<EmployeeVM> list = new List<EmployeeVM>();
            foreach (EmployeeDM employee in EmployeesDB.Employees)
                list.Add((EmployeeVM)employee);
            return list;
        }

        public EmployeeDM Get(string id)
        {
            using EmployeeDbContext EmployeesDB = new EmployeeDbContext();
            return EmployeesDB.Employees.Where(employee => employee.PayrollNumber.Equals(id)).FirstOrDefault();
        }

        public int Add(List<EmployeeDM> employees)
        {
            using EmployeeDbContext EmployeesDB = new EmployeeDbContext();
            int affectedRows = 0;
            // Adding objects after checking
            foreach (EmployeeDM employee in employees)
                if (EmployeesDB.Employees.Where(emp => emp.PayrollNumber == employee.PayrollNumber).FirstOrDefault() == null)
                {
                    affectedRows++;
                    EmployeesDB.Add(employee);
                }
            EmployeesDB.SaveChanges();
            return affectedRows;
        }

        internal void Edit(EmployeeDM emp)
        {
            using EmployeeDbContext EmployeesDB = new EmployeeDbContext();
            EmployeeDM employee = EmployeesDB.Employees.FirstOrDefault(employee => employee.PayrollNumber == emp.PayrollNumber);
            employee.Change(emp);
            EmployeesDB.SaveChanges();
        }

        public void Delete(EmployeeDM emp)
        {
            using EmployeeDbContext EmployeesDB = new EmployeeDbContext();
            EmployeesDB.Remove(emp);
            EmployeesDB.SaveChanges();
        }
    }

    /// <summary>
    /// Class for Empoyee DataBase Context EF 
    /// </summary>
    public class EmployeeDbContext : DbContext
    {
        public DbSet<EmployeeDM> Employees { get; set; }

        public EmployeeDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            string connString = @"Server=WIN-S2HR7LHV4GP\SOFT_SERVER;Database=Employees;Trusted_Connection=TRUE";
            builder.UseSqlServer(connString);
        }
    }

    public class EmployeeComparer : IComparer<string>
    {
        public int Compare([AllowNull] string x, [AllowNull] string y)
        {
            if (int.TryParse(x, out int xInt) && int.TryParse(y, out int yInt))
                return xInt.CompareTo(yInt);
            else
                return x.CompareTo(y);
        }
    }
}
