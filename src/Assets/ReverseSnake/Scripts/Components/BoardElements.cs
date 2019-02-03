using Leopotam.Ecs;
using System.Collections.Generic;

sealed class BoardElements
{
    [EcsIgnoreNullCheck]
    public List<BoardElement> Elements;
}
