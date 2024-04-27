using System;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents a note with a body and last modified timestamp.
    /// </summary>
    [Serializable]
    public class Note
    {
        /// <summary>
        /// The body text of the note.
        /// </summary>
        public string body;

        /// <summary>
        /// The timestamp of when the note was last modified.
        /// Updated automatically when the body is changed via Inspector TextArea.
        /// </summary>
        public SerializedDateTime lastModified;
    }
}