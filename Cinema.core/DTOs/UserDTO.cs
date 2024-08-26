using System;
using BaseInsightDotNet.Commons.Enums;
using static BaseInsightDotNet.Commons.Enums.Enumerate;

namespace Cinema.Core.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Point { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string AvatarUrl { get; set; }
        public Guid RankCustomerId { get; set; }
        public Guid UserStatusId { get; set; }
        public bool IsDeleted { get; set; }
        public string PhoneNumber { get; set; }
    }
}
