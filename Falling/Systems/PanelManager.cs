﻿using Falling.Components;
using Falling.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class PanelManager : IUpdatedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(PlayerControlled)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public int LowestPanelPoint = 0;
        public List<Entity> PanelEntities = new List<Entity>();

        public void Update(Entity entity)
        {
            var playerY = entity.GetComponent<Physics>().Body.Position.Y;
            var cameraWorldHeight = Game.GetSystem<CameraManager>().CameraWorldHeight;
            var playerCameraBottom  = playerY - cameraWorldHeight / 2;

            while (LowestPanelPoint > playerCameraBottom)
            {
                PanelEntities.Add(Game.Panel(LowestPanelPoint));
                LowestPanelPoint -= 10;
            }

            var playerCameraTopPlusBuffer = playerY + cameraWorldHeight / 2 + 10;
            foreach (var panelEntity in PanelEntities.ToList())
            {
                if (panelEntity.GetComponent<Physics>().Body.Position.Y > playerCameraTopPlusBuffer)
                {
                    PanelEntities.Remove(panelEntity);
                    Game.RemoveEntity(panelEntity);
                }
            }
        }
    }
}
