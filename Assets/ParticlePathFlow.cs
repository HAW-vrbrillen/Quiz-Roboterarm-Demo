using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlePathFlow : MonoBehaviour
{
    public Transform[] pathPoints; // Wegpunkte, die den Pfad definieren
    public float spread = 0.5f; // Wie stark die Partikel streuen sollen
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private float[] particlePathProgress;
    private Vector3[] particleOffsets;
    private int numParticles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        particlePathProgress = new float[ps.main.maxParticles];
        particleOffsets = new Vector3[ps.main.maxParticles];
        // Initialisiere eine zufällige Streuung für jedes Partikel
        for (int i = 0; i < particleOffsets.Length; i++)
        {
            particleOffsets[i] = new Vector3(
                Random.Range(-spread, spread), 
                Random.Range(-spread, spread), 
                Random.Range(-spread, spread)
            );
        }
    }

    void Update()
    {
        numParticles = ps.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            Vector3 nextPosition = CalculatePointAlongPath(i);
            // Wende die zufällige Streuung auf die Position an
            particles[i].position = nextPosition + particleOffsets[i];
        }

        ps.SetParticles(particles, numParticles);
    }

    Vector3 CalculatePointAlongPath(int particleIndex)
    {
        int currentPathIndex = (int)particlePathProgress[particleIndex];
        float t = particlePathProgress[particleIndex] - currentPathIndex;

        if (currentPathIndex < pathPoints.Length - 1)
        {
            Vector3 startPosition = pathPoints[currentPathIndex].position;
            Vector3 endPosition = pathPoints[currentPathIndex + 1].position;
            particlePathProgress[particleIndex] += Time.deltaTime; // Bewegungsgeschwindigkeit
            return Vector3.Lerp(startPosition, endPosition, t);
        }
        else
        {
            // Setze den Fortschritt zurück und behalte die Streuung bei
            particlePathProgress[particleIndex] = 0f;
            particleOffsets[particleIndex] = new Vector3(
                Random.Range(-spread, spread), 
                Random.Range(-spread, spread), 
                Random.Range(-spread, spread)
            );
            return pathPoints[0].position + particleOffsets[particleIndex];
        }
    }
}
