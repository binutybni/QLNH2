using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class Dkmh
{
    public int Id { get; set; }

    public int? IdGvgdmh { get; set; }

    public int? IdHs { get; set; }

    public int? TimeRegister { get; set; }

    public virtual Pcgvgd? IdGvgdmhNavigation { get; set; }

    public virtual Hocsinh? IdHsNavigation { get; set; }
}
