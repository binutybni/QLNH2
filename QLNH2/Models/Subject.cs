using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string? MaMh { get; set; }

    public string? NameSub { get; set; }

    public int? TimeCre { get; set; }

    public int? TimeUp { get; set; }

    public virtual ICollection<Pcgvgd> Pcgvgds { get; set; } = new List<Pcgvgd>();

    public virtual ICollection<PointStudent> PointStudents { get; set; } = new List<PointStudent>();
}
