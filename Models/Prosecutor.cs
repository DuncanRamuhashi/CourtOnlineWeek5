using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

public partial class Prosecutor
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; } = null!;

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Rank { get; set; }

    [StringLength(150)]
    public string? Office { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Prosecutor")]
    public virtual ICollection<CourtCase> CourtCases { get; set; } = new List<CourtCase>();
}
