using System;
using System.Collections.Generic;

namespace Lab2RPBDIS.Models;

public partial class Position
{
    public int PositionId { get; set; }

    public string? PositionName { get; set; }

    public float? SalaryUsd { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
