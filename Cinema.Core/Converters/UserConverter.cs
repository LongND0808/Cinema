using Cinema.Core.DTOs;
using Cinema.Domain.Entities;
using System;

namespace Cinema.Core.Converters
{
    public class UserConverter
    {
        public UserDTO ConvertToDTO(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                RankCustomerId = user.RankCustomerId,
                UserStatusId = user.UserStatusId,
                Point = user.Point,
                IsDeleted = user.isDeleted,
                PhoneNumber = user.PhoneNumber,
            };
        }
    }
}
