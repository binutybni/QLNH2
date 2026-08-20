using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Gvgd
{
    public int Id { get; set; }

    public string? MaGvgd { get; set; }

    public string? TenGvgd { get; set; }

    public int? TimeCre { get; set; }

    public int? TimeUp { get; set; }

    public virtual ICollection<Pcgvgd> Pcgvgds { get; set; } = new List<Pcgvgd>();
}
