using Falling.Components;
using Falling.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class TrimmedSpriteRenderer : IDrawnEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(TrimmedSprite)
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
            var dimensionsVector = new Vector2(width, height);
            var texture = Game.GetSystem<TextureManager>().Textures[entity.GetComponent<Textured>().Path];
            var color = entity.HasComponent<ColorTint>() ? entity.GetComponent<ColorTint>().Color : Color.White;

            var translationMatrix = Matrix.CreateTranslation(new Vector3(position, 0));
            var rotationMatrix = Matrix.CreateRotationZ(rotation);
            var transformMatrix = rotationMatrix * translationMatrix;

            var verticesList = Utils.GetVerticesFromBody(entity.GetComponent<Physics>().Body);

            var vertexManager = Game.GetSystem<VertexManager>();

            foreach (var vertices in verticesList)
            {
                var worldCoords = vertices.Select((vector) => Vector3.Transform(new Vector3(vector, 0), transformMatrix));
                var textureCoords = vertices.Select((vector) => (vector + dimensionsVector / 2) / dimensionsVector);
                vertexManager.AddPolygon(texture, color, vertices.Count, worldCoords, textureCoords);
            }
        }
    }
}
