using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.CustomTabs;
using Android.Support.CustomTabs.Shared;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CustomTabsHelper = Android.Support.CustomTabs.Shared.CustomTabsHelper;
using Uri = Android.Net.Uri;
using Pair = Android.Util.Pair;

namespace CustomTabsClientApplication
{
	// RoundIcon="@mipmap/ic_round_launcher",

	[Activity(Label = "CustomTabsClient.Application", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity, View.IOnClickListener, IServiceConnectionCallback
	{
		const string TAG = "CustomTabsClientExample";
		const string TOOLBAR_COLOR = "#ef6c00";

		EditText mEditText;
		CustomTabsSession mCustomTabsSession;
		CustomTabsClient mClient;
		CustomTabsServiceConnection mConnection;
		string mPackageNameToBind;
		Button mConnectButton;
		Button mWarmupButton;
		Button mMayLaunchButton;
		Button mLaunchButton;
		MediaPlayer mMediaPlayer;
		LogImportance mLogImportance;

		//	private static class NavigationCallback extends CustomTabsCallback
		//	{
		//		@Override
		//				public void onNavigationEvent(int navigationEvent, Bundle extras)
		//	{
		//		Log.w(TAG, "onNavigationEvent: Code = " + navigationEvent);
		//	}
		//}


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.main);

			mEditText = (EditText)FindViewById(Resource.Id.edit);
			mConnectButton = (Button)FindViewById(Resource.Id.connect_button);
			mWarmupButton = (Button)FindViewById(Resource.Id.warmup_button);
			mMayLaunchButton = (Button)FindViewById(Resource.Id.may_launch_button);
			mLaunchButton = (Button)FindViewById(Resource.Id.launch_button);
			var spinner = (Spinner)FindViewById(Resource.Id.spinner);
			mEditText.RequestFocus();
			mConnectButton.SetOnClickListener(this);
			mWarmupButton.SetOnClickListener(this);
			mMayLaunchButton.SetOnClickListener(this);
			mLaunchButton.SetOnClickListener(this);
			mMediaPlayer = MediaPlayer.Create(this, Resource.Raw.amazing_grace);

			var activityIntent = new Intent(Intent.ActionView, Uri.Parse("http://www.example.com"));
			var pm = PackageManager;
			var resolvedActivityList = pm.QueryIntentActivities(activityIntent, PackageInfoFlags.MatchAll);
			var packagesSupportingCustomTabs = new List<Pair>();
			foreach (var info in resolvedActivityList)
			{
				var serviceIntent = new Intent();
				serviceIntent.SetAction("android.support.customtabs.action.CustomTabsService");
				serviceIntent.SetPackage(info.ActivityInfo.PackageName);
				if (pm.ResolveService(serviceIntent, 0) != null)
				{
					packagesSupportingCustomTabs.Add(Pair.Create(info.LoadLabel(pm), info.ActivityInfo.PackageName));
				}
			}

			var arrayAdapter = new SpinnerArrayAdapter<Pair>(ApplicationContext, 0, packagesSupportingCustomTabs);
			spinner.Adapter = arrayAdapter;
			spinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
			{
				var item = arrayAdapter.GetItem(e.Position);
				if (TextUtils.IsEmpty(item.Second.ToString()))
				{
					mPackageNameToBind = null;
					return;
				}
				mPackageNameToBind = item.Second.ToString();
			};

			mLogImportance = new LogImportance();
			mLogImportance.Run();
		}

		protected override void OnDestroy()
		{
			mLogImportance?.Stop();
			UnbindCustomTabsService();
			base.OnDestroy();
		}

		class NavigationCallback : CustomTabsCallback
		{
			public override void OnNavigationEvent(int navigationEvent, Bundle extras)
			{
				Log.Warn(TAG, "onNavigationEvent: Code = " + navigationEvent);
			}
		}

		CustomTabsSession GetSession()
		{
			if (mClient == null)
			{
				mCustomTabsSession = null;
			}
			else if (mCustomTabsSession == null)
			{
				mCustomTabsSession = mClient.NewSession(new NavigationCallback());
				SessionHelper.setCurrentSession(mCustomTabsSession);
			}
			return mCustomTabsSession;
		}

		void BindCustomTabsService()
		{
			if (mClient != null) return;
			if (TextUtils.IsEmpty(mPackageNameToBind))
			{
				mPackageNameToBind = CustomTabsHelper.GetPackageNameToUse(this);
				if (mPackageNameToBind == null) return;
			}
			mConnection = new ServiceConnection(this);
			bool ok = CustomTabsClient.BindCustomTabsService(this, mPackageNameToBind, mConnection);
			if (ok)
			{
				mConnectButton.Enabled = false;
			}
			else
			{
				mConnection = null;
			}
		}

		void UnbindCustomTabsService()
		{
			if (mConnection == null)
				return;
			UnbindService(mConnection);
			mClient = null;
			mCustomTabsSession = null;
		}

		public void OnClick(View view)
		{
			string url = mEditText.Text;
			bool success;
			switch (view.Id)
			{
				case Resource.Id.connect_button:
					BindCustomTabsService();
					break;
				case Resource.Id.warmup_button:
					success = false;
					if (mClient != null)
						success = mClient.Warmup(0);
					mWarmupButton.Enabled &= success;
					break;
				case Resource.Id.may_launch_button:
					CustomTabsSession session = GetSession();
					success = false;
					if (mClient != null)
						success = session.MayLaunchUrl(Uri.Parse(url), null, null);
					mMayLaunchButton.Enabled &= success;
					break;
				case Resource.Id.launch_button:
					var builder = new CustomTabsIntent.Builder(GetSession());
					builder.SetToolbarColor(Color.ParseColor(TOOLBAR_COLOR)).SetShowTitle(true);
					PrepareMenuItems(builder);
					PrepareActionButton(builder);
					PrepareBottombar(builder);
					builder.SetStartAnimations(this, Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
					builder.SetExitAnimations(this, Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
					builder.SetCloseButtonIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_arrow_back));
					CustomTabsIntent customTabsIntent = builder.Build();
					CustomTabsHelper.AddKeepAliveExtra(this, customTabsIntent.Intent);
					customTabsIntent.LaunchUrl(this, Uri.Parse(url));
					break;
			}
		}

		void PrepareMenuItems(CustomTabsIntent.Builder builder)
		{
			var menuIntent = new Intent();
			menuIntent.SetClass(ApplicationContext, Class);
			// Optional animation configuration when the user clicks menu items.
			var menuBundle = ActivityOptions.MakeCustomAnimation(this, Android.Resource.Animation.SlideInLeft,
																	Android.Resource.Animation.SlideOutRight).ToBundle();
			var pi = PendingIntent.GetActivity(ApplicationContext, 0, menuIntent, 0, menuBundle);
			builder.AddMenuItem("Menu entry 1", pi);
		}

		void PrepareActionButton(CustomTabsIntent.Builder builder)
		{
			// An example intent that sends an email.
			Intent actionIntent = new Intent(Intent.ActionSend);
			actionIntent.SetType("*/*");
			actionIntent.PutExtra(Intent.ExtraEmail, "example@example.com");
			actionIntent.PutExtra(Intent.ExtraSubject, "example");
			PendingIntent pi = PendingIntent.GetActivity(this, 0, actionIntent, 0);
			Bitmap icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_share);
			builder.SetActionButton(icon, "send email", pi, true);
		}

		void PrepareBottombar(CustomTabsIntent.Builder builder)
		{
			BottomBarManager.SetMediaPlayer(mMediaPlayer);
			builder.SetSecondaryToolbarViews(BottomBarManager.CreateRemoteViews(this, true),
					BottomBarManager.GetClickableIDs(), BottomBarManager.GetOnClickPendingIntent(this));
		}

		public void OnServiceConnected(CustomTabsClient client)
		{
			mClient = client;
			mConnectButton.Enabled = false;
			mWarmupButton.Enabled = true;
			mMayLaunchButton.Enabled = true;
			mLaunchButton.Enabled = true;
		}

		public void OnServiceDisconnected()
		{
			mConnectButton.Enabled = true;
			mWarmupButton.Enabled = false;
			mMayLaunchButton.Enabled = false;
			mLaunchButton.Enabled = false;
			mClient = null;
		}
	}
}

