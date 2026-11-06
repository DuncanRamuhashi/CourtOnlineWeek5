using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Models;

public partial class Judge
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; } = null!;

    [StringLength(150)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Title { get; set; }

    public int? CourtId { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    public DateOnly? AppointmentDate { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CourtId")]
    [InverseProperty("Judges")]
    public virtual Court? Court { get; set; }

    [InverseProperty("Judge")]
    public virtual ICollection<CourtCase> CourtCases { get; set; } = new List<CourtCase>();
}
