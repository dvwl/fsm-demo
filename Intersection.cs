namespace fsm_demo;

public class Intersection(string name)
{
	public string Name { get; } = name;
	public TrafficLightState StraightState { get; private set; } = TrafficLightState.Red;
	public TrafficLightState RightTurnState { get; private set; } = TrafficLightState.Red;
	public bool PedestrianWaiting { get; private set; }
	public event Action<string>? PedestrianRequest;

	public void PressPedestrianButton()
	{
		if (StraightState == TrafficLightState.Red && RightTurnState == TrafficLightState.Red) return; // Pedestrian already crossed
		PedestrianWaiting = true;
		PedestrianRequest?.Invoke(Name);
	}

	public void HandledRequest()
	{
		PedestrianWaiting = false;
	}

	public void UpdateState(TrafficLightState straight, TrafficLightState rightTurn)
	{
		StraightState = straight;
		RightTurnState = rightTurn;
	}
}