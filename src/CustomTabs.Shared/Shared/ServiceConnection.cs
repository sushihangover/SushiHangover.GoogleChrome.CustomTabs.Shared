using System;
using Android.Content;

namespace Android.Support.CustomTabs.Shared
{
	/// <summary>
	/// Implementation for the CustomTabsServiceConnection that avoids leaking the ServiceConnectionCallback
	/// </summary>
	public class ServiceConnection : CustomTabsServiceConnection
	{
		// A weak reference to the ServiceConnectionCallback to avoid leaking it.
		readonly WeakReference<IServiceConnectionCallback> mConnectionCallback;

		[Preserve(Conditional = true)]
		public ServiceConnection(IServiceConnectionCallback connectionCallback)
		{
			mConnectionCallback = new WeakReference<IServiceConnectionCallback>(connectionCallback);
		}

		[Preserve(Conditional = true)]
		public override void OnCustomTabsServiceConnected(ComponentName name, CustomTabsClient client)
		{
			IServiceConnectionCallback connectionCallback;
			if (mConnectionCallback.TryGetTarget(out connectionCallback))
				connectionCallback?.OnServiceConnected(client);
		}

		[Preserve(Conditional = true)]
		public override void OnServiceDisconnected(ComponentName name)
		{
			IServiceConnectionCallback connectionCallback;
			if (mConnectionCallback.TryGetTarget(out connectionCallback))
				connectionCallback?.OnServiceDisconnected();
		}
	}
}
