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
        /// <summary>
        /// Method should be responsible for preparing request headers  
        /// </summary>
        /// <param name="restClient"></param>
        protected abstract void PrepareHeaders(RestClient restClient);
    }
}
