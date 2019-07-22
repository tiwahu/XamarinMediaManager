﻿using System;
using System.Threading.Tasks;
using MediaManager.Media;
using MediaManager.Playback;
using MediaManager.Video;

namespace MediaManager
{
    public delegate void BeforePlayingEventHandler(object sender, MediaPlayerEventArgs e);

    public delegate void AfterPlayingEventHandler(object sender, MediaPlayerEventArgs e);

    public interface IMediaPlayer<TPlayer, TPlayerView> : IMediaPlayer<TPlayer> where TPlayer : class where TPlayerView : class, IVideoView
    {
        TPlayerView PlayerView { get; }
    }

    public interface IMediaPlayer<TPlayer> : IMediaPlayer where TPlayer : class
    {
        TPlayer Player { get; set; }
    }

    public interface IMediaPlayer : IDisposable
    {

        //TODO: Maybe introduce a source property to find the current playing item
        //IMediaItem Source { get; internal set; }

        IVideoView VideoView { get; set; }

        bool AutoAttachVideoView { get; set; }

        /// <summary>
        /// Adds MediaItem to the Queue and starts playing
        /// </summary>
        Task Play(IMediaItem mediaItem);

        /// <summary>
        /// Starts playing
        /// </summary>
        Task Play();

        /// <summary>
        /// Stops playing but retains position
        /// </summary>
        Task Pause();

        /// <summary>
        /// Stops playing
        /// </summary>
        Task Stop();

        /// <summary>
        /// Changes position to the specified number of milliseconds from zero
        /// </summary>
        Task SeekTo(TimeSpan position);

        /// <summary>
        /// Setting or getting whether we are in the repeat state
        /// </summary>
        //RepeatMode RepeatMode { get; set; }

        event BeforePlayingEventHandler BeforePlaying;

        event AfterPlayingEventHandler AfterPlaying;
    }
}
