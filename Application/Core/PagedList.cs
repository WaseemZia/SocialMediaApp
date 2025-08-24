using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Application.Core
{
    public class PagedList<T,TCursor>
    {
        public List<T> Items { get; set; } = [];
        public TCursor? nextCursor { get; set; }
    }
}
