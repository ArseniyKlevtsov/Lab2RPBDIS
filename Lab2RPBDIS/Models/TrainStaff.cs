using System;
using System.Collections.Generic;

namespace Lab2RPBDIS.Models;

public partial class TrainStaff
{
    public int TrainStaffId { get; set; }

    public int? TrainId { get; set; }

    public int? EmployeeId { get; set; }

    public byte? NumberOfDayOfWeek { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Train? Train { get; set; }
}
