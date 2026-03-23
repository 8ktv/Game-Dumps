using System;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Scripting/ApiRestrictions.h")]
internal readonly struct DisableApiScope : IDisposable
{
	private readonly ApiRestrictions.ContextRestrictions m_ContextApi;

	private readonly ApiRestrictions.GlobalRestrictions m_GlobalApi;

	private readonly Object m_Context;

	public DisableApiScope(ApiRestrictions.ContextRestrictions api, Object context)
	{
		m_ContextApi = api;
		m_Context = context;
		ApiRestrictions.PushDisableApi(api, context);
		m_GlobalApi = ApiRestrictions.GlobalRestrictions.GLOBALCOUNT;
	}

	public DisableApiScope(ApiRestrictions.GlobalRestrictions api)
	{
		m_GlobalApi = api;
		m_Context = null;
		ApiRestrictions.PushDisableApi(api);
		m_ContextApi = ApiRestrictions.ContextRestrictions.CONTEXTCOUNT;
	}

	public void Dispose()
	{
		if (m_Context != null)
		{
			ApiRestrictions.PopDisableApi(m_ContextApi, m_Context);
		}
		else
		{
			ApiRestrictions.PopDisableApi(m_GlobalApi);
		}
	}
}
