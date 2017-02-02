using Android.App;
using Android.Content;

namespace AndroidCustomTabs
{
	public class WebviewFallback : CustomTabActivityHelper.ICustomTabFallback
	{
		public void OpenUri(Activity activity, Android.Net.Uri uri)
		{
			var intent = new Intent(activity, typeof(WebviewActivity));
			intent.PutExtra(WebviewActivity.EXTRA_URL, uri.ToString());
			activity.StartActivity(intent);
		}
	}
}
