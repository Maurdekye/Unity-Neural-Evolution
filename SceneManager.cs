using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {


    [Header("Simulation Speed")]
    [Range(1, 100)]
    public float Timescale = 1;

    [Header("Evolution Settings")]
    public NetworkProprietor EvolvingObject;
    [Range(0, 1)]
    public double MutationFactor = 0.02;
    [Range(0, 1000)]
    public int PopulationSize = 30;
    [Range(0, 15)]
    public float GracePeriod = 3;
    [Range(0, 2)]
    public double GraceExtensionMinimumDistance = 0.2;
    [SerializeField]
    public Transform StartingPosition;
    public Canvas UI;
    public Camera FollowingCamera;

    List<NetworkFitnessPair> Population;
    int subject_index = 0;
    int generation = 1;
    float last_fitness_increase;
    double best_fitness;
    double best_fitness_limiter;
    NetworkProprietor simulating_object;
    Vector3 DefaultOffset;
    bool AdvancingGeneration = false;

    void Start() {
        EvolvingObject.Initialize();
        Population = new List<NetworkFitnessPair>();
        for (int i = 0; i < PopulationSize; i++) {
            Population.Add(
                new NetworkFitnessPair(new Network(EvolvingObject.network, Network.CopyMethod.RANDOM))
            );
        }
        DefaultOffset = FollowingCamera.transform.position - StartingPosition.position;
        CreateNewSubject();
    }

    void FixedUpdate() {

        Time.timeScale = Timescale;

        if (AdvancingGeneration)
            return;

        // Update UI Elements

        Text GenCounter = UI.transform.FindChild("Generation").gameObject.GetComponent<Text>();
        Text SubjectCounter = UI.transform.FindChild("Subject").gameObject.GetComponent<Text>();
        Text FitnessTracker = UI.transform.FindChild("FitnessTracker").gameObject.GetComponent<Text>();
        Text GraceTime = UI.transform.FindChild("GraceTime").gameObject.GetComponent<Text>();

        GenCounter.text = $"Generation {generation}";
        SubjectCounter.text = $"{subject_index + 1}/{Population.Count}";
        FitnessTracker.text = $"{simulating_object.fitness:0.00} m [{best_fitness:0.00} m]";
        GraceTime.text = $"{(last_fitness_increase + GracePeriod) - Time.fixedTime:0.00} s";

        // Track Fitness

        best_fitness = System.Math.Max(simulating_object.fitness, best_fitness);
        if (simulating_object.fitness > best_fitness_limiter + GraceExtensionMinimumDistance) {
            best_fitness_limiter = simulating_object.fitness;
            last_fitness_increase = Time.fixedTime;
        } else if (Time.fixedTime > last_fitness_increase + GracePeriod) {
            if (subject_index >= Population.Count - 1) {
                CloseGeneration();
                generation += 1;
                BeginNextGeneration();
            } else {
                EndCurrentSubject();
                subject_index += 1;
                CreateNewSubject();
            }
        }

        // Update Camera

        FollowingCamera.transform.position = simulating_object.transform.position + DefaultOffset;
        FollowingCamera.transform.LookAt(simulating_object.transform);
    }

    public void EndCurrentSubject() {
        Population[subject_index].fitness = simulating_object.fitness;
        DestroyImmediate(simulating_object.gameObject);
    }

    public void CreateNewSubject() {
        simulating_object = Instantiate(EvolvingObject);
        simulating_object.transform.position = StartingPosition.position;
        simulating_object.transform.rotation = StartingPosition.rotation;
        simulating_object.network = Population[subject_index].network;
        best_fitness = 0;
        best_fitness_limiter = 0;
        last_fitness_increase = Time.fixedTime;
    }

    public void CloseGeneration() {
        AdvancingGeneration = true;
        EndCurrentSubject();
        int pop_size = Population.Count;
        Debug.Log("\n\n\n            ===================    Generation Results:");
        Population.ForEach(n => Debug.Log($"{n}: {n.fitness}"));
        Population = NetworkEvolver.Cull(Population);
        Debug.Log("            -------------------    Remaining specimens:");
        Population.ForEach(n => Debug.Log($"{n}: {n.fitness}"));
        Population = NetworkEvolver.Breed(Population, pop_size, MutationFactor);
        Debug.Log("            -------------------    New Population:");
        Population.ForEach(n => Debug.Log($"{n}: {n.fitness}"));
    }

    public void BeginNextGeneration() {
        subject_index = 0;
        CreateNewSubject();
        AdvancingGeneration = false;
    }
}

public abstract class NetworkProprietor : MonoBehaviour {

    public abstract Network network { get; set; }
    public abstract double fitness { get; set; }
    public abstract void Initialize();
}