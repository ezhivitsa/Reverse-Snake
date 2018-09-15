using System;

[Serializable]
sealed class Step
{
    public int Row;

    public int Column;

    public int Number;

    public int StartNumber;

    public int Round;

    public bool Active = true;
}