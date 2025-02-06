namespace fsm_demo;

public class TrafficSystem
{
	private List<Intersection> intersections;
	private TrafficSystemState currentState;
	private readonly object lockObject = new();
	private static readonly Dictionary<TrafficLightState, ConsoleColor> StateColorMap = new()
	{
		{ TrafficLightState.Green, ConsoleColor.Green },
		{ TrafficLightState.Yellow, ConsoleColor.Yellow },
		{ TrafficLightState.Red, ConsoleColor.Red }
	};

	public TrafficSystem()
	{
		intersections =
		[
			new Intersection("North"),
			new Intersection("East"),
			new Intersection("South"),
			new Intersection("West")
		];

		foreach (var intersection in intersections)
		{
			intersection.PedestrianRequest += async (name) => await HandlePedestrianRequestAsync(name);
		}

		currentState = TrafficSystemState.NorthSouthStraight;
	}

	public async Task StartAsync()
	{
		Task.Run(SimulatePedestrianRequests).GetAwaiter();

		while (true)
		{
			await TransitionToNextStateAsync();
		}
	}

	private async Task TransitionToNextStateAsync()
	{
		Intersection? withPedestrian = intersections.Where(i => i.PedestrianWaiting == true).FirstOrDefault();

		(TrafficSystemState newState, int idx1, int idx2, TrafficLightState straight, TrafficLightState turn, int delay) transition = currentState switch
		{
			TrafficSystemState.NorthSouthStraight => (TrafficSystemState.NorthSouthStraightYellow, 0, 2, TrafficLightState.Green, TrafficLightState.Red, 5000),
			TrafficSystemState.NorthSouthStraightYellow => (withPedestrian?.Name == "North" || withPedestrian?.Name == "South" ? TrafficSystemState.EastWestStraight : TrafficSystemState.NorthSouthRightTurn, 0, 2, TrafficLightState.Yellow, TrafficLightState.Red, 2000),
			TrafficSystemState.NorthSouthRightTurn => (TrafficSystemState.NorthSouthRightTurnYellow, 0, 2, TrafficLightState.Red, TrafficLightState.Green, 5000),
			TrafficSystemState.NorthSouthRightTurnYellow => (TrafficSystemState.EastWestStraight, 0, 2, TrafficLightState.Red, TrafficLightState.Yellow, 2000),
			TrafficSystemState.EastWestStraight => (TrafficSystemState.EastWestStraightYellow, 1, 3, TrafficLightState.Green, TrafficLightState.Red, 5000),
			TrafficSystemState.EastWestStraightYellow => (withPedestrian?.Name == "East" || withPedestrian?.Name == "West" ? TrafficSystemState.NorthSouthStraight : TrafficSystemState.EastWestRightTurn, 1, 3, TrafficLightState.Yellow, TrafficLightState.Red, 2000),
			TrafficSystemState.EastWestRightTurn => (TrafficSystemState.EastWestRightTurnYellow, 1, 3, TrafficLightState.Red, TrafficLightState.Green, 5000),
			TrafficSystemState.EastWestRightTurnYellow => (TrafficSystemState.NorthSouthStraight, 1, 3, TrafficLightState.Red, TrafficLightState.Yellow, 2000),
			_ => throw new InvalidOperationException("Invalid state transition")
		};

		await SetStateAsync(transition.idx1, transition.idx2, transition.straight, transition.turn, transition.delay);
		currentState = transition.newState;

		if (withPedestrian != null)
		{
			// Notify intersection that we handled the request
			withPedestrian.HandledRequest();
		}
	}

	private async Task SetStateAsync(int i1, int i2, TrafficLightState straight, TrafficLightState rightTurn, int delay)
	{
		intersections[i1].UpdateState(straight, rightTurn);
		intersections[i2].UpdateState(straight, rightTurn);
		foreach(Intersection intersection in intersections.Where(i => i.Name != intersections[i1].Name && i.Name != intersections[i2].Name))
		{
			intersection.UpdateState(TrafficLightState.Red, TrafficLightState.Red);
		}
		LogState(intersections);
		await Task.Delay(delay);
	}

	private async Task HandlePedestrianRequestAsync(string intersectionName)
	{
		var intersection = intersections.FirstOrDefault(i => i.Name == intersectionName);
		if (intersection == null || 
			(intersection.StraightState == TrafficLightState.Red && intersection.RightTurnState == TrafficLightState.Red))
			return;

		Console.WriteLine($"{intersection.Name}: Pedestrian pressed button!");

		// Wait for the state machine to process the request
		while (intersection.PedestrianWaiting)
		{
			await Task.Delay(500);
		}

		Console.WriteLine($"{intersection.Name}: Pedestrian crossed!");
	}

	private void LogState(IReadOnlyList<Intersection> intersections)
	{
		ConsoleColor originalColor = Console.ForegroundColor;

		for (int i = 0; i < intersections.Count / 2; i++)
		{
			if (i + 2 < intersections.Count) // Ensure there's a pair
			{
				Console.ForegroundColor = originalColor;
				Console.Write($"{intersections[i].Name}-{intersections[i + 2].Name}: Straight ");
				LogTrafficLightState(intersections[i].StraightState);

				Console.ForegroundColor = originalColor;
				Console.Write(", Right Turn ");
				LogTrafficLightState(intersections[i].RightTurnState);
				Console.WriteLine();
			}
		}

		Console.ForegroundColor = originalColor;
		Console.WriteLine("---");
	}

	// Helper method to log traffic light state
	private void LogTrafficLightState(TrafficLightState state)
	{
		Console.ForegroundColor = StateColorMap.GetValueOrDefault(state, ConsoleColor.White);
		Console.Write(state.ToString());
	}

	private void SimulatePedestrianRequests()
	{
		Random random = new();
		while (true)
		{
			Thread.Sleep(random.Next(3, 8) * 1000);
			int index = random.Next(0, intersections.Count);
			lock (lockObject)
			{
				intersections[index].PressPedestrianButton();
			}
		}
	}
}
