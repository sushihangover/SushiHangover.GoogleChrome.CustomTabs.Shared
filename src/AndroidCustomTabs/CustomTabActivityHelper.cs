using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.CustomTabs;
using Android.Support.CustomTabs.Shared;
using CustomTabsHelper = Android.Support.CustomTabs.Shared.CustomTabsHelper;
using Uri = Android.Net.Uri;

namespace AndroidCustomTabs
{
	public class CustomTabActivityHelper : Java.Lang.Object, IServiceConnectionCallback
	{
		CustomTabsSession mCustomTabsSession;
		CustomTabsClient mClient;
		CustomTabsServiceConnection mConnection;
		IConnectionCallback mConnectionCallback;

		public static void OpenCustomTab(Activity activity,
									 CustomTabsIntent customTabsIntent,
									 Uri uri,
									 ICustomTabFallback fallback)
		{
			var packageName = CustomTabsHelper.GetPackageNameToUse(activity);

			//If we cant find a package name, it means theres no browser that supports
			//Chrome Custom Tabs installed. So, we fallback to the webview
			if (packageName == null)
			{
				fallback?.OpenUri(activity, uri);
			}
			else
			{
				customTabsIntent.Intent.SetPackage(packageName);
				customTabsIntent.LaunchUrl(activity, uri);
			}
		}

		public void UnbindCustomTabsService(Activity activity)
		{
			if (mConnection == null) return;
			activity.UnbindService(mConnection);
			mClient = null;
			mCustomTabsSession = null;
			mConnection = null;
		}

		public CustomTabsSession GetSession()
		{
			if (mClient == null)
			{
				mCustomTabsSession = null;
			}
			else if (mCustomTabsSession == null)
			{
				mCustomTabsSession = mClient.NewSession(null as CustomTabsCallback);
			}
			return mCustomTabsSession;
		}

		public void SetConnectionCallback(IConnectionCallback connectionCallback)
		{
			mConnectionCallback = connectionCallback;
		}

		public void BindCustomTabsService(Activity activity)
		{
			if (mClient != null)
				return;

			var packageName = CustomTabsHelper.GetPackageNameToUse(activity);
			if (packageName == null)
				return;

			mConnection = new ServiceConnection(this);
			CustomTabsClient.BindCustomTabsService(activity, packageName, mConnection);
		}

		public bool MayLaunchUrl(Uri uri, Bundle extras, List<Bundle> otherLikelyBundles)
		{
			if (mClient == null)
				return false;

			var session = GetSession();
			if (session == null)
				return false;

			return session.MayLaunchUrl(uri, extras, otherLikelyBundles);
		}

		public void OnServiceConnected(CustomTabsClient client)
		{
			mClient = client;
			mClient.Warmup(0L);
			mConnectionCallback?.OnCustomTabsConnected();
		}

		public void OnServiceDisconnected()
		{
			mClient = null;
			mCustomTabsSession = null;
			mConnectionCallback?.OnCustomTabsDisconnected();
		}

		/// <summary>
		///  A Callback for when the service is connected or disconnected. Use those callbacks to
		///  handle UI changes when the service is connected or disconnected.
		/// </summary>
		public interface IConnectionCallback
		{
			/**
			 * Called when the service is connected.
			 */
			void OnCustomTabsConnected();

			/**
			 * Called when the service is disconnected.
			 */
			void OnCustomTabsDisconnected();
		}

		/// <summary>
		/// To be used as a fallback to open the Uri when Custom Tabs is not available.
		/// </summary>
		public interface ICustomTabFallback
		{
			/**
			 *
			 * @param activity The Activity that wants to open the Uri.
			 * @param uri The uri to be opened by the fallback.
			 */
			void OpenUri(Activity activity, Uri uri);
		}

	}
}
