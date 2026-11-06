using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

[Index("NextHearingDate", Name = "IX_CourtCases_NextHearingDate")]
[Index("OriginalCaseId", Name = "IX_CourtCases_OriginalCaseId")]
[Index("Status", Name = "IX_CourtCases_Status")]
public partial class CourtCase
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public int OriginalCaseId { get; set; }

    public int? CourtId { get; set; }

    [StringLength(50)]
    public string? JudgeId { get; set; }

    [StringLength(50)]
    public string? ProsecutorId { get; set; }

    [StringLength(50)]
    public string? CaseNumber { get; set; }

    public DateOnly? FilingDate { get; set; }

    public DateTime? NextHearingDate { get; set; }

    public DateOnly? ClosedDate { get; set; }

    [StringLength(200)]
    public string? Outcome { get; set; }

    public string? Notes { get; set; }

    public bool IsConfidential { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("CourtCase")]
    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    [ForeignKey("CourtId")]
    [InverseProperty("CourtCases")]
    public virtual Court? Court { get; set; }

    [InverseProperty("CourtCase")]
    public virtual ICollection<CourtCaseDefendant> CourtCaseDefendants { get; set; } = new List<CourtCaseDefendant>();

    [ForeignKey("JudgeId")]
    [InverseProperty("CourtCases")]
    public virtual Judge? Judge { get; set; }

    [ForeignKey("OriginalCaseId")]
    [InverseProperty("CourtCases")]
    public virtual OriginalCase OriginalCase { get; set; } = null!;

    [ForeignKey("ProsecutorId")]
    [InverseProperty("CourtCases")]
    public virtual Prosecutor? Prosecutor { get; set; }
}
