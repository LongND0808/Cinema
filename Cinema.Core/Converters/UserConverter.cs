using Cinema.Core.DTOs;
using Cinema.Core.IConverters;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace Cinema.Core.Converters
{
    public class UserConverter : IUserConverter
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

        public User ConvertToEntity(UserDTO userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentNullException(nameof(userDTO), "UserDTO cannot be null");
            }

            return new User
            {
                Id = userDTO.Id,
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                FullName = userDTO.FullName,
                DateOfBirth = userDTO.DateOfBirth,
                Gender = userDTO.Gender,
                AvatarUrl = userDTO.AvatarUrl,
                RankCustomerId = userDTO.RankCustomerId,
                UserStatusId = userDTO.UserStatusId,
                Point = userDTO.Point,
                isDeleted = userDTO.IsDeleted,
                PhoneNumber = userDTO.PhoneNumber
            };
        }
    }
}
