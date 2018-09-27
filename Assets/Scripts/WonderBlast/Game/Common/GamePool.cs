using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace WonderBlast.Game.Common
{
    public class GamePool : MonoBehaviour
    {
        public ObjectPool blockPool = null;
        //public ObjectPool orangeBlockPool = null;
        //public ObjectPool yellowblockPool = null;
        //public ObjectPool greenblockPool = null;
        //public ObjectPool blueblockPool = null;
        //public ObjectPool purpleblockPool = null;

        public ObjectPool arrowBombPool = null;
        public ObjectPool bombPool = null;
        public ObjectPool ranbowPool = null;

        //List<ObjectPool> colorBlocks = new List<ObjectPool>();

        private void Awake()
        {
            Assert.IsNotNull(blockPool);
            //Assert.IsNotNull(orangeBlockPool);
            //Assert.IsNotNull(yellowblockPool);
            //Assert.IsNotNull(greenblockPool);
            //Assert.IsNotNull(blueblockPool);
            //Assert.IsNotNull(purpleblockPool);
            Assert.IsNotNull(arrowBombPool);
            Assert.IsNotNull(bombPool);
            Assert.IsNotNull(ranbowPool);

            //colorBlocks.Add(redBlockPool);
            //colorBlocks.Add(orangeBlockPool);
            //colorBlocks.Add(yellowblockPool);
            //colorBlocks.Add(greenblockPool);
            //colorBlocks.Add(blueblockPool);
            //colorBlocks.Add(purpleblockPool);
        }

        public BlockEntity GetBlockEntity(int level, LevelBlock block)
        {
            if(block is LevelBlockType)
            {
                //var type = (LevelBlockType)block;
                return blockPool.GetObject().GetComponent<BlockEntity>();
                //switch(type.type)
                //{
                //    case BlockType.red:
                //        return redBlockPool.GetObject().GetComponent<BlockEntity>();
                //    case BlockType.orange:
                //        return orangeBlockPool.GetObject().GetComponent<BlockEntity>();
                //    case BlockType.yellow:
                //        return yellowblockPool.GetObject().GetComponent<BlockEntity>();
                //    case BlockType.green:
                //        return greenblockPool.GetObject().GetComponent<BlockEntity>();
                //    case BlockType.blue:
                //        return blueblockPool.GetObject().GetComponent<BlockEntity>();
                //    case BlockType.purple:
                //        return purpleblockPool.GetObject().GetComponent<BlockEntity>();
                //    case BlockType.random:
                //        {
                //            int random = Random.Range((int)BlockType.red, level + 1);
                //            return colorBlocks[random].GetObject().GetComponent<BlockEntity>();
                //        }
                //}
            }
            else if(block is LevelSpecialType)
            {
                var type = (LevelSpecialType)block;
                switch (type.type)
                {
                    case SpecialType.arrow:
                        return arrowBombPool.GetObject().GetComponent<BlockEntity>();
                    case SpecialType.bomb:
                        return bombPool.GetObject().GetComponent<BlockEntity>();
                    case SpecialType.ranbow:
                        return ranbowPool.GetObject().GetComponent<BlockEntity>();
                }
            }

            return null;
        }
    }
}