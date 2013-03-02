using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace DialogueEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Use this.Content to load your game content here
            Conversation.Initialize(Content.Load<SpriteFont>(@"Fonts\Segoe"), 
                Content.Load<SoundEffect>(@"SoundEffects\ContinueDialogue"),
                Content.Load<Texture2D>(@"Textures\DialogueBoxBackground"), 
                new Rectangle(50, 50, 400, 100),
                Content.Load<Texture2D>(@"Textures\BorderImage"),
                5,
                Color.Black,
                Content.Load<Texture2D>(@"Textures\ConversationContinueIcon"),
                Content);

            // Load Avatars
            DirectoryInfo directoryInfo = new DirectoryInfo(Content.RootDirectory + @"\Avatars\");
            FileInfo[] fileInfo = directoryInfo.GetFiles();
            ArrayList arrayList = new ArrayList();

            foreach (FileInfo fi in fileInfo)
                arrayList.Add(fi.FullName);

            for (int i = 0; i < arrayList.Count; i++)
            {
                Conversation.Avatars.Add(Content.Load<Texture2D>(@"Avatars\" + i));
            }

            Conversation.StartConversation(1);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Add your update logic here
            Conversation.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Add your drawing code here
            spriteBatch.Begin();
            Conversation.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
