using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class GamePool : MonoBehaviour
    {
        public ObjectPool empty = null;
        public ObjectPool redBlockPool = null;
        public ObjectPool orangeBlockPool = null;
        public ObjectPool yellowBlockPool = null;
        public ObjectPool greenBlockPool = null;
        public ObjectPool blueBlockPool = null;
        public ObjectPool purpleBlockPool = null;

        public ObjectPool arrowBombPool = null;
        public ObjectPool bombPool = null;
        public ObjectPool rainbowPool = null;

        public ObjectPool redBlockParticlesPool = null;
        public ObjectPool orangeBlockParticlesPool = null;
        public ObjectPool yellowBlockParticlesPool = null;
        public ObjectPool greenBlockParticlesPool = null;
        public ObjectPool blueBlockParticlesPool = null;
        public ObjectPool purpleBlockParticlesPool = null;
        
        public ObjectPool lineHorizontalParticlesPool = null;
        public ObjectPool lineVerticalParticlesPool = null;
        public ObjectPool bombParticlesPool = null;
        public ObjectPool bombComboParticlesPool = null;

        List<ObjectPool> colorBlocks = new List<ObjectPool>();

        private void Awake()
        {
            Assert.IsNotNull(empty);
            Assert.IsNotNull(redBlockPool);
            Assert.IsNotNull(orangeBlockPool);
            Assert.IsNotNull(yellowBlockPool);
            Assert.IsNotNull(greenBlockPool);
            Assert.IsNotNull(blueBlockPool);
            Assert.IsNotNull(purpleBlockPool);

            Assert.IsNotNull(arrowBombPool);
            Assert.IsNotNull(bombPool);
            Assert.IsNotNull(rainbowPool);

            Assert.IsNotNull(redBlockParticlesPool);
            Assert.IsNotNull(orangeBlockParticlesPool);
            Assert.IsNotNull(yellowBlockParticlesPool);
            Assert.IsNotNull(greenBlockParticlesPool);
            Assert.IsNotNull(blueBlockParticlesPool);
            Assert.IsNotNull(purpleBlockParticlesPool);
            
            Assert.IsNotNull(lineHorizontalParticlesPool);
            Assert.IsNotNull(lineVerticalParticlesPool);

            colorBlocks.Add(redBlockPool);
            colorBlocks.Add(orangeBlockPool);
            colorBlocks.Add(yellowBlockPool);
            colorBlocks.Add(greenBlockPool);
            colorBlocks.Add(blueBlockPool);
            colorBlocks.Add(purpleBlockPool);
        }

        public BlockEntity GetBlockEntity(LevelBlock block)
        {
            if(block is LevelBlockType)
            {
                var type = (LevelBlockType)block;
                switch (type.type)
                {
                    case BlockType.empty:
                        {
                            return empty.GetObject().GetComponent<BlockEntity>();
                        }
                    case BlockType.red:
                        {
                            return redBlockPool.GetObject().GetComponent<BlockEntity>();
                        }
                    case BlockType.orange:
                        {
                            return orangeBlockPool.GetObject().GetComponent<BlockEntity>();
                        }
                    case BlockType.yellow:
                        {
                            return yellowBlockPool.GetObject().GetComponent<BlockEntity>();
                        }
                    case BlockType.green:
                        {
                            return greenBlockPool.GetObject().GetComponent<BlockEntity>();
                        }
                    case BlockType.blue:
                        {
                            return blueBlockPool.GetObject().GetComponent<BlockEntity>();
                        }
                    case BlockType.purple:
                        {
                            return purpleBlockPool.GetObject().GetComponent<BlockEntity>();
                        }
                    case BlockType.random:
                        {
                            int random = Random.Range(0, GameMgr.Get().Max);
                            return colorBlocks[random].GetObject().GetComponent<BlockEntity>();
                        }
                }
            }
            else if(block is LevelBoosterType)
            {
                var type = (LevelBoosterType)block;
                switch (type.type)
                {
                    case BoosterType.arrow:
                        return arrowBombPool.GetObject().GetComponent<BlockEntity>();
                    case BoosterType.bomb:
                        return bombPool.GetObject().GetComponent<BlockEntity>();
                    case BoosterType.rainbow:
                        return rainbowPool.GetObject().GetComponent<BlockEntity>();
                }
            }

            return null;
        }

        public GameObject GetParticles(BlockEntity blockEntity)
        {
            Block block = blockEntity as Block;
            if(block != null)
            {
                switch(block._BlockType)
                {
                    case BlockType.red:
                        return redBlockParticlesPool.GetObject();
                    case BlockType.orange:
                        return orangeBlockParticlesPool.GetObject();
                    case BlockType.yellow:
                        return yellowBlockParticlesPool.GetObject();
                    case BlockType.green:
                        return greenBlockParticlesPool.GetObject();
                    case BlockType.blue:
                        return blueBlockParticlesPool.GetObject();
                    case BlockType.purple:
                        return purpleBlockParticlesPool.GetObject();
                }
            }

            return null;
        }
    }
}