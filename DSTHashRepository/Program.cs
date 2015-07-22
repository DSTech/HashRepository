using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Session;
using Nancy.TinyIoc;

namespace DSTHashRepository {
	public class Program {
		public static void Main(string[] args) {
			var bootstrapper = new Bootstrapper();
			using (var host = new Nancy.Hosting.Self.NancyHost(new Uri("http://localhost:3000"), bootstrapper: bootstrapper, configuration: new Nancy.Hosting.Self.HostConfiguration {
				RewriteLocalhost = true,
			})) {
				host.Start();
				while (Console.ReadKey(true).KeyChar != 'q') {
					Thread.Sleep(250);
				};
			}
		}
	}
}
