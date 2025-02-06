# Traffic Light & Pedestrian Crossing State Machine Demo
## Project Overview
This C# console application is designed to help learners understand the workings of a finite state machine (FSM) towards applications for human-machine interfaces (HMIs) like PLCs and hardware I/O systems. 
Through this demo, learners can explore state transitions in a traffic light system with pedestrian interrupts and gain foundational knowledge that will assist in handling future automation projects.

## Features
- State Transitions: The traffic light system follows a specific sequence:
  - North-South directions go straight.
  - North-South directions transition to right turns.
  - East-West directions go straight.
  - East-West directions transition to right turns.
  - The cycle repeats.
- Pedestrian Interrupts: The system allows for pedestrian requests to interrupt the state transitions. For example, while North-South is going straight, a pedestrian request can quickly transition the state to East-West straight, prioritizing the opposite direction.

## Usage Instructions
### Cloning the Repository
To clone this repository to your local machine, run:
```bash
git clone https://github.com/dvwl/fsm-demo.git
```

### Running the Application
Ensure you have .NET 8 installed on your system.
Navigate to the project directory and run the following command to start the demo:
```bash
dotnet run
```

### Experimenting with the Demo
To observe the state machine without pedestrian interrupts:
- Open the `TrafficSystem` class.
- Comment out `Task.Run(SimulatePedestrianRequests).GetAwaiter();` that handle pedestrian requests.
- Run the application to see the state transitions without interruptions.

## Code Details
The application is built using object-oriented programming principles in C# for .NET 8. It uses a finite state machine to simulate traffic light behavior and handle pedestrian interrupts, with clear separation of concerns between traffic light states, pedestrian requests, and state transitions.

## License
This project is licensed under the MIT License.

## Future Enhancements
- Dynamic Intersection Allocation: Future versions may support dynamically adding intersections by reading from a configuration file or command-line arguments.
- I encourage contributions! Feel free to submit pull requests for bug fixes, improvements, or new features.
