using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Session;
using Nancy.TinyIoc;

namespace DSTHashRepository {
	public class Bootstrapper : DefaultNancyBootstrapper {
		private readonly string adminSecret;
		public Bootstrapper(string adminSecret) {
			this.adminSecret = adminSecret;
		}
		protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
			base.ApplicationStartup(container, pipelines);
			CookieBasedSessions.Enable(pipelines);
			var hashRepositoryConfig = new HashRepositoryConfig(adminSecret: adminSecret) {
			};
			container.Register<IHashRepositoryConfig>(hashRepositoryConfig);
		}

		private static void SessionStart(ISession session) {
			//session start code here
		}

		private static void SessionEnd(ISession session) {
			//session end code here
		}
	}
}
