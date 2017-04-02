using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media.Session;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;

namespace Plugin.MediaManager.AudioCompat
{
    public class AudioPlayer : Java.Lang.Object, 
        IDisposable,
        IAudioPlayer,
        MediaPlayer.IOnBufferingUpdateListener,
        MediaPlayer.IOnCompletionListener,
        MediaPlayer.IOnErrorListener,
        MediaPlayer.IOnPreparedListener,
        MediaPlayer.IOnSeekCompleteListener
    //MediaPlayer.IOnInfoListener
    {
        private AudioPlayerImplementation audioController;
        private MediaPlayer _mediaPlayer;

        private TimeSpan _buffered = TimeSpan.Zero;
        private int _audioPlayerState;
        private IMediaFile _currentMediaFile;

        public Dictionary<string, string> RequestHeaders { get; set; }

        public AudioPlayer(AudioPlayerImplementation controller)
        {
            audioController = controller;
        }

        public TimeSpan Buffered
        {
            get
            {
                if (_mediaPlayer == null)
                    return TimeSpan.Zero;
                else
                    return _buffered;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (_mediaPlayer == null
                    || (_audioPlayerState != PlaybackStateCompat.StatePlaying
                        && _audioPlayerState != PlaybackStateCompat.StatePaused))
                    return TimeSpan.Zero;
                else
                    return TimeSpan.FromMilliseconds(_mediaPlayer.Duration);
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_mediaPlayer == null
                    || (_audioPlayerState != PlaybackStateCompat.StatePlaying
                        && _audioPlayerState != PlaybackStateCompat.StatePaused))
                    return TimeSpan.FromSeconds(-1);
                else
                    return TimeSpan.FromMilliseconds(_mediaPlayer.CurrentPosition);
            }
        }

        public MediaPlayerStatus Status
        {
            get
            {
                return MediaPlayerStatus.Stopped;
            }
        }

        public async Task Pause()
        {
            if (_mediaPlayer == null)
                return;

            if (_mediaPlayer.IsPlaying)
                _mediaPlayer.Pause();
        }

        public async Task Play(IMediaFile mediaFile = null)
        {
            if(_mediaPlayer == null)
                InitializePlayer();

            _currentMediaFile = mediaFile;

            switch (_currentMediaFile.Availability)
            {
                case ResourceAvailability.Remote:
                    Android.Net.Uri uri = Android.Net.Uri.Parse(_currentMediaFile.Url);
                    await _mediaPlayer?.SetDataSourceAsync(Application.Context, uri, RequestHeaders);
                    break;
                case ResourceAvailability.Local:
                    Java.IO.File file = new Java.IO.File(_currentMediaFile.Url);
                    Java.IO.FileInputStream inputStream = new Java.IO.FileInputStream(file);
                    await _mediaPlayer?.SetDataSourceAsync(inputStream.FD);
                    inputStream.Close();
                    break;
            }
        }

        public async Task Seek(TimeSpan position)
        {
            _mediaPlayer?.SeekTo(Convert.ToInt32(position.TotalMilliseconds));
        }

        public async Task Stop()
        {
            try
            {
                if (_mediaPlayer == null)
                    return;

                if (_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Stop();
                }
                _mediaPlayer.Reset();
            }
            catch (Java.Lang.IllegalStateException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void InitializePlayer()
        {
        	//DisposeMediaPlayer();
        	_mediaPlayer = new MediaPlayer();

        	//Tell our player to sream music
            _mediaPlayer.SetAudioStreamType(Stream.Music);

            //Wake mode will be partial to keep the CPU still running under lock screen
            _mediaPlayer.SetWakeMode(Application.Context, WakeLockFlags.Partial);
            _mediaPlayer.SetOnBufferingUpdateListener(this);
            _mediaPlayer.SetOnCompletionListener(this);
            _mediaPlayer.SetOnErrorListener(this);
            _mediaPlayer.SetOnPreparedListener(this);
        }

        public void OnCompletion(MediaPlayer mp)
        {
            OnMediaFinished(new MediaFinishedEventArgs(_currentMediaFile));
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            //SessionManager.UpdatePlaybackState(PlaybackStateCompat.StateError, Position.Seconds, Enum.GetName(typeof(MediaError), what));
            Stop();
            return true;
        }

        public void OnSeekComplete(MediaPlayer mp)
        {
            //TODO: Implement buffering on seeking
        }

        public void OnPrepared(MediaPlayer mp)
        {
            mp.Start();
            //SessionManager.UpdatePlaybackState(PlaybackStateCompat.StatePlaying, Position.Seconds);
        }

        public void OnBufferingUpdate(MediaPlayer mp, int percent)
        {
            int duration = 0;
            if (_audioPlayerState == PlaybackStateCompat.StatePlaying ||
                _audioPlayerState == PlaybackStateCompat.StatePaused)
                duration = mp.Duration;

            int newBufferedTime = duration * percent / 100;
            if (newBufferedTime != Convert.ToInt32(Buffered.TotalSeconds))
            {
                _buffered = TimeSpan.FromSeconds(newBufferedTime);
            }
        }

        public event BufferingChangedEventHandler BufferingChanged;
        public event MediaFailedEventHandler MediaFailed;
        public event MediaFinishedEventHandler MediaFinished;
        public event PlayingChangedEventHandler PlayingChanged;
        public event StatusChangedEventHandler StatusChanged;

        protected virtual void OnBufferingChanged(BufferingChangedEventArgs e)
        {
            BufferingChanged?.Invoke(this, e);
        }

        protected virtual void OnMediaFailed(MediaFailedEventArgs e)
        {
            MediaFailed?.Invoke(this, e);
        }

        protected virtual void OnMediaFinished(MediaFinishedEventArgs e)
        {
            MediaFinished?.Invoke(this, e);
        }

        protected virtual void OnPlayingChanged(PlayingChangedEventArgs e)
        {
            PlayingChanged?.Invoke(this, e);
        }

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        ~AudioPlayer()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _mediaPlayer != null)
            {
                _mediaPlayer.Dispose();
                _mediaPlayer = null;
            }
        }

        public void Dispose()
        {
        	Dispose(true);
        	GC.SuppressFinalize(this);
        }
    }
}
