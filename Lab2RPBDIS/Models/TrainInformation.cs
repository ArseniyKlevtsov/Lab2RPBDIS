using System;
using System.Collections.Generic;

namespace Lab2RPBDIS.Models;

public partial class TrainInformation
{
    public int TrainId { get; set; }

    public string? TrainNumber { get; set; }

    public string? TrainType { get; set; }

    public string? DepartureStop { get; set; }

    public string? ArrivalStop { get; set; }

    public float? DistanceInKm { get; set; }

    public TimeSpan? DepartureTime { get; set; }

    public TimeSpan? ArrivalTime { get; set; }

    public string IsBrandedTrain { get; set; } = null!;
}
