﻿using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Application
{
    public class ApplicationUpdateDto
    {
        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public int StatusId { get; set; }
    }
}
