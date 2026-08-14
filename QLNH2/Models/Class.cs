using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Class
{
    public int Id { get; set; }

    public string? NameClass { get; set; }

    public int? Schoolid { get; set; }

    public int? TimeCreate { get; set; }

    public int? TimeUpdate { get; set; }

    public virtual ICollection<Hocsinh> Hocsinhs { get; set; } = new List<Hocsinh>();

    public virtual School? School { get; set; }
}
