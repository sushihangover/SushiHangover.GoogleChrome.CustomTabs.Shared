using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.CustomTabs;
using Android.Support.V7.App;
using Android.Views;
using Uri = Android.Net.Uri;
using CustomTabsHelper = Android.Support.CustomTabs.Shared.CustomTabsHelper;
using Android.Support.CustomTabs.Shared;

namespace AndroidCustomTabs
{
	[Activity(Label = "@string/title_activity_service_connection")]
	[MetaData("android.support.PARENT_ACTIVITY", Value = ".DemoListActivity")]
	public class ServiceConnectionActivity : AppCompatActivity, View.IOnClickListener, CustomTabActivityHelper.IConnectionCallback
	{
		EditText mUrlEditText;
		View mMayLaunchUrlButton;
		LogImportance mLogImportance;
		CustomTabActivityHelper customTabActivityHelper;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_serviceconnection);

			customTabActivityHelper = new CustomTabActivityHelper();
			customTabActivityHelper.SetConnectionCallback(this);

			mUrlEditText = (EditText)FindViewById(Resource.Id.url);
			mMayLaunchUrlButton = FindViewById(Resource.Id.button_may_launch_url);
			mMayLaunchUrlButton.Enabled = false;
			mMayLaunchUrlButton.SetOnClickListener(this);

			FindViewById(Resource.Id.start_custom_tab).SetOnClickListener(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			mLogImportance?.Stop();
			mLogImportance?.Dispose();
			mLogImportance = null;
			customTabActivityHelper.SetConnectionCallback(null);
		}

		protected override void OnStart()
		{
			base.OnStart();

			if (mLogImportance == null)
				mLogImportance = new LogImportance();
			mLogImportance.Run();
			customTabActivityHelper.BindCustomTabsService(this);
		}

		protected override void OnStop()
		{
			base.OnStop();

			customTabActivityHelper.UnbindCustomTabsService(this);
			mMayLaunchUrlButton.Enabled = false;
		}

		public void OnCustomTabsConnected()
		{
			mMayLaunchUrlButton.Enabled = true;
		}

		public void OnCustomTabsDisconnected()
		{
			mMayLaunchUrlButton.Enabled = false;
		}

		public void OnClick(View view)
		{
			Uri uri = Uri.Parse(mUrlEditText.Text);
			switch (view.Id)
			{
				case Resource.Id.button_may_launch_url:
					customTabActivityHelper.MayLaunchUrl(uri, null, null);
					break;
				case Resource.Id.start_custom_tab:
					CustomTabsIntent customTabsIntent =
						new CustomTabsIntent.Builder(customTabActivityHelper.GetSession()).Build();
					CustomTabsHelper.AddKeepAliveExtra(this, customTabsIntent.Intent);
					CustomTabActivityHelper.OpenCustomTab(this, customTabsIntent, uri, new WebviewFallback());
					break;
				default:
					throw new Exception("Unkown View Clicked");
			}
		}
	}
}
