using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;

using System.ServiceModel;
using WcfDummyController;

namespace HttpDummySender
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("APP> launch");

            Console.WriteLine("APP> starting wcf service");
            Host = new ServiceHost(typeof(VisualizationController));
            Host.AddServiceEndpoint(typeof(IVisualizationController), new BasicHttpBinding(), "http://localhost:60042/visualizationController");
            Host.Open();
            Console.WriteLine("APP> shutdown");
        }

        static ServiceHost Host;
    }
}
