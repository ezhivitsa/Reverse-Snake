using Unity.Entities;
using UnityEngine;

public class SnakeStepSystem : ComponentSystem
{
    private struct Group
    {
        public Transform Transform;
        public SnakeStepInput SnakeStepInput;
    }

    protected override void OnUpdate()
    {
        foreach (var entity in GetEntities<Group>())
        {
            entity.Transform.position = new Vector3(
                entity.SnakeStepInput.PositionX,
                0,
                entity.SnakeStepInput.PositionY
            );
        }
    }
}
