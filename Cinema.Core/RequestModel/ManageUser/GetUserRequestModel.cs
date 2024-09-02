using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.RequestModel.ManageUser
{
    public class GetUserRequestModel
    {
        public Guid? RoleId { get; set; }

        public Guid? RankId { get; set; }

        public Guid? StatusId { get; set; }
    }
}
