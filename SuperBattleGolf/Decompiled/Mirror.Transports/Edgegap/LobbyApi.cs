using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Edgegap;

public class LobbyApi
{
	private struct CreateLobbyServiceRequest
	{
		public string name;
	}

	public struct LobbyServiceResponse
	{
		public string name;

		public string url;

		public string status;
	}

	[Header("Lobby Config")]
	public string LobbyUrl;

	public LobbyBrief[] Lobbies;

	public LobbyApi(string url)
	{
		LobbyUrl = url;
	}

	private static UnityWebRequest SendJson<T>(string url, T data, string method = "POST")
	{
		string s = JsonUtility.ToJson(data);
		UnityWebRequest unityWebRequest = new UnityWebRequest(url, method);
		unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		unityWebRequest.SetRequestHeader("Accept", "application/json");
		unityWebRequest.SetRequestHeader("Content-Type", "application/json");
		return unityWebRequest;
	}

	private static bool CheckErrorResponse(UnityWebRequest request, Action<string> onError)
	{
		if (request.result != UnityWebRequest.Result.Success && (request.result != UnityWebRequest.Result.ProtocolError || request.responseCode == 0L))
		{
			onError?.Invoke(request.error);
			return true;
		}
		if (request.responseCode < 200 || request.responseCode >= 300)
		{
			onError?.Invoke($"non-200 status code: {request.responseCode}. Body:\n {request.downloadHandler.text}");
			return true;
		}
		return false;
	}

	public void RefreshLobbies(Action<LobbyBrief[]> onLoaded, Action<string> onError)
	{
		UnityWebRequest request = UnityWebRequest.Get(LobbyUrl + "/lobbies");
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					ListLobbiesResponse listLobbiesResponse = JsonUtility.FromJson<ListLobbiesResponse>(request.downloadHandler.text);
					Lobbies = listLobbiesResponse.data;
					onLoaded?.Invoke(listLobbiesResponse.data);
				}
			}
		};
	}

	public void CreateLobby(LobbyCreateRequest createData, Action<Lobby> onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson(LobbyUrl + "/lobbies", createData);
		request.SetRequestHeader("Content-Type", "application/json");
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					Lobby obj = JsonUtility.FromJson<Lobby>(request.downloadHandler.text);
					onResponse?.Invoke(obj);
				}
			}
		};
	}

	public void UpdateLobby(string lobbyId, LobbyUpdateRequest updateData, Action<LobbyBrief> onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson(LobbyUrl + "/lobbies/" + lobbyId, updateData, "PATCH");
		request.SetRequestHeader("Content-Type", "application/json");
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					LobbyBrief obj = JsonUtility.FromJson<LobbyBrief>(request.downloadHandler.text);
					onResponse?.Invoke(obj);
				}
			}
		};
	}

	public void GetLobby(string lobbyId, Action<Lobby> onResponse, Action<string> onError)
	{
		UnityWebRequest request = UnityWebRequest.Get(LobbyUrl + "/lobbies/" + lobbyId);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					Lobby obj = JsonUtility.FromJson<Lobby>(request.downloadHandler.text);
					onResponse?.Invoke(obj);
				}
			}
		};
	}

	public void JoinLobby(LobbyJoinOrLeaveRequest data, Action onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson(LobbyUrl + "/lobbies:join", data);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					onResponse?.Invoke();
				}
			}
		};
	}

	public void LeaveLobby(LobbyJoinOrLeaveRequest data, Action onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson(LobbyUrl + "/lobbies:leave", data);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					onResponse?.Invoke();
				}
			}
		};
	}

	public void StartLobby(LobbyIdRequest data, Action onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson(LobbyUrl + "/lobbies:start", data);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					onResponse?.Invoke();
				}
			}
		};
	}

	public void DeleteLobby(string lobbyId, Action onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson(LobbyUrl + "/lobbies/" + lobbyId, "", "DELETE");
		request.SetRequestHeader("Content-Type", "application/json");
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					onResponse?.Invoke();
				}
			}
		};
	}

	public static void TrimApiKey(ref string apiKey)
	{
		if (apiKey != null)
		{
			if (apiKey.StartsWith("token "))
			{
				apiKey = apiKey.Substring("token ".Length);
			}
			apiKey = apiKey.Trim();
		}
	}

	public static void CreateAndDeployLobbyService(string apiKey, string name, Action<LobbyServiceResponse> onResponse, Action<string> onError)
	{
		TrimApiKey(ref apiKey);
		GetLobbyService(apiKey, name, delegate(LobbyServiceResponse? response)
		{
			if (!response.HasValue)
			{
				CreateLobbyService(apiKey, name, onResponse, onError);
			}
			else if (!string.IsNullOrEmpty(response.Value.url))
			{
				onResponse(response.Value);
			}
			else
			{
				DeployLobbyService(apiKey, name, onResponse, onError);
			}
		}, onError);
	}

	private static void CreateLobbyService(string apiKey, string name, Action<LobbyServiceResponse> onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson("https://api.edgegap.com/v1/lobbies", new CreateLobbyServiceRequest
		{
			name = name
		});
		request.SetRequestHeader("Authorization", "token " + apiKey);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					DeployLobbyService(apiKey, name, onResponse, onError);
				}
			}
		};
	}

	public static void GetLobbyService(string apiKey, string name, Action<LobbyServiceResponse?> onResponse, Action<string> onError)
	{
		TrimApiKey(ref apiKey);
		UnityWebRequest request = UnityWebRequest.Get("https://api.edgegap.com/v1/lobbies/" + name);
		request.SetRequestHeader("Authorization", "token " + apiKey);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (request.responseCode == 404)
				{
					onResponse(null);
				}
				else if (!CheckErrorResponse(request, onError))
				{
					LobbyServiceResponse value = JsonUtility.FromJson<LobbyServiceResponse>(request.downloadHandler.text);
					onResponse(value);
				}
			}
		};
	}

	public static void TerminateLobbyService(string apiKey, string name, Action<LobbyServiceResponse> onResponse, Action<string> onError)
	{
		TrimApiKey(ref apiKey);
		UnityWebRequest request = SendJson("https://api.edgegap.com/v1/lobbies:terminate", new CreateLobbyServiceRequest
		{
			name = name
		});
		request.SetRequestHeader("Authorization", "token " + apiKey);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					LobbyServiceResponse obj = JsonUtility.FromJson<LobbyServiceResponse>(request.downloadHandler.text);
					onResponse?.Invoke(obj);
				}
			}
		};
	}

	private static void DeployLobbyService(string apiKey, string name, Action<LobbyServiceResponse> onResponse, Action<string> onError)
	{
		UnityWebRequest request = SendJson("https://api.edgegap.com/v1/lobbies:deploy", new CreateLobbyServiceRequest
		{
			name = name
		});
		request.SetRequestHeader("Authorization", "token " + apiKey);
		request.SendWebRequest().completed += delegate
		{
			using (request)
			{
				if (!CheckErrorResponse(request, onError))
				{
					LobbyServiceResponse obj = JsonUtility.FromJson<LobbyServiceResponse>(request.downloadHandler.text);
					onResponse?.Invoke(obj);
				}
			}
		};
	}
}
