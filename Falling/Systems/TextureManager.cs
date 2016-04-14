using Falling.Components;
using Falling.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class TextureManager : IInitializedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Textured),
            typeof(PhysicsSource)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public Dictionary<string, Texture2D> Textures { get; private set; }
        private ContentManager contentManager;
        private GraphicsDevice graphics;

        public TextureManager(ContentManager contentManager, GraphicsDevice graphics)
        {
            Textures = new Dictionary<string, Texture2D>();

            this.contentManager = contentManager;
            this.graphics = graphics;
        }

        public void Initialize(Entity entity)
        {
            Texture2D texture = null;
            if (entity.HasComponent<Textured>())
            {
                texture = LoadTexturesIfNeeded(entity.GetComponent<Textured>().Path);
            }

            if (entity.HasComponent<PhysicsSource>())
            {
                texture = LoadTexturesIfNeeded(entity.GetComponent<PhysicsSource>().Path);
            }

            if (entity.HasComponent<Dimensions>() && texture != null)
            {
                var dimensions = entity.GetComponent<Dimensions>();
                if (dimensions.Width == 0 && dimensions.Height != 0)
                {
                    dimensions.Width = texture.Width * (float)texture.Height / dimensions.Height;
                }
                else if (dimensions.Height == 0 && dimensions.Width != 0)
                {
                    dimensions.Height = texture.Height * (float)texture.Width / dimensions.Width;
                }
            }
        }

        private Texture2D LoadTexturesIfNeeded(string path)
        {
            Texture2D texture;
            if (!Textures.ContainsKey(path))
            {
                texture = contentManager.Load<Texture2D>(path);
                Textures[path] = texture;
            }
            else
            {
                texture = Textures[path];
            }
            return texture;
        }
    }
}
