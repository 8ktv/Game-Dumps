using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.ResourceManagement.AsyncOperations;

internal class GetDownloadSizeOperation : AsyncOperationBase<long>
{
	private IEnumerable<IResourceLocation> m_Locations;

	private bool m_Started;

	public void Init(IEnumerable<IResourceLocation> locations, ResourceManager resourceManager)
	{
		m_Locations = locations;
		m_RM = resourceManager;
		m_Started = false;
	}

	private void Calculate()
	{
		if (m_Started)
		{
			return;
		}
		m_Started = true;
		long num = 0L;
		try
		{
			foreach (IResourceLocation location in m_Locations)
			{
				if (location.Data is ILocationSizeData locationSizeData)
				{
					num += locationSizeData.ComputeSize(location, m_RM);
				}
			}
		}
		catch (Exception ex)
		{
			Complete(0L, success: false, "Error calculating download size: " + ex.ToString());
			return;
		}
		Complete(num, success: true, "");
	}

	protected override void Execute()
	{
		Calculate();
	}

	protected override bool InvokeWaitForCompletion()
	{
		Calculate();
		return true;
	}
}
