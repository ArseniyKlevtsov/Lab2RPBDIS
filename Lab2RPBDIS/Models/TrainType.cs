using System;
using System.Collections.Generic;

namespace Lab2RPBDIS.Models;

public partial class TrainType
{
    public int TrainTypeId { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<Train> Trains { get; set; } = new List<Train>();
}
