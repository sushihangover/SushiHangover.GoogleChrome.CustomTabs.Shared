using System.Collections.Generic;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Util;
using Java.Lang;
using Uri = Android.Net.Uri;

namespace Android.Support.CustomTabs.Shared
{
	public static class CustomTabsHelper
	{
		const string TAG = "CustomTabsHelper";
		const string STABLE_PACKAGE = "com.android.chrome";
		const string BETA_PACKAGE = "com.chrome.beta";
		const string DEV_PACKAGE = "com.chrome.dev";
		const string LOCAL_PACKAGE = "com.google.android.apps.chrome";
		const string FIREFOX = "org.mozilla.firefox";
		const string EXTRA_CUSTOM_TABS_KEEP_ALIVE = "android.support.customtabs.extra.KEEP_ALIVE";
		const string ACTION_CUSTOM_TABS_CONNECTION = "android.support.customtabs.action.CustomTabsService";

		static string sPackageNameToUse;

		public static void AddKeepAliveExtra(Context context, Intent intent)
		{
			var keepAliveIntent = new Intent().SetClassName(
				context.PackageName,
				Class.FromType(typeof(KeepAliveService)).CanonicalName);
			intent.PutExtra(EXTRA_CUSTOM_TABS_KEEP_ALIVE, keepAliveIntent);
		}

		public static string GetPackageNameToUse(Context context)
		{
			if (sPackageNameToUse != null) return sPackageNameToUse;

			var pm = context.PackageManager;
			// Get default VIEW intent handler.
			var activityIntent = new Intent(Intent.ActionView, Uri.Parse("http://www.example.com"));
			var defaultViewHandlerInfo = pm.ResolveActivity(activityIntent, 0);
			string defaultViewHandlerPackageName = null;
			if (defaultViewHandlerInfo != null)
			{
				defaultViewHandlerPackageName = defaultViewHandlerInfo.ActivityInfo.PackageName;
			}

			// Get all apps that can handle VIEW intents.
			// Re: https://github.com/GoogleChrome/custom-tabs-client/pull/25
			// var resolvedActivityList = pm.QueryIntentActivities(activityIntent, 0);
			IList<ResolveInfo> resolvedActivityList;
			if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
				resolvedActivityList = pm.QueryIntentActivities(activityIntent, PackageInfoFlags.MatchAll);
			else
				resolvedActivityList = pm.QueryIntentActivities(activityIntent, PackageInfoFlags.MatchDefaultOnly);
			var packagesSupportingCustomTabs = new List<string>();

			foreach (var info in resolvedActivityList)
			{
				var serviceIntent = new Intent();
				serviceIntent.SetAction(ACTION_CUSTOM_TABS_CONNECTION);
				serviceIntent.SetPackage(info.ActivityInfo.PackageName);
				if (pm.ResolveService(serviceIntent, 0) != null)
				{
					packagesSupportingCustomTabs.Add(info.ActivityInfo.PackageName);
					Log.Debug(TAG, info.ActivityInfo.PackageName);
				}
			}

			// Now packagesSupportingCustomTabs contains all apps that can handle both VIEW intents
			// and service calls.
			if (packagesSupportingCustomTabs.Count == 0)
			{
				sPackageNameToUse = null;
			}
			else if (packagesSupportingCustomTabs.Count == 1)
			{
				sPackageNameToUse = packagesSupportingCustomTabs[0];
			}
			else if (!TextUtils.IsEmpty(defaultViewHandlerPackageName)
				&& !HasSpecializedHandlerIntents(context, activityIntent)
				&& packagesSupportingCustomTabs.Contains(defaultViewHandlerPackageName))
			{
				sPackageNameToUse = defaultViewHandlerPackageName;
			}
			else if (packagesSupportingCustomTabs.Contains(FIREFOX))
			{
				sPackageNameToUse = FIREFOX;
			}
			else if (packagesSupportingCustomTabs.Contains(STABLE_PACKAGE))
			{
				sPackageNameToUse = STABLE_PACKAGE;
			}
			else if (packagesSupportingCustomTabs.Contains(BETA_PACKAGE))
			{
				sPackageNameToUse = BETA_PACKAGE;
			}
			else if (packagesSupportingCustomTabs.Contains(DEV_PACKAGE))
			{
				sPackageNameToUse = DEV_PACKAGE;
			}
			else if (packagesSupportingCustomTabs.Contains(LOCAL_PACKAGE))
			{
				sPackageNameToUse = LOCAL_PACKAGE;
			}
			Log.Info(TAG, $"PackageNameToUse: {sPackageNameToUse}");
			return sPackageNameToUse;
		}

		static bool HasSpecializedHandlerIntents(Context context, Intent intent)
		{
			try
			{
				var pm = context.PackageManager;
				var handlers = pm.QueryIntentActivities(intent, PackageInfoFlags.ResolvedFilter);
				if (handlers == null || handlers.Count == 0)
					return false;
				foreach (var resolveInfo in handlers)
				{
					var filter = resolveInfo.Filter;
					if (filter == null)
						continue;
					if (filter.CountDataAuthorities() == 0 || filter.CountDataPaths() == 0)
						continue;
					if (resolveInfo.ActivityInfo == null)
						continue;
					return true;
				}
			}
			catch (RuntimeException e)
			{
				Log.Error(TAG, $"Runtime exception while getting specialized handlers : \n{e}");
			}
			return false;
		}

		public static string[] GetPackages()
		{
			return new string[] { "", STABLE_PACKAGE, BETA_PACKAGE, DEV_PACKAGE, LOCAL_PACKAGE };
		}

	}
}
