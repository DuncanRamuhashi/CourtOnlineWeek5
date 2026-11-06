using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

[Index("CaseNumber", Name = "IX_OriginalCases_CaseNumber")]
[Index("CaseNumber", Name = "UQ_OriginalCases_CaseNumber", IsUnique = true)]
public partial class OriginalCase
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string CaseNumber { get; set; } = null!;

    [StringLength(250)]
    public string Name { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    [StringLength(50)]
    public string? CaseType { get; set; }

    [StringLength(20)]
    public string? Priority { get; set; }

    [StringLength(150)]
    public string? ReportedBy { get; set; }

    [InverseProperty("OriginalCase")]
    public virtual ICollection<CourtCase> CourtCases { get; set; } = new List<CourtCase>();
}
