using Cinema.Core.DTOs;
using Cinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.IConverters
{
    public interface IUserConverter
    {
        UserDTO ConvertToDTO(User user);
        User ConvertToEntity(UserDTO userDTO);
    }
}
