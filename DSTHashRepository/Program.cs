using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Nancy.Session;
using Nancy.TinyIoc;

namespace DSTHashRepository {
	public class Program {
		public static void Main(string[] args) {
			string adminSecret;
			if (args.Length > 0) {
				adminSecret = args.First();
			} else {
				adminSecret = null;//Make admin inaccessible
			}
			Console.WriteLine("Admin access {0}abled.", adminSecret != null ? "en" : "di");
			var bindUri = new Uri("http://localhost:3000");
			var bootstrapper = new Bootstrapper(adminSecret);
			var hostConfiguration = new HostConfiguration {
				RewriteLocalhost = true,
			};
			
			using (var host = new NancyHost(baseUri: bindUri, bootstrapper: bootstrapper, configuration: hostConfiguration)) {
				host.Start();
				Console.WriteLine("Host running...");
				while (Console.ReadKey(true).KeyChar != 'q') {
					Thread.Sleep(250);
				};
			}
		}
	}
}
