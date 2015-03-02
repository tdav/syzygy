using amulware.Graphics;
using Bearded.Utilities.Math;
using Bearded.Utilities.SpaceTime;
using Syzygy.Rendering;
using TimeSpan = Bearded.Utilities.SpaceTime.TimeSpan;

namespace Syzygy.Game.Astronomy
{
    sealed class OrbitingBody : GameObject, IBody
    {
        private readonly Id<IBody> id;
        private readonly IBody parent;
        private readonly Unit orbitRadius;
        private Direction2 orbitDirection;
        private readonly Unit radius;
        private readonly float mass;
        private readonly Color color;

        private readonly Angle angularVelocity;

        private Position2 center;

        public OrbitingBody(GameState game, Id<IBody> id, IBody parent,
            Unit orbitRadius, Direction2 orbitDirection, Unit radius, float mass, Color color)
            : base(game)
        {
            this.id = id;
            this.parent = parent;
            this.orbitRadius = orbitRadius;
            this.orbitDirection = orbitDirection;
            this.radius = radius;
            this.mass = mass;
            this.color = color;

            this.center = this.calculatePosition();

            this.angularVelocity =
                ((Constants.G * parent.Mass / orbitRadius.NumericValue).Sqrted() / orbitRadius.NumericValue).Radians();

            game.Bodies.Add(this);
            game.BodyDictionary.Add(id, this);
        }

        public OrbitingBody(GameState game, Id<IBody> id, Id<IBody> parentId,
            Unit orbitRadius, Direction2 orbitDirection, Unit radius, float mass, Color color)
            : this(game, id, game.BodyDictionary[parentId], orbitRadius, orbitDirection, radius, mass, color)
        {
        }

        private Position2 calculatePosition()
        {
            return new Position2(this.parent.Shape.Center.Vector + this.orbitDirection.Vector * this.orbitRadius.NumericValue);
        }

        public Circle Shape { get { return new Circle(this.center, this.radius); } }
        public float Mass { get { return this.mass; } }

        public override void Update(TimeSpan t)
        {
            var step = this.angularVelocity * (float)t.NumericValue;
            this.orbitDirection += step;
            this.center = this.calculatePosition();
        }

        public override void Draw(GeometryManager geos)
        {
            geos.Primitives.Color = this.color;
            geos.Primitives.DrawCircle(this.center.Vector, this.radius.NumericValue);
        }

    }
}
