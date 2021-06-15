using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeAppCons
{
    public class EmployeeCSVM
    {
        [Index(0)]
        public string PayrollNumber { get; set; }
        [Index(1)]
        public string Forename { get; set; }
        [Index(2)]
        public string Surname { get; set; }
        [Index(3)]
        public string DateOfBirth { get; set; }
        [Index(4)]
        public string Telephone { get; set; }
        [Index(5)]
        public string Mobile { get; set; }
        [Index(6)]
        public string Address { get; set; }
        [Index(7)]
        public string Address2 { get; set; }
        [Index(8)]
        public string Postcode { get; set; }
        [Index(9)]
        public string EmailHome { get; set; }
        [Index(10)]
        public string StartDate { get; set; }

        public bool Validate()
        {
            return (string.IsNullOrWhiteSpace(PayrollNumber) &&
                string.IsNullOrWhiteSpace(Forename) &&
                string.IsNullOrWhiteSpace(Surname) &&
                string.IsNullOrWhiteSpace(DateOfBirth) &&
                string.IsNullOrWhiteSpace(Telephone) &&
                string.IsNullOrWhiteSpace(Mobile) &&
                string.IsNullOrWhiteSpace(Address) &&
                string.IsNullOrWhiteSpace(Address2) &&
                string.IsNullOrWhiteSpace(Postcode) &&
                string.IsNullOrWhiteSpace(EmailHome) &&
                string.IsNullOrWhiteSpace(StartDate) &&
                DateTime.TryParse(DateOfBirth, out DateTime temp1) &&
                DateTime.TryParse(StartDate, out DateTime temp2));
        }
    }

    public class EmployeeDM
    {
        public string Payroll_Number { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime Date_of_Birth { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Address_2 { get; set; }
        public string Postcode { get; set; }
        public string Email_Home { get; set; }
        public DateTime Start_Date { get; set; }
        public EmployeeDM(EmployeeCSVM csvm)
        {
            Payroll_Number = csvm.PayrollNumber;
            Forename = csvm.Forename;
        }
    }

}
