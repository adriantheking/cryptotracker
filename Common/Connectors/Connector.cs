using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Connectors
{
    public abstract class Connector
    {
        protected abstract void PrepareRequest(RestClient restClient);
    }
}
