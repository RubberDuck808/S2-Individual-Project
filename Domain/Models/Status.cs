﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Status
    {
        public int StatusId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}