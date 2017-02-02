using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Java.Lang;

namespace Android.Support.CustomTabs.Shared
{
	// https://bugzilla.xamarin.com/show_bug.cgi?id=52170
	// Bug 52170 - A Service defined within an Android-based Library will cause two Service entries in manifest (edit)
	//< service android: exported = "true" android: label = "KeepAliveService" android: name = "md53facaf42d4b267787bed3d0053111821.KeepAliveService" />
	//< service android: name = "md59f70a99687498e7ba187118950981d26.KeepAliveService" />

	/// <summary>
	/// Empty service used by the custom tab to bind to, raising the application's importance.
	/// </summary>
	[Service(Exported = true)]
	public class KeepAliveService : App.Service
	{
		const string TAG = "KeepAliveService";

		IBinder binder;
		[Preserve(Conditional = true)]
		public KeepAliveService()
		{
			var serviceClassName = Class.FromType(typeof(KeepAliveService)).CanonicalName;
			Log.Debug(TAG, $"{serviceClassName}");
		}

		[Preserve(Conditional = true)]
		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			return StartCommandResult.NotSticky;
		}

		[Preserve(Conditional = true)]
		public override IBinder OnBind(Intent intent)
		{
			binder = new KeepAliveServiceBinder(this);
			return binder;
		}

		[Preserve(Conditional = true)]
		public class KeepAliveServiceBinder : Binder
		{
			readonly KeepAliveService service;

			[Preserve(Conditional = true)]
			public KeepAliveServiceBinder(KeepAliveService service)
			{
				this.service = service;
			}

			[Preserve(Conditional = true)]
			public KeepAliveService GetKeepAliveService()
			{
				return service;
			}
		}
	}
}
