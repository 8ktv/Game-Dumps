namespace UnityEngine;

public enum SimulationStage : ushort
{
	None = 0,
	PrepareSimulation = 1,
	RunSimulation = 2,
	PublishSimulationResults = 4,
	All = 7
}
