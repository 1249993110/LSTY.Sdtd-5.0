using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.WebApi.ViewModels
{
    public class ChatLogQueryParams : PaginationQueryParams
    {
        public string SteamId { get; set; }

    }
}
