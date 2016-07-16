using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if VELOX_DB
namespace Velox.DB.Core
#else
namespace Velox.Core
#endif
{
    public class PreserveAttribute : System.Attribute
    {
        public bool Conditional { get; set; }
    }
}
