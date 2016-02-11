using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Nancy;
using Caching;

namespace DSTHashRepository {
	public class HashRepositoryModule : Nancy.NancyModule {
	    private LRUCache<string, string> HashCache = new LRUCache<string, string>(1024, 24);
		private string ToHashName(string rawHash) => String.Concat(rawHash.Where(c => c != '/' && c != '.' && c != '\\'));//no navigation characters
		private string FilesDir => Path.Combine(".", "files");
		private string ToFilePath(string hashName) => Path.Combine(FilesDir, hashName);

        private string ToHashPath(string rawHash) {
            string hex;
            if (HashCache.TryGetValue(rawHash, out hex)) {
                return hex;
            }
            byte[] hashHash;
            using (var sha2 = new SHA256Managed()) {
                hashHash = sha2.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
            }
            hex = BytesToHex(hashHash);
            HashCache.Add(rawHash, hex);
            return hex;
        }

        public static string BytesToHex(byte[] bytes) {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public HashRepositoryModule(IHashRepositoryConfig config) {
			if (!Directory.Exists(FilesDir)) {
				Directory.CreateDirectory(FilesDir);
			}
			Get["/"] = _ => "DST Hash Repository Prototype";
			Head["/{hash*}"] = _ => {
				var hashPath = ToHashPath((string)_.hash);
				Console.WriteLine("HEAD: {0}", hashPath);
				if (!File.Exists(hashPath)) {
					return HttpStatusCode.NotFound;
				}
				return HttpStatusCode.OK;
			};
			Get["/{hash*}"] = _ => {
				var hashPath = ToHashPath((string)_.hash);
				Console.WriteLine("GET: {0}", hashPath);
				if (!File.Exists(hashPath)) {
					return HttpStatusCode.NotFound;
				}
				return Response.FromStream(File.OpenRead(hashPath), "application/binary");
			};
			Put["/{hash*}", true] = async (_, ct) => {
				var hashPath = ToHashPath((string)_.hash);
				var secretHeader = Request.Headers["secret"].FirstOrDefault();
				if (!config.CheckCredentials("", secretHeader)) {
					Console.WriteLine("PUT|AUTHFAIL: {0}", hashPath);
					return HttpStatusCode.Forbidden;
				}
				Console.WriteLine("PUT: {0}", hashPath);
				using (var destFile = File.Create(hashPath)) {
					await Request.Body.CopyToAsync(destFile);
				}
				return HttpStatusCode.OK;
			};
			Delete["/{hash*}"] = _ => {
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
