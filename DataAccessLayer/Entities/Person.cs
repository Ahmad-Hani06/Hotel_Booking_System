using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Person
{
    public int PersonId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public virtual Guest? Guest { get; set; }

    public virtual User? User { get; set; }
}
