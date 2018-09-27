using WonderBlast.Game.Common;

namespace WonderBlast.Game.Manager
{
    public class GameMgr : MonoSingleton<GameMgr>
    {
        public GamePool gamePools = null;
        private Stage stage = null;

        private int min = 0;
        private int max = 0;

        private void Start()
        {
            min = (int)BlockType.red;
            max = (int)BlockType.purple;
            CreateStage();
        }

        private void CreateStage()
        {
            stage = Stage.Create();
        }

        public void StageUpdate(int x, int y, SpecialType specialType)
        {
            if (stage == null) return;
            stage.SpecialMatch(x, y);
            //switch(specialType)
            //{
            //    case SpecialType.left_right_arrow:
            //        stage.WidthMatch(y);
            //        break;
            //    case SpecialType.up_down_arrow:
            //        stage.HeightMatch(x);
            //        break;
            //    case SpecialType.bomb:
            //        stage.NineMatch(x, y);
            //        break;
            //    case SpecialType.ranbow:
            //        stage.SameColorMatch(x, y);
            //        break;
            //}
        }

        public void StageUpdate(int x, int y, BlockType blockType)
        {
            if (stage == null) return;
            stage.NormMatch(x, y, blockType);
        }

        //Property
        public Stage _Stage { get { return (instance == null) ? null : instance.stage; } }
        public int Min { get { return min; } }
        public int Max { get { return max; } }
    }
}