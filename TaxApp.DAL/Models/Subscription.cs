using System;
using System.Collections.Generic;
namespace TaxApp.DAL.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double? Amount { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
