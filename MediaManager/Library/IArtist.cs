﻿using System.Collections.Generic;
using MediaManager.Media;

namespace MediaManager.Library
{
    public interface IArtist
    {
        string ArtistId { get; set; }

        string Name { get; set; }

        string Biography { get; set; }

        string Tags { get; set; }

        string Genre { get; set; }

        object Image { get; set; }

        string ImageUri { get; set; }

        object Rating { get; set; }

        IList<IAlbum> Albums { get; set; }

        IList<IMediaItem> TopTracks { get; set; }
    }
}
