namespace WaterBlast.Game.Common
{
    public enum BlockerType
    {
        none,
    }

    public enum BlockType
    {
        none,
        red,
        orange,
        yellow,
        green,
        blue,
        purple,
        random,
    }

    public enum BoosterType
    {
        none,
        arrow,
        bomb,
        rainbow,
    }

    public enum BoosterSynthesis
    {
        none,
        arrowBombAndBomb,
        arrowBombAndRainbow,
        BombAndRainbow,
    }

    public enum State
    {
        idle,
        move,
        wait,
        booster_move,//특수블럭쪽으로 움직이는 상태.
    }
}
