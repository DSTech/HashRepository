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
			Head["/{hash:minLength(4)}"] = _ => {
				var hashPath = ToHashPath((string)_.hash);
				Console.WriteLine("HEAD: {0}", hashPath);
				if (!File.Exists(hashPath)) {
					return HttpStatusCode.NotFound;
				}
				return HttpStatusCode.OK;
			};
			Get["/{hash:minLength(4)}"] = _ => {
				var hashPath = ToHashPath((string)_.hash);
				Console.WriteLine("GET: {0}", hashPath);
				if (!File.Exists(hashPath)) {
					return HttpStatusCode.NotFound;
				}
				return Response.FromStream(File.OpenRead(hashPath), "application/binary");
			};
			Put["/{hash:minLength(4)}", true] = async (_, ct) => {
				var hashPath = ToHashPath((string)_.hash);
				var secretHeader = Request.Headers["secret"].FirstOrDefault();
				if (!config.CheckCredentials("", secretHeader)) {
					Console.WriteLine("PUT|AUTHFAIL: {0}", hashPath);
					return HttpStatusCode.Forbidden;
				}
				Console.WriteLine("PUT: {0}", hashPath);
				using (var destFile = File.OpenWrite(hashPath)) {
					await Request.Body.CopyToAsync(destFile);
				}
				return HttpStatusCode.OK;
			};
			Delete["/{hash:minLength(4)}"] = _ => {
				var hashPath = ToHashPath((string)_.hash);
				var secretHeader = Request.Headers["secret"].FirstOrDefault();
				if (!config.CheckCredentials("", secretHeader)) {
					Console.WriteLine("DELETE|AUTHFAIL: {0}", hashPath);
					return HttpStatusCode.Forbidden;
				}
				Console.WriteLine("DELETE: {0}", hashPath);
				if (!File.Exists(hashPath)) {
					return HttpStatusCode.NotFound;
				}
				File.Delete(hashPath);
				return HttpStatusCode.OK;
			};
		}
	}
}
