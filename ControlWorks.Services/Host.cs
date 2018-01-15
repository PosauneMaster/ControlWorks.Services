using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlWorks.Services.Rest;

namespace ControlWorks.Services
{
    public class Host
    {
        public void Start()
        {
            WebApiApplication.Start();
        }

        public void Stop()
        {
        }

    }
}
