using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

public partial class Court
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(150)]
    public string Location { get; set; } = null!;

    [StringLength(20)]
    public string? CourtCode { get; set; }

    [StringLength(50)]
    public string? CourtLevel { get; set; }

    [StringLength(100)]
    public string? Province { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Court")]
    public virtual ICollection<CourtCase> CourtCases { get; set; } = new List<CourtCase>();

    [InverseProperty("Court")]
    public virtual ICollection<Judge> Judges { get; set; } = new List<Judge>();
}
