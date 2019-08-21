﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MediaManager.Library
{
    public interface IRadio : IList<IMediaItem>, INotifyCollectionChanged, INotifyPropertyChanged, IContentItem
    {
        void Move(int oldIndex, int newIndex);

        string Uri { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        string Tags { get; set; }

        string Genre { get; set; }

        object Image { get; set; }

        string ImageUri { get; set; }

        object Rating { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime UpdatedAt { get; set; }

        SharingType SharingType { get; set; }
    }
}
