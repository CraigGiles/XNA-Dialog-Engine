using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ConversationEngine
{
    public class Speaker
    {
        #region Declarations

        public enum AvatarState { Normal, Surprised, Sad, Angry };

        public AvatarState avatarState = AvatarState.Normal;
        public int AvatarIndex;
        public string Message;

        public Dictionary<string, int> Choices = new Dictionary<string, int>();

        public bool IsChoice
        {
            get { return (Choices != null); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Add a new Speaker to a Conversation
        /// </summary>
        /// <param name="avatar">Avatar Index for Speaker</param>
        /// <param name="msg">Speaker's Message</param>
        public Speaker(int avatar, string msg, AvatarState state, Dictionary<string, int> choices)
        {
            AvatarIndex = avatar;
            Message = msg;
            avatarState = state;
            Choices = choices;
        }

        /// <summary>
        /// Needed for Serialization. Not intended for use.
        /// </summary>
        public Speaker()
        {
            AvatarIndex = 0;
            Message = "";
            avatarState = AvatarState.Normal;
            Choices = null;
        }

        #endregion
    }
}
