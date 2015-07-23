namespace DSTHashRepository {
	public interface IHashRepositoryConfig {
		bool CheckCredentials(string user, string secret);
	}
}
