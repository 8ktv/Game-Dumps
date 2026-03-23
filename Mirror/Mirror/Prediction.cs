using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

public static class Prediction
{
	public static bool Sample<T>(SortedList<double, T> history, double timestamp, out T before, out T after, out int afterIndex, out double t)
	{
		before = default(T);
		after = default(T);
		t = 0.0;
		afterIndex = -1;
		if (history.Count < 2)
		{
			return false;
		}
		if (timestamp < history.Keys[0])
		{
			return false;
		}
		int num = 0;
		KeyValuePair<double, T> keyValuePair = default(KeyValuePair<double, T>);
		for (int i = 0; i < history.Count; i++)
		{
			double num2 = history.Keys[i];
			T val = history.Values[i];
			if (timestamp == num2)
			{
				before = val;
				after = val;
				afterIndex = num;
				t = Mathd.InverseLerp(num2, num2, timestamp);
				return true;
			}
			if (num2 > timestamp)
			{
				before = keyValuePair.Value;
				after = val;
				afterIndex = num;
				t = Mathd.InverseLerp(keyValuePair.Key, num2, timestamp);
				return true;
			}
			keyValuePair = new KeyValuePair<double, T>(num2, val);
			num++;
		}
		return false;
	}

	public static T CorrectHistory<T>(SortedList<double, T> history, int stateHistoryLimit, T corrected, T before, T after, int afterIndex) where T : PredictedState
	{
		if (history.Count >= stateHistoryLimit)
		{
			history.RemoveAt(0);
			afterIndex--;
		}
		double num = after.timestamp - before.timestamp;
		double num2 = after.timestamp - corrected.timestamp;
		double num3 = ((num != 0.0) ? (num2 / num) : 0.0);
		Vector3 positionDelta = Vector3.Lerp(Vector3.zero, after.positionDelta, (float)num3);
		after.positionDelta = positionDelta;
		Vector3 velocityDelta = Vector3.Lerp(Vector3.zero, after.velocityDelta, (float)num3);
		after.velocityDelta = velocityDelta;
		Vector3 angularVelocityDelta = Vector3.Lerp(Vector3.zero, after.angularVelocityDelta, (float)num3);
		after.angularVelocityDelta = angularVelocityDelta;
		Quaternion normalized = Quaternion.Slerp(Quaternion.identity, after.rotationDelta, (float)num3).normalized;
		after.rotationDelta = normalized;
		history[after.timestamp] = after;
		T result = corrected;
		for (int i = afterIndex; i < history.Count; i++)
		{
			double key = history.Keys[i];
			T val = history.Values[i];
			Vector3 position = result.position + val.positionDelta;
			val.position = position;
			Vector3 velocity = result.velocity + val.velocityDelta;
			val.velocity = velocity;
			Vector3 angularVelocity = result.angularVelocity + val.angularVelocityDelta;
			val.angularVelocity = angularVelocity;
			Quaternion normalized2 = (val.rotationDelta * result.rotation).normalized;
			val.rotation = normalized2;
			history[key] = val;
			result = val;
		}
		return result;
	}
}
