using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening.Plugins;

public class QuaternionPlugin : ABSTweenPlugin<Quaternion, Vector3, QuaternionOptions>
{
	public override void Reset(TweenerCore<Quaternion, Vector3, QuaternionOptions> t)
	{
	}

	public override void SetFrom(TweenerCore<Quaternion, Vector3, QuaternionOptions> t, bool isRelative)
	{
		Vector3 endValue = t.endValue;
		t.endValue = t.getter().eulerAngles;
		if (t.plugOptions.rotateMode == RotateMode.Fast && !t.isRelative)
		{
			t.startValue = GetEulerValForCalculations(t, endValue, t.endValue);
		}
		else if (t.plugOptions.rotateMode == RotateMode.FastBeyond360)
		{
			t.startValue = GetEulerValForCalculations(t, t.endValue + endValue, t.endValue);
		}
		else
		{
			Quaternion quaternion = t.getter();
			if (t.plugOptions.rotateMode == RotateMode.WorldAxisAdd)
			{
				t.startValue = (quaternion * Quaternion.Inverse(quaternion) * Quaternion.Euler(endValue) * quaternion).eulerAngles;
			}
			else
			{
				t.startValue = (quaternion * Quaternion.Euler(endValue)).eulerAngles;
			}
			t.endValue = -endValue;
		}
		t.setter(Quaternion.Euler(t.startValue));
	}

	public override void SetFrom(TweenerCore<Quaternion, Vector3, QuaternionOptions> t, Vector3 fromValue, bool setImmediately, bool isRelative)
	{
		if (isRelative)
		{
			Vector3 eulerAngles = t.getter().eulerAngles;
			t.endValue += eulerAngles;
			fromValue += eulerAngles;
		}
		t.startValue = GetEulerValForCalculations(t, fromValue, t.endValue);
		if (setImmediately)
		{
			t.setter(Quaternion.Euler(fromValue));
		}
	}

	public override Vector3 ConvertToStartValue(TweenerCore<Quaternion, Vector3, QuaternionOptions> t, Quaternion value)
	{
		return value.eulerAngles;
	}

	public override void SetRelativeEndValue(TweenerCore<Quaternion, Vector3, QuaternionOptions> t)
	{
		t.endValue += t.startValue;
	}

	public override void SetChangeValue(TweenerCore<Quaternion, Vector3, QuaternionOptions> t)
	{
		Vector3 vector = (t.isFrom ? t.endValue : GetEulerValForCalculations(t, t.endValue, t.startValue));
		Vector3 startValue = t.startValue;
		if (t.plugOptions.rotateMode == RotateMode.Fast && !t.isRelative)
		{
			if (vector.x > 360f || vector.x < 360f)
			{
				vector.x %= 360f;
			}
			if (vector.y > 360f || vector.y < 360f)
			{
				vector.y %= 360f;
			}
			if (vector.z > 360f || vector.z < 360f)
			{
				vector.z %= 360f;
			}
			Vector3 changeValue = vector - startValue;
			float num = ((changeValue.x > 0f) ? changeValue.x : (0f - changeValue.x));
			if (num > 180f)
			{
				changeValue.x = ((changeValue.x > 0f) ? (0f - (360f - num)) : (360f - num));
			}
			num = ((changeValue.y > 0f) ? changeValue.y : (0f - changeValue.y));
			if (num > 180f)
			{
				changeValue.y = ((changeValue.y > 0f) ? (0f - (360f - num)) : (360f - num));
			}
			num = ((changeValue.z > 0f) ? changeValue.z : (0f - changeValue.z));
			if (num > 180f)
			{
				changeValue.z = ((changeValue.z > 0f) ? (0f - (360f - num)) : (360f - num));
			}
			t.changeValue = changeValue;
		}
		else if (t.plugOptions.rotateMode == RotateMode.FastBeyond360 || t.isRelative)
		{
			t.changeValue = vector - startValue;
		}
		else
		{
			t.changeValue = vector;
		}
	}

	public override float GetSpeedBasedDuration(QuaternionOptions options, float unitsXSecond, Vector3 changeValue)
	{
		return changeValue.magnitude / unitsXSecond;
	}

