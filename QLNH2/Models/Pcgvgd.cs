using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Pcgvgd
{
    public int Id { get; set; }

    public int? IdGvgd { get; set; }

    public int? IdClass { get; set; }

    public int? IdMh { get; set; }

    public int? IdQt { get; set; }

    public int? IdNh { get; set; }

    public virtual ICollection<Dkmh> Dkmhs { get; set; } = new List<Dkmh>();

    public virtual Class? IdClassNavigation { get; set; }

    public virtual Gvgd? IdGvgdNavigation { get; set; }

    public virtual Subject? IdMhNavigation { get; set; }

    public virtual Nh? IdNhNavigation { get; set; }

    public virtual Progress? IdQtNavigation { get; set; }
}
