﻿using System;

namespace MongoDB.Entities.Core
{
    /// <summary>
    /// Implement this interface on entities you want the library to automatically store the creation date with
    /// </summary>
    public interface ICreatedOn
    {
        /// <summary>
        /// This property will be automatically set when an entity is created.
        /// <para>TIP: This property is useful when sorting by creation date.</para>
        /// </summary>
        DateTime CreatedOn { get; set; }
    }
}
