﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTOs.Landlord;
using BLL.DTOs.Student;

namespace BLL.Interfaces
{
    public interface IAccountService
    {
        Task RegisterStudentAsync(StudentRegistrationDto dto);
        Task RegisterLandlordAsync(LandlordRegistrationDto dto);
        Task<(bool Success, string? UserId, string? Role, string? Error)> LoginAsync(string email, string password);

        Task<string> GetFullNameByUserIdAsync(string userId);

    }

}
