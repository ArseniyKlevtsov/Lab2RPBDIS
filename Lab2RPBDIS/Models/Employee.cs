using System;
using System.Collections.Generic;

namespace Lab2RPBDIS.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public int? Age { get; set; }

    public float? WorkExperience { get; set; }

    public int? PositionId { get; set; }

    public DateTime? HireDate { get; set; }

    public virtual Position? Position { get; set; }

    public virtual ICollection<TrainStaff> TrainStaffs { get; set; } = new List<TrainStaff>();
}
