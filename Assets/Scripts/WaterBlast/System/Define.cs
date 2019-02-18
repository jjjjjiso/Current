namespace WaterBlast.System
{
    public enum BlockerType
    {
        none,
        ice,
    }

    public enum BlockType
    {
        empty,
        red,
        orange,
        yellow,
        green,
        blue,
        purple,
        random,
    }

    public enum ColorType
    {
        red,
        orange,
        yellow,
        green,
        blue,
        purple,
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

    public enum ItemType
    {
        hammer,
        horizon,
        vertical,
        mix,
    }

    public enum State
    {
        idle,
        move,
        wait,
        booster_move,//특수블럭쪽으로 움직이는 상태.
    }

    public enum GamePopupState
    {
        start,
        success,
        failed,
    }
}
