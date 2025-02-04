using System;
using System.Collections.Generic;

namespace proje.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}

