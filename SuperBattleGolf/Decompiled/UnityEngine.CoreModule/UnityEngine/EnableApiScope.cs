using System;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Scripting/ApiRestrictions.h")]
internal readonly struct EnableApiScope : IDisposable
{
	private readonly ApiRestrictions.ContextRestrictions m_ContextApi;

	private readonly ApiRestrictions.GlobalRestrictions m_GlobalApi;

	private readonly Object m_Context;

	public EnableApiScope(ApiRestrictions.ContextRestrictions api, Object context)
	{
		m_ContextApi = api;
		m_Context = context;
		ApiRestrictions.PopDisableApi(api, context);
		m_GlobalApi = ApiRestrictions.GlobalRestrictions.GLOBALCOUNT;
	}

	public EnableApiScope(ApiRestrictions.GlobalRestrictions api)
	{
		m_GlobalApi = api;
		m_Context = null;
		ApiRestrictions.PopDisableApi(api);
		m_ContextApi = ApiRestrictions.ContextRestrictions.CONTEXTCOUNT;
	}

	public void Dispose()
	{
		if (m_Context != null)
		{
			ApiRestrictions.PushDisableApi(m_ContextApi, m_Context);
		}
		else
		{
			ApiRestrictions.PushDisableApi(m_GlobalApi);
		}
	}
}
