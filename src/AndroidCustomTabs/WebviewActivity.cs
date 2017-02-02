using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Webkit;

namespace AndroidCustomTabs
{
	/// <summary>
	/// This Activity is used as a fallback when there is no browser installed that supports Chrome Custom Tabs
	/// </summary>
	[Activity(Label = "@string/title_activity_webview")]
	[MetaData("android.support.PARENT_ACTIVITY", Value = ".DemoListActivity")]
	public class WebviewActivity : AppCompatActivity
	{
		public const string EXTRA_URL = "extra.url";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_webview);
			var url = Intent.GetStringExtra(EXTRA_URL);
			var webView = (WebView)FindViewById(Resource.Id.webview);
			webView.SetWebViewClient(new WebViewClient());
			webView.Settings.JavaScriptEnabled = true;
			Title = url;
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			webView.LoadUrl(url);
		}

		public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
		{
			switch (item.ItemId)
			{
				// Respond to the action bar's Up/Home button
				case Android.Resource.Id.Home:
					Finish();
					return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}
