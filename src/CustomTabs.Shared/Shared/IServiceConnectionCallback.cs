using Android.Support.CustomTabs;

namespace Android.Support.CustomTabs.Shared
{
	/// <summary>
	/// Callback for events when connecting and disconnecting from Custom Tabs Service.
	/// </summary>
	public interface IServiceConnectionCallback
	{
		/// <summary> 
		/// Called when the service is connected.
		/// </summary>
		/// <param name="client">Client.</param>
		void OnServiceConnected(CustomTabsClient client);

		/// <summary>
		///Called when the service is disconnected.
		/// </summary>
		void OnServiceDisconnected();
	}
}
