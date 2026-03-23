using UnityEngine;
using UnityEngine.SceneManagement;

public static class DevConsoleCmd
{
	[CCommand("quit", "", false, false, description = "Quits the game")]
	public static void Quit()
	{
		Application.Quit();
	}

	[CCommand("loadscene", "", false, false, description = "Force load any scene in the build")]
	public static void LoadScene(string scene)
	{
		SceneManager.LoadScene(scene);
	}
}
