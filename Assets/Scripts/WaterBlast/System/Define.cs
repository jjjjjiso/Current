namespace WaterBlast.System
{
    public enum BlockerType
    {
        none,
        muddywater,
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
        bubble,
        can,
        paper,
        box1,
        box2,
        radiation,
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
