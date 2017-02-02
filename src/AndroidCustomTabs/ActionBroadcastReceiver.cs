using Android.Content;
using Android.Widget;

namespace AndroidCustomTabs
{
	[BroadcastReceiver]
	public class ActionBroadcastReceiver : BroadcastReceiver
	{
		public static string KEY_ACTION_SOURCE = "org.chromium.customtabsdemos.ACTION_SOURCE";
		public const int ACTION_ACTION_BUTTON = 1;
		public const int ACTION_MENU_ITEM = 2;
		public const int ACTION_TOOLBAR = 3;

		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.DataString != null)
			{
				string toastText = GetToastText(context, intent.GetIntExtra(KEY_ACTION_SOURCE, -1), intent.DataString);
				Toast.MakeText(context, toastText, ToastLength.Short).Show();
			}
		}

		string GetToastText(Context context, int actionId, string url)
		{
			switch (actionId)
			{
				case ACTION_ACTION_BUTTON:
					return context.GetString(Resource.String.action_button_toast_text, url);
				case ACTION_MENU_ITEM:
					return context.GetString(Resource.String.menu_item_toast_text, url);
				case ACTION_TOOLBAR:
					return context.GetString(Resource.String.toolbar_toast_text, url);
				default:
					return context.GetString(Resource.String.unknown_toast_text, url);
			}
		}
	}
}
