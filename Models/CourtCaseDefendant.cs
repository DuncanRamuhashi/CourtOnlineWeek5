using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

[PrimaryKey("CourtCaseId", "DefendantId")]
public partial class CourtCaseDefendant
{
    [Key]
    public int CourtCaseId { get; set; }

    [Key]
    [StringLength(50)]
    public string DefendantId { get; set; } = null!;

    [StringLength(50)]
    public string? Role { get; set; }

    public bool IsPrimary { get; set; }

    [ForeignKey("CourtCaseId")]
    [InverseProperty("CourtCaseDefendants")]
    public virtual CourtCase CourtCase { get; set; } = null!;

    [ForeignKey("DefendantId")]
    [InverseProperty("CourtCaseDefendants")]
    public virtual Defendant Defendant { get; set; } = null!;
}
