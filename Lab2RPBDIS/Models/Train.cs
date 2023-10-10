using System;
using System.Collections.Generic;

namespace Lab2RPBDIS.Models;

public partial class Train
{
    public int TrainId { get; set; }

    public string? TrainNumber { get; set; }

    public int? TrainTypeId { get; set; }

    public int? ArrivalStopId { get; set; }

    public int? DepartureStopId { get; set; }

    public float? DistanceInKm { get; set; }

    public TimeSpan? ArrivalTime { get; set; }

    public TimeSpan? DepartureTime { get; set; }

    public bool? IsBrandedTrain { get; set; }

    public virtual Stop? ArrivalStop { get; set; }

    public virtual Stop? DepartureStop { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<TrainStaff> TrainStaffs { get; set; } = new List<TrainStaff>();

    public virtual TrainType? TrainType { get; set; }

    public override string ToString()
    {
        return $"Train ID: {TrainId}, TrainNumber: {TrainNumber},  ..."; 
    }

}
