namespace WaterBlast.System
{
    public enum BlockerType
    {
        none,
        bubble,
        radiation,
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
        can,
        paper,
        box,
        sticky,
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
        blocker,
    }

    public enum GamePopupState
    {
        start,
        success,
        failed,
    }

    public enum LobbyButtonType
    {
        rank,
        home,
        shop
    }
}