	public override void EvaluateAndApply(QuaternionOptions options, Tween t, bool isRelative, DOGetter<Quaternion> getter, DOSetter<Quaternion> setter, float elapsed, Vector3 startValue, Vector3 changeValue, float duration, bool usingInversePosition, int newCompletedSteps, UpdateNotice updateNotice)
	{
		if (options.dynamicLookAt)
		{
			TweenerCore<Quaternion, Vector3, QuaternionOptions> tweenerCore = (TweenerCore<Quaternion, Vector3, QuaternionOptions>)t;
			tweenerCore.endValue = options.dynamicLookAtWorldPosition;
			SpecialPluginsUtils.SetLookAt(tweenerCore);
			SetChangeValue(tweenerCore);
			changeValue = tweenerCore.changeValue;
		}
		Vector3 euler = startValue;
		if (t.loopType == LoopType.Incremental)
		{
			euler += changeValue * (t.isComplete ? (t.completedLoops - 1) : t.completedLoops);
		}
		if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
		{
			euler += changeValue * ((t.loopType != LoopType.Incremental) ? 1 : t.loops) * (t.sequenceParent.isComplete ? (t.sequenceParent.completedLoops - 1) : t.sequenceParent.completedLoops);
		}
		float num = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
		RotateMode rotateMode = options.rotateMode;
		if ((uint)(rotateMode - 2) <= 1u)
		{
			Quaternion quaternion = Quaternion.Euler(startValue);
			euler.x = changeValue.x * num;
			euler.y = changeValue.y * num;
			euler.z = changeValue.z * num;
			if (options.rotateMode == RotateMode.WorldAxisAdd)
			{
				setter(quaternion * Quaternion.Inverse(quaternion) * Quaternion.Euler(euler) * quaternion);
			}
			else
			{
				setter(quaternion * Quaternion.Euler(euler));
			}
		}
		else
		{
			euler.x += changeValue.x * num;
			euler.y += changeValue.y * num;
			euler.z += changeValue.z * num;
			setter(Quaternion.Euler(euler));
		}
	}

	private Vector3 GetEulerValForCalculations(TweenerCore<Quaternion, Vector3, QuaternionOptions> t, Vector3 val, Vector3 counterVal)
	{
		if (t.isRelative)
		{
			return val;
		}
		RotateMode rotateMode = t.plugOptions.rotateMode;
		if ((uint)(rotateMode - 2) <= 1u)
		{
			return val;
		}
		Vector3 result = FlipEulerAngles(val);
		bool flag = Mathf.Approximately(counterVal.x, val.x);
		bool flag2 = Mathf.Approximately(counterVal.x, result.x);
		bool flag3 = Mathf.Approximately(counterVal.y, val.y);
		bool flag4 = Mathf.Approximately(counterVal.y, result.y);
		bool flag5 = Mathf.Approximately(counterVal.z, val.z);
		bool flag6 = Mathf.Approximately(counterVal.z, result.z);
		bool flag7 = (flag && (flag3 || flag5)) || (flag3 && flag5);
		bool flag8 = (!flag7 && flag2 && (flag4 || flag6)) || (flag4 && flag6);
		if (!flag7 && !flag8)
		{
			return val;
		}
		int num = 0;
		num = ((!flag7) ? (flag2 ? ((!flag4) ? 1 : 2) : 0) : (flag ? ((!flag3) ? 1 : 2) : 0));
		bool flag9 = false;
		switch (num)
		{
		case 0:
			flag9 = !Mathf.Approximately(counterVal.y, val.y) || !Mathf.Approximately(counterVal.z, val.z);
			break;
		case 1:
			flag9 = !Mathf.Approximately(counterVal.x, val.x) || !Mathf.Approximately(counterVal.z, val.z);
			break;
		case 2:
			flag9 = !Mathf.Approximately(counterVal.x, val.x) || !Mathf.Approximately(counterVal.y, val.y);
			break;
		}
		if (!flag9)
		{
			return val;
		}
		return result;
	}

	private Vector3 FlipEulerAngles(Vector3 euler)
	{
		return new Vector3(180f - euler.x, euler.y + 180f, euler.z + 180f);
	}
}
