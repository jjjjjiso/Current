using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using WaterBlast.System;
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
        
        public ObjectPool canBlockPool = null;
        public ObjectPool paperBlockPool = null;
        public ObjectPool boxBlockPool = null;
        public ObjectPool stickyBlockPool = null;

        public ObjectPool bubbleBlockerPool = null;
        public ObjectPool radiationBlockerPool = null;

        public ObjectPool arrowBombPool = null;
        public ObjectPool bombPool = null;
        public ObjectPool rainbowPool = null;

        public ObjectPool redBlockParticlesPool = null;
        public ObjectPool orangeBlockParticlesPool = null;
        public ObjectPool yellowBlockParticlesPool = null;
        public ObjectPool greenBlockParticlesPool = null;
        public ObjectPool blueBlockParticlesPool = null;
        public ObjectPool purpleBlockParticlesPool = null;
        public ObjectPool boxBlockParticlesPool = null;
        public ObjectPool stickyBlockParticlesPool = null;

        public ObjectPool bubbleParticlesPool = null;

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
            
            Assert.IsNotNull(canBlockPool);
            Assert.IsNotNull(paperBlockPool);
            Assert.IsNotNull(boxBlockPool);
            Assert.IsNotNull(stickyBlockPool);

            Assert.IsNotNull(bubbleBlockerPool);
            Assert.IsNotNull(radiationBlockerPool);

            Assert.IsNotNull(arrowBombPool);
            Assert.IsNotNull(bombPool);
            Assert.IsNotNull(rainbowPool);

            Assert.IsNotNull(redBlockParticlesPool);
            Assert.IsNotNull(orangeBlockParticlesPool);
            Assert.IsNotNull(yellowBlockParticlesPool);
            Assert.IsNotNull(greenBlockParticlesPool);
            Assert.IsNotNull(blueBlockParticlesPool);
            Assert.IsNotNull(purpleBlockParticlesPool);
            Assert.IsNotNull(boxBlockParticlesPool);
            Assert.IsNotNull(stickyBlockParticlesPool);

            Assert.IsNotNull(bubbleParticlesPool);

            Assert.IsNotNull(lineHorizontalParticlesPool);
            Assert.IsNotNull(lineVerticalParticlesPool);

            colorBlocks.Add(redBlockPool);
            colorBlocks.Add(orangeBlockPool);
            colorBlocks.Add(yellowBlockPool);
            colorBlocks.Add(greenBlockPool);
            colorBlocks.Add(blueBlockPool);
            colorBlocks.Add(purpleBlockPool);
        }

        public BlockEntity GetBlockerEntity(LevelBlock block)
        {
            switch (block.blockerType)
            {
                case BlockerType.bubble:
                    return bubbleBlockerPool.GetObj().GetComponent<BlockEntity>();
                case BlockerType.radiation:
                    return radiationBlockerPool.GetObj().GetComponent<BlockEntity>();
            }

            return null;
        }

        public GameObject GetBlockerParticles(BlockEntity blockEntity)
        {
            Blocker blocker = blockEntity as Blocker;
            if (blocker != null)
            {
                switch (blocker._BlockerType)
                {
                    case BlockerType.bubble:
                        return bubbleParticlesPool.GetObj();
                }
            }

            return null;
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
                            return empty.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.red:
                        {
                            return redBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.orange:
                        {
                            return orangeBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.yellow:
                        {
                            return yellowBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.green:
                        {
                            return greenBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.blue:
                        {
                            return blueBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.purple:
                        {
                            return purpleBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.random:
                        {
                            int random = Random.Range(0, GameMgr.G.availableColors.Count);
                            ColorType colorType = GameMgr.G.availableColors[random];
                            ObjectPool temp = null;
                            switch (colorType)
                            {
                                case ColorType.red:
                                    temp = redBlockPool;
                                    break;
                                case ColorType.orange:
                                    temp = orangeBlockPool;
                                    break;
                                case ColorType.yellow:
                                    temp = yellowBlockPool;
                                    break;
                                case ColorType.green:
                                    temp = greenBlockPool;
                                    break;
                                case ColorType.blue:
                                    temp = blueBlockPool;
                                    break;
                                case ColorType.purple:
                                    temp = purpleBlockPool;
                                    break;
                            }
                            return temp.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.can:
                        {
                            return canBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.paper:
                        {
                            return paperBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.box:
                        {
                            return boxBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                    case BlockType.sticky:
                        {
                            return stickyBlockPool.GetObj().GetComponent<BlockEntity>();
                        }
                }
            }
            else if(block is LevelBoosterType)
            {
                var type = (LevelBoosterType)block;
                switch (type.type)
                {
                    case BoosterType.arrow:
                        return arrowBombPool.GetObj().GetComponent<BlockEntity>();
                    case BoosterType.bomb:
                        return bombPool.GetObj().GetComponent<BlockEntity>();
                    case BoosterType.rainbow:
                        return rainbowPool.GetObj().GetComponent<BlockEntity>();
                }
            }

            return null;
        }

        public GameObject GetParticles(BlockEntity blockEntity)
        {
            Block block = blockEntity as Block;
            if (block != null)
            {
                switch (block._BlockType)
                {
                    case BlockType.red:
                        return redBlockParticlesPool.GetObj();
                    case BlockType.orange:
                        return orangeBlockParticlesPool.GetObj();
                    case BlockType.yellow:
                        return yellowBlockParticlesPool.GetObj();
                    case BlockType.green:
                        return greenBlockParticlesPool.GetObj();
                    case BlockType.blue:
                        return blueBlockParticlesPool.GetObj();
                    case BlockType.purple:
                        return purpleBlockParticlesPool.GetObj();
                    case BlockType.box:
                        return boxBlockParticlesPool.GetObj();
                    case BlockType.sticky:
                        return stickyBlockParticlesPool.GetObj();
                }
            }

            return null;
        }
    }
}