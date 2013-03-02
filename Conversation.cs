using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ConversationEngine;

namespace DialogueEngine
{
    public static class Conversation
    {
        #region Declarations

        public static List<Speaker> ConversationSpeakers = new List<Speaker>();
        private static int currentSpeakerIndex = 0;
        public static ContentManager Content;

        public static SpriteFont spriteFont;

        public static SoundEffect soundEffect;

        private static Rectangle textRectangle;
        private static string message;
        
        private static string revealedMessage;
        private static float messageSpeed = 0.008f;
        private static float messageTimer = 0.0f;
        private static int stringIndex;
        public static bool MessageShown = false;

        private static Texture2D splitIcon;
        private static float splitIconSpeed = 0.4f;
        private static float splitIconTimer = 0.0f;
        private static int splitIconOffsetValue = 5;
        private static bool splitIconOffset = false;

        private static Texture2D backgroundImage;
        private static Rectangle boxRectangle;

        private static Texture2D borderImage;
        private static int borderWidth;
        private static Color borderColor;

        public static List<Texture2D> Avatars = new List<Texture2D>();
        private static Rectangle avatarRectangle;

        private static int currentChoiceSelection = 0;
        private static float choiceSpeed = 0.15f;
        private static float choiceTimer = 0.0f;

        public static bool Expired = false;
        
        #endregion

        #region Properties

        public static Vector2 StringPosition
        {
            get { return new Vector2(textRectangle.X, textRectangle.Y); }
        }

        public static string Message
        {
            get { return message; }
            set { message = constrainText(value); }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the Conversation Class
        /// </summary>
        /// <param name="font">Font to display text with</param>
        /// <param name="background">Window Background Image</param>
        /// <param name="initialRectangle">Window Background Rectangle</param>
        /// <param name="bImage">Window Border Image</param>
        /// <param name="bWidth">Window Border Width</param>
        /// <param name="bColor">Window Border Color</param>
        /// <param name="sIcon">Continue Reading Icon</param>
        public static void Initialize(SpriteFont font, SoundEffect sound, Texture2D background, Rectangle initialRectangle, Texture2D bImage, int bWidth, Color bColor, Texture2D sIcon, ContentManager content)
        {
            spriteFont = font;
            soundEffect = sound;
            backgroundImage = background;
            boxRectangle = initialRectangle;
            textRectangle = new Rectangle(initialRectangle.X + 10, initialRectangle.Y + 10, initialRectangle.Width - 20, initialRectangle.Height - 20);
            borderImage = bImage;
            borderWidth = bWidth;
            borderColor = bColor;
            splitIcon = sIcon;
            Content = content;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a new Conversation
        /// </summary>
        /// <param name="conversationID">Conversation ID to use</param>
        public static void StartConversation(int conversationID)
        {
            currentSpeakerIndex = 0;
            LoadConversation(conversationID);

            // You can create conversations like this. The output will be in the DialogueEngine\DialogueEngine\bin\x86\Debug directory.
            //ConversationSpeakers.Add(new Speaker(2, "This is a Test", Speaker.AvatarState.Normal, null));
            //ConversationSpeakers.Add(new Speaker(2, "Reid Flamm, Golden Sun is a great game. You really should play the first and second. Like, really. Or Ian will smite you. Alright, now I'm just testing to see how many lines I can get in this dialogue box. Like, really. It's important, ok? Don't hate. I need to figure out when I can artificially break one guy talking into two or three boxes. Cause yeah, that's important. But oh man, how do I manage that with different fonts? That will be an issue. Hrm..... Oh hey, it works. Go figure!", Speaker.AvatarState.Sad, null));
            //ConversationSpeakers.Add(new Speaker(2, "Return to the first speaker, and try ending with a preformatted string. And..... end.\n \n         -- Ian", Speaker.AvatarState.Surprised, null));
            //ConversationSpeakers.Add(new Speaker(2, "This is just a fourth avatar state test.", Speaker.AvatarState.Angry, null));
            //ConversationSpeakers.Add(new Speaker(2, "", Speaker.AvatarState.Normal, new Dictionary<string, int>() { { "Test Choice 1", 2 }, { "Test Choice 2", 2 }, { "Test Choice 3", 2 } })); // { Option String, Filename } 
            //SaveConversation(2); // the number is the file name.

            CreateBox(ConversationSpeakers[currentSpeakerIndex].Message, 
                new Rectangle(100, 200, 600, 150),
                new Rectangle(250, 215, 445, 115),
                new Rectangle(120, 215, 115, 115),
                backgroundImage);
        }
        
        /// <summary>
        /// Creates a new Conversation Window
        /// </summary>
        /// <param name="msg">Speaker Message</param>
        /// <param name="msgBox">Window Rectangle</param>
        /// <param name="textBox">Text Rectangle</param>
        /// <param name="avatarBox">Avatar Rectangle</param>
        /// <param name="background">Background Image</param>
        public static void CreateBox(string msg, Rectangle msgBox, Rectangle textBox, Rectangle avatarBox, Texture2D background)
        {
            boxRectangle = msgBox;
            textRectangle = textBox;
            avatarRectangle = avatarBox;
            backgroundImage = background;
            Expired = false;
            stringIndex = 0;
            revealedMessage = "";
            MessageShown = false;

            // Set last so it breaks appropriately to fit the box (textRectangle MUST be set first)
            Message = msg;
        }

        /// <summary>
        /// Removes the Conversation Box
        /// </summary>
        public static void RemoveBox()
        {
            Expired = true;
        }

        #endregion

        #region Saving and Loading Conversations

        /// <summary>
        /// Saves a Conversation File
        /// </summary>
        public static void SaveConversation(int id)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(id.ToString() + ".xml", settings))
            {
                IntermediateSerializer.Serialize<List<Speaker>>(writer, ConversationSpeakers, null);
            }
        }

