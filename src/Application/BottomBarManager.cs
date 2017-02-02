using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.CustomTabs;
using Android.Widget;
using Java.Lang;

namespace CustomTabsClientApplication
{
	/// <summary>
	/// A {@link BroadcastReceiver} that manages the interaction with the active Custom Tab.
	/// </summary>
	[BroadcastReceiver]
	public class BottomBarManager : BroadcastReceiver
	{
		static WeakReference<MediaPlayer> sMediaPlayerWeakRef;

		public override void OnReceive(Context context, Intent intent)
		{
			int clickedId = intent.GetIntExtra(CustomTabsIntent.ExtraRemoteviewsClickedId, -1);
			Toast.MakeText(context,
						   "Current URL " + intent.DataString + "\nClicked id " + clickedId, ToastLength.Short).Show();

			CustomTabsSession session = SessionHelper.GetCurrentSession();
			if (session == null) return;

			if (clickedId == Resource.Id.play_pause)
			{
				MediaPlayer player = null;
				sMediaPlayerWeakRef.TryGetTarget(out player);
				if (player != null)
				{
					bool isPlaying = player.IsPlaying;
					if (isPlaying) player.Pause();
					else player.Start();
					// Update the play/stop icon to respect the current state.
					session.SetSecondaryToolbarViews(CreateRemoteViews(context, isPlaying), GetClickableIDs(),
							GetOnClickPendingIntent(context));
				}
			}
			else if (clickedId == Resource.Id.cover)
			{
				// Clicking on the cover image will dismiss the bottom bar.
				session.SetSecondaryToolbarViews(null, null, null);
			}
		}

		/// <summary>
		/// Creates the remote views.
		/// </summary>
		/// <returns>The remote views.</returns>
		/// <param name="context">Context.</param>
		/// <param name="showPlayIcon">If set to <c>true</c> show play icon.</param>
		public static RemoteViews CreateRemoteViews(Context context, bool showPlayIcon)
		{
			RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.remote_view);

			int iconRes = showPlayIcon ? Resource.Drawable.ic_play : Resource.Drawable.ic_stop;
			remoteViews.SetImageViewResource(Resource.Id.play_pause, iconRes);
			return remoteViews;
		}

		/// <summary>
		/// @return The PendingIntent that will be triggered when the user clicks on the Views listed by
		/// {@link BottomBarManager#getClickableIDs()}.
		/// </summary>
		/// <returns>The clickable identifier.</returns>
		public static int[] GetClickableIDs()
		{
			return new int[] { Resource.Id.play_pause, Resource.Id.cover };
		}

		/// <summary>
		/// Sets the {@link MediaPlayer} to be used when the user clicks on the RemoteViews.
		/// </summary>
		/// <returns>The on click pending intent.</returns>
		/// <param name="context">Context.</param>
		public static PendingIntent GetOnClickPendingIntent(Context context)
		{
			Intent broadcastIntent = new Intent(context, Class.FromType(typeof(BottomBarManager)));
			return PendingIntent.GetBroadcast(context, 0, broadcastIntent, 0);
		}

		/// <summary>
		/// Sets the {@link MediaPlayer} to be used when the user clicks on the RemoteViews.
		/// </summary>
		/// <param name="player">Player.</param>
		public static void SetMediaPlayer(MediaPlayer player)
		{
			sMediaPlayerWeakRef = new WeakReference<MediaPlayer>(player);
		}
	}
}
