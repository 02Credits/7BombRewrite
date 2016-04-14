using Falling.Components;
using Falling.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class SpriteRenderer : IDrawnEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Sprite)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public void Draw(Entity entity)
        {
            var transform = entity.GetComponent<Transform>();
            var position = transform.Position;
            var rotation = transform.Rotation;
            var dimensions = entity.GetComponent<Dimensions>();
            var width = dimensions.Width;
            var height = dimensions.Height;
            var texture = Game.GetSystem<TextureManager>().Textures[entity.GetComponent<Textured>().Path];
            var color = entity.HasComponent<ColorTint>() ? entity.GetComponent<ColorTint>().Color : Color.White;

            var translationMatrix = Matrix.CreateTranslation(new Vector3(position, 0));
            var rotationMatrix = Matrix.CreateRotationZ(rotation);
            var transformationMatrix = rotationMatrix * translationMatrix;

            var widthOverTwo = width / 2;
            var heightOverTwo = height / 2;

            var p0 = Vector3.Transform(new Vector3(-widthOverTwo, -heightOverTwo, 0), transformationMatrix);
            var p1 = Vector3.Transform(new Vector3(widthOverTwo, -heightOverTwo, 0), transformationMatrix);
            var p2 = Vector3.Transform(new Vector3(widthOverTwo, heightOverTwo, 0), transformationMatrix);
            var p3 = Vector3.Transform(new Vector3(-widthOverTwo, heightOverTwo, 0), transformationMatrix);

            var t0 = new Vector2(0, 0);
            var t1 = new Vector2(1, 0);
            var t2 = new Vector2(1, 1);
            var t3 = new Vector2(0, 1);

            Game.GetSystem<VertexManager>().AddRectangle(texture, color,
                p0, p1, p2, p3,
                t0, t1, t2, t3);
        }
    }
}
