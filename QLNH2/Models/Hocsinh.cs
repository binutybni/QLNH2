using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Hocsinh
{
    public int Id { get; set; }

    public string? NameStudent { get; set; }

    public string? CodeStudent { get; set; }

    public int? TimeCreate { get; set; }

    public int? TimeUpdate { get; set; }

    public int? Classid { get; set; }

    public virtual Class? Class { get; set; }
}
