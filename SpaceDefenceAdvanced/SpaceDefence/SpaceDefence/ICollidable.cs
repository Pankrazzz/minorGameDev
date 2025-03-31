using SpaceDefence.Collision;

namespace SpaceDefence
{
    interface ICollidable
    {
        // Dont like this solution, but it works for now
        Collider GetCollider();
    }
}
