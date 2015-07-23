using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace DSTHashRepository {
	public class HashRepositoryModule : Nancy.NancyModule {
		private string ToHashName(string rawHash) => String.Concat(rawHash.Where(c => c != '/' && c != '.' && c != '\\'));//no navigation characters
		private string FilesDir => Path.Combine(".", "files");
		private string ToFilePath(string hashName) => Path.Combine(FilesDir, hashName);
		private string ToHashPath(string rawHash) => ToFilePath(ToHashName(rawHash));
		public HashRepositoryModule(IHashRepositoryConfig config) {
			if (!Directory.Exists(FilesDir)) {
				Directory.CreateDirectory(FilesDir);
			}
			Get["/"] = _ => "DST Hash Repository Prototype";
			Get["/{hash:minLength(4)}"] = _ => {
				Console.WriteLine("Serving request on {0}", _.hash);
                return Response.FromStream(File.OpenRead(ToHashPath((string)_.hash)), "application/binary");
			};
			Put["/{hash:minLength(4)}", true] = async (_, ct) => {
				var secretHeader = Request.Headers["secret"].FirstOrDefault();
                if (!config.CheckCredentials("", secretHeader)) {
					Console.WriteLine("Failed to authenticate on {0}", _.hash);
					return HttpStatusCode.Forbidden;
				}
				Console.WriteLine("Secret accepted on {0}", _.hash);
				var hashPath = ToHashPath((string)_.hash);
				using (var destFile = File.OpenWrite(hashPath)) {
					await Request.Body.CopyToAsync(destFile);
				}
				return HttpStatusCode.OK;
			};
		}
	}
}
