using Android.App;
using Android.OS;
using Android.Util;
using D = System.Diagnostics.Debug;

namespace Android.Support.CustomTabs.Shared
{
	public class LogImportance : Java.Lang.Object, Java.Lang.IRunnable
	{
		const string TAG = "LogImportance";

		Importance mPreviousImportance = Importance.Empty;
		bool mPreviousServiceInUse;
		Handler mHandler;

		[Preserve(Conditional = true)]
		public void Run()
		{
			if (mHandler == null)
				mHandler = new Handler(Looper.MainLooper);
			ActivityManager.RunningAppProcessInfo state = new ActivityManager.RunningAppProcessInfo();
			ActivityManager.GetMyMemoryState(state);
			var importance = state.Importance;
			bool serviceInUse = state.ImportanceReasonCode == ImportanceReason.ServiceInUse;
			if (importance != mPreviousImportance || serviceInUse != mPreviousServiceInUse)
			{
				mPreviousImportance = importance;
				mPreviousServiceInUse = serviceInUse;
				string message = "New importance = " + importance;
				if (serviceInUse) 
					message += " (Reason: Service in use)";
				Log.Warn(TAG, message);
			}
			mHandler?.PostDelayed(this, 1000 * 2);
			//D.WriteLine("Running");
		}

		[Preserve(Conditional = true)]
		public void Stop()
		{
			mHandler?.RemoveCallbacks(this);
		}
	}
}
