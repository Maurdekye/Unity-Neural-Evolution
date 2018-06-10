using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NetworkFitnessPair {
    public double fitness;
    public Network network;
    public string name;

    public NetworkFitnessPair(double fitness, Network network) {
        this.fitness = fitness;
        this.network = network;
    }

    public NetworkFitnessPair(Network network) {
        this.fitness = 0;
        this.network = network;
    }

    public NetworkFitnessPair(NetworkFitnessPair network) {
        this.fitness = network.fitness;
        this.network = new Network(network.network, Network.CopyMethod.DEEP);
    }
}

public class NetworkEvolver {
    /*public static Network Aggregate(List<NetworkFitnessPair> weighted_networks) {

        Network sample_network = weighted_networks.First().network;
        Network final_network = new Network(sample_network, Network.CopyMethod.SHALLOW);

        foreach (KeyValuePair<double, Network> weighted_network in weighted_networks) {
            double weight = weighted_network.Key;
            Network network = weighted_network.Value;
            
            for (int i = 0; i < network.input_layers; i++) {
                for (int h = 0; h < network.hidden_layers; h++) {
                    final_network.input_hidden_weights[i, h] += network.input_hidden_weights[i, h] * weight;
                }
            }

            for (int h = 0; h < network.hidden_layers; h++) {
                for (int o = 0; o < network.output_layers; o++) {
                    final_network.hidden_output_weights[h, o] += network.hidden_output_weights[h, o] * weight;
                }
            }
        }

        for (int i = 0; i < final_network.input_layers; i++) {
            for (int h = 0; h < final_network.hidden_layers; h++) {
                final_network.input_hidden_weights[i, h] /= weighted_networks.Count;
            }
        }

        for (int h = 0; h < final_network.hidden_layers; h++) {
            for (int o = 0; o < final_network.output_layers; o++) {
                final_network.hidden_output_weights[h, o] /= weighted_networks.Count;
            }
        }

        return final_network;
    }*/

    public static List<NetworkFitnessPair> Cull(List<NetworkFitnessPair> fit_networks) {
        double max_fitness = fit_networks.OrderByDescending(nf => nf.fitness).First().fitness;
        return fit_networks.Where(nf => nf.fitness / max_fitness >= Random.value).ToList();
    }

    public static NetworkFitnessPair Couple(NetworkFitnessPair male, NetworkFitnessPair female) {
        NetworkFitnessPair child = new NetworkFitnessPair(new Network(male.network, Network.CopyMethod.DEEP));

        for (int i = 0; i < child.network.input_layers; i++) {
            for (int h = 0; h < child.network.hidden_layers; h++) {
                if (Random.value > 0.5)
                    child.network.input_hidden_weights[i, h] = female.network.input_hidden_weights[i, h];
            }
        }

        for (int h = 0; h < child.network.hidden_layers; h++) {
            for (int o = 0; o < child.network.output_layers; o++) {
                if (Random.value > 0.5)
                    child.network.hidden_output_weights[h, o] = female.network.hidden_output_weights[h, o];
            }
        }

        return child;
    }

    public static List<NetworkFitnessPair> Breed(List<NetworkFitnessPair> networks, int target_size, double variation) {
        List<NetworkFitnessPair> new_networks = new List<NetworkFitnessPair>();
        foreach (NetworkFitnessPair n in networks) {
            new_networks.Add(new NetworkFitnessPair(n));
        }
        return FillGeneration(networks, new_networks, target_size, variation);
    }

    /*public static List<Network> NewGeneration(List<Network> networks, int target_size, double variation) {
        return FillGeneration(networks, new List<Network>(), target_size, variation);
    }*/

    public static List<NetworkFitnessPair> FillGeneration(List<NetworkFitnessPair> old_networks, List<NetworkFitnessPair> new_networks, int target_size, double variation) {
        while (new_networks.Count < target_size) {

            int male_ind = Random.Range(0, old_networks.Count);
            int female_ind = Random.Range(0, old_networks.Count);

            NetworkFitnessPair child = Couple(old_networks[male_ind], old_networks[female_ind]);
            if (variation != 0)
                child.network.Mutate(variation);

            new_networks.Add(child);
        }

        return new_networks;
    }
}