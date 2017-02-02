using System;
using Android.Support.CustomTabs;

namespace CustomTabsClientApplication
{
	/// <summary>
	/// A class that keeps tracks of the current {@link CustomTabsSession} and helps other components of
	/// the app to get access to the current session.
	/// </summary>
	public static class SessionHelper
	{
		static WeakReference<CustomTabsSession> sCurrentSession;

		/// <summary>
		/// @return The current {@link CustomTabsSession} object.
		/// </summary>
		/// <returns>The current session.</returns>
		public static CustomTabsSession GetCurrentSession()
		{
			CustomTabsSession customTabsSession = null;
			sCurrentSession?.TryGetTarget(out customTabsSession);
			return customTabsSession;
		}

		/// <summary>
		/// Sets the current session to the given one.
		/// </summary>
		/// <param name="session">Session.</param>
		public static void setCurrentSession(CustomTabsSession session)
		{
			sCurrentSession = new WeakReference<CustomTabsSession>(session);
		}
	}
}
