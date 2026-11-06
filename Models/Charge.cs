using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

[Index("CourtCaseId", Name = "IX_Charges_CourtCaseId")]
public partial class Charge
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    public int CourtCaseId { get; set; }

    [StringLength(50)]
    public string? ChargeCode { get; set; }

    [StringLength(200)]
    public string? Statute { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Plea { get; set; }

    [StringLength(30)]
    public string? Severity { get; set; }

    [ForeignKey("CourtCaseId")]
    [InverseProperty("Charges")]
    public virtual CourtCase CourtCase { get; set; } = null!;
}
