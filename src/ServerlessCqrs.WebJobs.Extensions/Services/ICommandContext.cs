namespace ServerlessCqrs.WebJobs.Extensions.Services
{
	public interface ICommandContext
	{
		ISession GetSession();
	}
}