        /// <summary>
        /// Loads a Conversation File
        /// </summary>
        public static void LoadConversation(int id)
        {
            currentSpeakerIndex = 0;
            ConversationSpeakers = null;
            ConversationSpeakers = Content.Load<List<Speaker>>(@"Conversations\" + id);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Breaks up a string so it fits in the box. Will split long messages into two or three if necessary
        /// </summary>
        /// <param name="message">Speaker Message String</param>
        /// <returns>Formatted String</returns>
        private static string constrainText(String message)
        {
            bool filled = false;
            string line = "";
            string returnString = "";
            string[] wordArray = message.Split(' ');

            // Go through each word in string
            foreach (string word in wordArray)
            {
                // If we add the next word to the current line and go beyond the width...
                if (spriteFont.MeasureString(line + word).X > textRectangle.Width)
                {
                    // If adding a new line doesn't put us beyond height
                    if (spriteFont.MeasureString(returnString + line + "\n").Y < textRectangle.Height)
                    {
                        returnString += line + "\n";
                        line = "";
                    }
                    // If adding a new line does put us beyond height
                    else if (!filled)
                    {
                        filled = true;
                        returnString += line;
                        line = "";
                    }
                }
                line += word + " ";
            }
            
            // We need to add another Speaker Object first
            if (filled)
            {
                ConversationSpeakers.Insert(currentSpeakerIndex + 1, 
                                            new Speaker(ConversationSpeakers[currentSpeakerIndex].AvatarIndex, 
                                                        line, 
                                                        ConversationSpeakers[currentSpeakerIndex].avatarState,
                                                        ConversationSpeakers[currentSpeakerIndex].Choices));
                return returnString;
            }
            else
            {
                return returnString + line;
            }
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Handles User Input during a Conversation
        /// </summary>
        /// <param name="keyboardState">KeyboardState</param>
        private static void HandleKeyboardInput(KeyboardState keyboardState)
        {
            // Regular Message
            if (!ConversationSpeakers[currentSpeakerIndex].IsChoice)
            {
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    // Continue to next message
                    if (currentSpeakerIndex + 1 < ConversationSpeakers.Count)
                    {
                        soundEffect.Play();
                        currentSpeakerIndex++;
                        revealedMessage = "";
                        stringIndex = 0;
                        ConversationSpeakers[currentSpeakerIndex].Message = constrainText(ConversationSpeakers[currentSpeakerIndex].Message);
                    }
                    // End Dialogue
                    else
                    {
                        RemoveBox();
                    }
                    MessageShown = false;
                }
                choiceTimer = 0.0f;
            }
            // A choice
            else
            {
                if (currentChoiceSelection < ConversationSpeakers[currentSpeakerIndex].Choices.Count - 1 && keyboardState.IsKeyDown(Keys.Down))
                {
                    currentChoiceSelection++;
                    choiceTimer = 0.0f;
                }
                if (currentChoiceSelection > 0 && keyboardState.IsKeyDown(Keys.Up))
                {
                    currentChoiceSelection--;
                    choiceTimer = 0.0f;
                }

                // Handle Selection
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    soundEffect.Play();
                    LoadConversation(ConversationSpeakers[currentSpeakerIndex].Choices.ElementAt(currentChoiceSelection).Value);
                }
            }
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Update the Conversation Message Box
        /// </summary>
        /// <param name="gameTime">XNA GameTime</param>
        public static void Update(GameTime gameTime)
        {
            if (!Expired)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                messageTimer += elapsed;
                splitIconTimer += elapsed;
                choiceTimer += elapsed;

                if (!ConversationSpeakers[currentSpeakerIndex].IsChoice && messageTimer >= messageSpeed)
                {
                    // Typewriter Effect
                    if (stringIndex < ConversationSpeakers[currentSpeakerIndex].Message.Length)
                    {
                        revealedMessage += ConversationSpeakers[currentSpeakerIndex].Message[stringIndex];
                        stringIndex++;
                    }
                    // Full message displayed, handle input
                    else
                    {
                        MessageShown = true;
                        KeyboardState keyboardState = Keyboard.GetState();
                        HandleKeyboardInput(keyboardState);
                    }
                    messageTimer = 0.0f;
                }

                if (ConversationSpeakers[currentSpeakerIndex].IsChoice && choiceTimer > choiceSpeed)
                {
                    KeyboardState keyboardState = Keyboard.GetState();
                    HandleKeyboardInput(keyboardState);
                }

                // Update Continue Reading Icon
                if (splitIconTimer >= splitIconSpeed)
                {
                    splitIconOffset = !splitIconOffset;
                    splitIconTimer = 0.0f;
                }
            }
        }

        /// <summary>
        /// Draws the Conversation Box to the Screen
        /// </summary>
        /// <param name="spriteBatch">XNA SpriteBatch</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!Expired)
            {
                // Only draw border if specified
                if (borderImage != null)
                {
                    spriteBatch.Draw(borderImage, 
                        new Rectangle(boxRectangle.X - borderWidth, boxRectangle.Y - borderWidth, boxRectangle.Width + 2 * borderWidth, boxRectangle.Height + 2 * borderWidth), 
                        borderColor);
                }
                
                // Only draw Background if specified
                if (backgroundImage != null)
                {
                    spriteBatch.Draw(backgroundImage, boxRectangle, Color.White);
                }
                
                // Check to make sure we have the Avatar
                if (ConversationSpeakers[currentSpeakerIndex].AvatarIndex < Avatars.Count())
                {
                    Rectangle avatarSource;

                    // These boxes correspond do the different Avatar States.
                    switch (ConversationSpeakers[currentSpeakerIndex].avatarState)
                    {
                        default:
                        case Speaker.AvatarState.Normal:
                            avatarSource = new Rectangle(0, 0, 96, 96);
                            break;
                        case Speaker.AvatarState.Surprised:
                            avatarSource = new Rectangle(96, 0, 96, 96);
                            break;
                        case Speaker.AvatarState.Sad:
                            avatarSource = new Rectangle(0, 96, 96, 96);
                            break;
                        case Speaker.AvatarState.Angry:
                            avatarSource = new Rectangle(96, 96, 96, 96);
                            break;
                    }

                    spriteBatch.Draw(Avatars[ConversationSpeakers[currentSpeakerIndex].AvatarIndex], avatarRectangle, avatarSource, Color.White);
                }
                
                // Draw the Message if no Choice specified
                if (!ConversationSpeakers[currentSpeakerIndex].IsChoice)
                {
                    spriteBatch.DrawString(spriteFont, revealedMessage, StringPosition, Color.White);
                    // Check to see if we need to draw the Continue Reading icon
                    if (MessageShown && currentSpeakerIndex + 1 < ConversationSpeakers.Count())
                    {
                        Rectangle splitRectangle = new Rectangle(boxRectangle.X + boxRectangle.Width - 2 * splitIcon.Width + splitIcon.Width / 2,
                                                                 boxRectangle.Y + boxRectangle.Height - 2 * splitIcon.Height + splitIcon.Height / 2,
                                                                 splitIcon.Width,
                                                                 splitIcon.Height);

                        if (splitIconOffset)
                        {
                            splitRectangle.Y += splitIconOffsetValue;
                        }

                        spriteBatch.Draw(splitIcon, splitRectangle, Color.White);
                    }
                }
                // Draw the Options for a Choice
                else
                {
                    int offset = textRectangle.Height / ConversationSpeakers[currentSpeakerIndex].Choices.Count;
                    int i = 0;

                    foreach (KeyValuePair<string, int> choice in ConversationSpeakers[currentSpeakerIndex].Choices)
                    {
                        spriteBatch.DrawString(spriteFont, choice.Key, new Vector2(StringPosition.X + splitIcon.Height + 10, StringPosition.Y + offset * i), Color.White);
                        i++;
                    }

                    spriteBatch.Draw(splitIcon, new Rectangle((int)StringPosition.X, (int)StringPosition.Y + offset * currentChoiceSelection, splitIcon.Width, splitIcon.Height), new Rectangle(0, 0, splitIcon.Width, splitIcon.Height), Color.White, 0.5f, Vector2.Zero, SpriteEffects.None, 0.0f);
                }
            }
        }

        #endregion
    }
}