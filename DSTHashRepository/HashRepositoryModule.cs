using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace DSTHashRepository {
	public class HashRepositoryModule : Nancy.NancyModule {
		public HashRepositoryModule() {
			Get["/"] = _ => "DST Hash Repository Prototype";
			Get["/{hash:minLength(1)}"] = _ => {
				var hash = String.Concat(((string)_.hash).Where(c => c != '/' && c != '.' && c != '\\'));//no navigation characters
				return Response.FromStream(File.OpenRead(Path.Combine(".", "files", hash)), "application/binary");
			};
			Put["/{hash:minLength(1)}", ctxt => ctxt.Request.Session["isAdmin"] == (object)true] = _ => {
				//TODO: allow upload of files if isAdmin
				return HttpStatusCode.Forbidden;
			};
			Get["/auth"] = _ => {
				var password = (string)Request.Query.password;
				if (String.IsNullOrWhiteSpace(password) || password != "testingPassword") {//TODO: Allow changing password via config
					return HttpStatusCode.Forbidden;
				}
				//Session["isAdmin"] = true;//TODO: Enable when a system for allowing the password to be changed exists.
				return HttpStatusCode.OK;
			};
		}
	}
}
