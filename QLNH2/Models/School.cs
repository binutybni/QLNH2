using System;
using System.Collections.Generic;

namespace QLNH2.Models;

public partial class School
{
    public int Id { get; set; }

    public string? NameSchool { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public int? TimeCreate { get; set; }

    public int? TimeUpdate { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
