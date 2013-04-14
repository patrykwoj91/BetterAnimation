using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using AnimationAux;

namespace BetterSkinned
{
    /// <summary>
    /// This is the main class for your game
    /// </summary>
    public class SkinnedGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        /// <summary>
        /// This graphics device we are drawing on in this program
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// The camera we use
        /// </summary>
        private Camera camera;

        /// <summary>
        /// The animated model we are displaying
        /// </summary>
       // private AnimatedModel model = null;

        /// <summary>
        /// This model is loaded solely for the dance animation
        /// </summary>
        private AnimatedModel dance = null;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SkinnedGame()
        {
            // XNA startup
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            // Some basic setup for the display window
            this.IsMouseVisible = true;
			this.Window.AllowUserResizing = true;
			this.graphics.PreferredBackBufferWidth = 1024;
			this.graphics.PreferredBackBufferHeight = 768;

            // Create a simple mouse-based camera
            camera = new Camera(graphics);
            camera.Eye = new Vector3(190, 247, 387);
            camera.Center = new Vector3(-20, 86, 159);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the model we will display
            //model = new AnimatedModel("Victoria-hat-tpose");
            //model.LoadContent(Content);

            // Load the model that has an animation clip it in
            dance = new AnimatedModel("test_FBX_Y");
            dance.LoadContent(Content);

            // Obtain the clip we want to play. I'm using an absolute index, 
            // because XNA 4.0 won't allow you to have more than one animation
            // associated with a model, anyway. It would be easy to add code
            // to look up the clip by name and to index it by name in the model.
            AnimationClip clip = dance.Clips["Run"];
            //dance.Clips["Walk"].Duration = 8;

            // And play the clip
            AnimationPlayer player = dance.PlayClip(clip);
            player.Looping = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            dance.Update(gameTime);

            camera.Update(graphics.GraphicsDevice, gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.LightGray);

            dance.Draw(graphics.GraphicsDevice, camera, Matrix.Identity);

            base.Draw(gameTime);
        }
    }
}
