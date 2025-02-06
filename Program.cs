namespace fsm_demo;

using System.Threading.Tasks;

public class Program
{
	public static async Task Main(string[] args)
	{
		TrafficSystem trafficSystem = new();
		await trafficSystem.StartAsync();
	}
}
