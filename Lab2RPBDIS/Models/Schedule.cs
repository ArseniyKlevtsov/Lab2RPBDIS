using System;
using System.Collections.Generic;

namespace Lab2RPBDIS.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int? TrainId { get; set; }

    public int? StopId { get; set; }

    public byte? NumberOfDayOfWeek { get; set; }

    public TimeSpan? ArrivalTime { get; set; }

    public TimeSpan? DepartureTime { get; set; }

    public virtual Stop? Stop { get; set; }

    public virtual Train? Train { get; set; }
}
