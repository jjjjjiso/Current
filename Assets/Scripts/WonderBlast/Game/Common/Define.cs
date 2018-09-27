namespace WonderBlast.Game.Common
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

    public enum SpecialType
    {
        none,
        arrow,
        bomb,
        ranbow,
    }

    public enum State
    {
        idle,
        move,
        wait,
        special_move,//특수블럭쪽으로 움직이는 상태.
    }

    public enum Direction
    {
        left,
        right,
        up,
        down,
    }
}
