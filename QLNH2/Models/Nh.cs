using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Nh
{
    public int Id { get; set; }

    public string? MaNh { get; set; }

    public string? TenNh { get; set; }

    public int? TimeCre { get; set; }

    public int? TimeUp { get; set; }

    public virtual ICollection<Pcgvgd> Pcgvgds { get; set; } = new List<Pcgvgd>();
}
