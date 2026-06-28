using Engine.Physics;
using GLTech.World;

namespace GLTech.Scripting.Physics
{
    /// <summary>
    /// Corpo cinemático da engine: o componente reutilizável que "se move respeitando
    /// colisões". Integra a <see cref="Velocity"/> em passos físicos fixos, resolve a
    /// colisão delegando ao serviço da engine (<see cref="Scene.MoveAndSlide"/>, círculo
    /// de raio <see cref="Radius"/> com deslize) e interpola a posição entre passos para
    /// renderização suave.
    ///
    /// Separação de papéis: o JOGO só escreve <see cref="Velocity"/> (via um controller);
    /// a ENGINE faz a detecção, a resposta (clip + slide) e a interpolação. Serve a
    /// qualquer entidade (player, inimigos, projéteis), não só à câmera.
    /// </summary>
    public class KinematicBody : Script
    {
        // Estado físico discreto (atualizado no FixedUpdate).
        private Vector previousPosition; // passo anterior — base da interpolação
        private Vector currentPosition;  // passo mais recente
        private Vector velocity;

        /// <summary>Raio do corpo usado na colisão de círculo. 0 = trata como ponto.</summary>
        public float Radius { get; set; } = 0.1f;

        /// <summary>Quando false, o corpo ignora colisões e atravessa paredes (noclip).</summary>
        public bool CollisionEnabled { get; set; } = true;

        /// <summary>Posição inicial do corpo no mundo.</summary>
        public Vector StartPosition { get; set; } = Vector.Zero;

        /// <summary>Velocidade do corpo (unidades/segundo). É o que o jogo controla.</summary>
        public Vector Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        public float Speed => velocity.Module;

        /// <summary>Posição física mais recente (sem o suavizado de interpolação).</summary>
        public Vector Position => currentPosition;

        /// <summary>Acelera respeitando o passo de tempo atual.</summary>
        public void Accelerate(Vector acceleration) => velocity += acceleration * Time.TimeStep;

        /// <summary>Soma um impulso instantâneo de velocidade.</summary>
        public void AddVelocity(Vector deltaVelocity) => velocity += deltaVelocity;

        [ScriptStart]
        private void Start()
        {
            currentPosition = StartPosition;
            previousPosition = StartPosition;
            if (Entity is not null)
                Entity.WorldPosition = StartPosition;
        }

        [ScriptFixedUpdate]
        private void FixedUpdate()
        {
            previousPosition = currentPosition;

            Vector step = velocity * Time.TimeStep;

            if (CollisionEnabled)
            {
                MoveResult result = Scene.CollisionSystem.MoveAndSlide(currentPosition, step, velocity, Radius);
                currentPosition = result.newPosition;
                velocity = result.newVelocity;
            }
            else
            {
                currentPosition += step;
            }
        }

        [ScriptUpdate]
        private void Update()
        {
            // Interpolação estilo "Fix Your Timestep": renderiza entre o passo físico
            // anterior e o atual conforme a fração de tempo já acumulada no passo seguinte.
            if (Entity is not null) {
                Vector interpolated = previousPosition + (currentPosition - previousPosition) * Time.FixedRemainder;
                Entity.WorldPosition = interpolated;
            }
        }
    }
}
