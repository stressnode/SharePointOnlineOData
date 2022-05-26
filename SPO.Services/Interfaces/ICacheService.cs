namespace SPO.Services
{
	public interface ICacheService
	{
		object GetValue(string cacheKey);
		bool Add(string cacheKey, object value, int cacheExpirationHours = 24);
		void Delete(string cacheKey);
	}
}
