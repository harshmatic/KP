using ESPL.KP.Helpers.Core;

namespace ESPL.KP.Helpers.Employee
{
    public class EmployeesResourceParameters:BaseResourceParameters
    {
         public string OrderBy { get; set; } = "FirstName";
    }
}