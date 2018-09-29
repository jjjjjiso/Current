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
            max = (int)BlockType.blue;
            CreateStage();
        }

        private void CreateStage()
        {
            stage = Stage.Create();
        }

        public void StageUpdate(int x, int y, SpecialType specialType)
        {
            if (stage == null) return;
            if (stage.isSpecialWait) return;
            stage.SpecialMatch(x, y);
        }

        public void StageUpdate(int x, int y, BlockType blockType)
        {
            if (stage == null) return;
            if (stage.isSpecialWait) return;
            stage.NormMatch(x, y, blockType);
        }

        //Property
        public Stage _Stage { get { return (instance == null) ? null : instance.stage; } }
        public int Min { get { return min; } }
        public int Max { get { return max; } }
    }
}