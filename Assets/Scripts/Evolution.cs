using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Genome {
    public double fitness;
    public Network network;
    public string name;

    public Genome(double fitness, Network network) {
        this.fitness = fitness;
        this.network = network;
    }

    public Genome(Network network) {
        this.fitness = 0;
        this.network = network;
    }

    public Genome(Genome network) {
        this.fitness = network.fitness;
        this.network = new Network(network.network, Network.CopyMethod.DEEP);
    }
}

public class Evolution {

    public static List<Genome> Cull(List<Genome> genomes, double best_percentage) {
        return genomes.OrderByDescending(g => g.fitness).Take((int)(genomes.Count * best_percentage)).ToList();
    }

    public static Genome Couple(Genome male, Genome female) {
        Genome child = new Genome(male);

        for (int l = 0; l < child.network.layers.Length - 1; l++) {
            for (int x = 0; x < child.network.layers[l]; x++) {
                for (int y = 0; y < child.network.layers[l+1]; y++) {
                    if (Random.value > 0.5)
                        child.network.weights[l][x, y] = female.network.weights[l][x, y];
                }
            }
        }

        return child;
    }

    public static List<Genome> Breed(List<Genome> old_networks, int target_size, double mutation_variation, double mutation_chance, double problem_child_chance) {
        List<Genome> new_networks = old_networks.Select(g => new Genome(g)).ToList();

        while (new_networks.Count < target_size) {

            int male_ind = Random.Range(0, old_networks.Count);
            int female_ind = Random.Range(0, old_networks.Count);

            Genome child;
            if (Random.value < problem_child_chance)
                child = new Genome(new Network(new_networks[0].network, Network.CopyMethod.RANDOM));
            else
                child = Couple(old_networks[male_ind], old_networks[female_ind]);

            child.network.Mutate(mutation_variation, mutation_chance);
            new_networks.Add(child);
        }

        return new_networks;
    }
}