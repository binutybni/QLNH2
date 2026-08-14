using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Progress
{
    public int Id { get; set; }

    public string? NameProgress { get; set; }

    public int? TimeCre { get; set; }

    public int? TimeUp { get; set; }

    public virtual ICollection<PointStudent> PointStudents { get; set; } = new List<PointStudent>();
}
