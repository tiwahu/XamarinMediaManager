using System;
using Plugin.MediaManager;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;
using UIKit;

namespace MediaSample.iOS
{
    public partial class ViewController : UIViewController
    {
        VideoSurface _videoSurface;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _videoSurface = new VideoSurface();
            VideoView.Add(_videoSurface);
            CrossMediaManager.Current.VideoPlayer.RenderSurface = _videoSurface;
            CrossMediaManager.Current.PlayingChanged += (sender, e) => ProgressView.Progress = (float)e.Progress;
        }

        public override void ViewDidLayoutSubviews()
        {
            _videoSurface.Frame = VideoView.Frame;
            base.ViewDidLayoutSubviews();
        }

        partial void PlayButton_TouchUpInside(UIButton sender2)
        {
            var video = new MediaFile() {
                Url = "https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4",
                Type = MediaFileType.Video
            };
            CrossMediaManager.Current.MediaFileChanged += (object sender, MediaFileChangedEventArgs e) => {
                var artist = e.File.Metadata.Artist;
            };
            CrossMediaManager.Current.Play("https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4", MediaFileType.Video);
        }
    }
}

