namespace DSTHashRepository {
	public class HashRepositoryConfig : IHashRepositoryConfig {
		private string adminSecret;

		public HashRepositoryConfig(string adminSecret) {
			this.adminSecret = adminSecret;
		}

		public bool CheckCredentials(string user, string secret) {
			if (adminSecret == null) {
				return false;//Allow no-admin configuration
			}
			return secret == adminSecret;
		}
	}
}
