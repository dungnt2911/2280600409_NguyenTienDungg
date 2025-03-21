﻿using Microsoft.AspNetCore.Identity;

namespace _2280600409_NguyenTienDungg.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
    }
}
