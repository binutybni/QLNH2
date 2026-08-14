using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class PointStudent
{
    public int Id { get; set; }

    public double? Point { get; set; }

    public string? Evaluate { get; set; }

    public int? IdSv { get; set; }

    public int? IdMh { get; set; }

    public int? IdQt { get; set; }

    public virtual Subject? IdMhNavigation { get; set; }

    public virtual Progress? IdQtNavigation { get; set; }

    public virtual Hocsinh? IdSvNavigation { get; set; }
}
