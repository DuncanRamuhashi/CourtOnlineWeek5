using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

[Index("Idnumber", Name = "IX_Defendants_IDNumber")]
public partial class Defendant
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; } = null!;

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column("IDNumber")]
    [StringLength(20)]
    public string? Idnumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(50)]
    public string? Nationality { get; set; }

    public bool IsInCustody { get; set; }

    [InverseProperty("Defendant")]
    public virtual ICollection<CourtCaseDefendant> CourtCaseDefendants { get; set; } = new List<CourtCaseDefendant>();
}
