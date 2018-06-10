using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NetworkException : System.Exception {
    public NetworkException(string message) : base(message) { }
}

public class Network {

    public int[] layers;
    public List<double[,]> weights;

    // ctors

    public enum CopyMethod {
        SHALLOW, RANDOM, DEEP
    }

    public Network(int[] layers) {
        this.layers = layers;

        Init_Weights(null);
    }

    public Network(Network other) {
        layers = other.layers;

        Init_Weights(other);
    }

    public Network(Network other, CopyMethod method) {
        layers = other.layers;

        if (method == CopyMethod.DEEP) {
            Init_Weights(other);
        } else if (method == CopyMethod.RANDOM) {
            Init_Weights(null);
        } else if (method == CopyMethod.SHALLOW) {
            for (int i = 0; i < layers.Length - 1; i++)
                weights.Add(new double[layers[i], layers[i + 1]]);
        }
    }

    public void Init_Weights(Network other) {
        if (layers.Length < 2) {
            throw new NetworkException($"Too few layers, {layers.Length}. Must have at least 2 layers.");
        } else if (layers.Any(l => l == 0)) {
            throw new NetworkException("All layers must have at least one node.");
        }

        weights = new List<double[,]>();
        for (int l = 0; l < layers.Length - 1; l++) {
            weights.Add(new double[layers[l], layers[l + 1]]);
            for (int x = 0; x < layers[l]; x++) {
                for (int y = 0; y < layers[l+1]; y++) {
                    weights[l][x, y] = other == null ? Random_neg_to_pos() : other.weights[l][x, y];
                }
            }
        }
    }

    // methods

    public double[] Evaluate(double[] inputs) {
        if (inputs.Length != layers[0]) {
            throw new NetworkException($"Given # input layers ({inputs.Length}) does not correspond to required # input layers ({layers[0]})");
        }

        double[] previous_values = inputs;
        double[] next_values = new double[layers[1]];

        for (int l = 0; l < layers.Length - 1; l++) {
            for (int y = 0; y < layers[l+1]; y++) {
                double sum = 0;
                for (int x = 0; x < layers[l]; x++) {
                    sum += previous_values[x] * weights[l][x, y];
                }
                next_values[y] = Epsilon(sum);
            }
            if (l < layers.Length - 2) {
                previous_values = next_values;
                next_values = new double[layers[l + 1]];
            }
        }

        return next_values;
    }

    public void Mutate(double variance, double chance) {

        for (int l = 0; l < layers.Length - 1; l++) {
            for (int x = 0; x < layers[l]; x++) {
                for (int y = 0; y < layers[l+1]; y++) {
                    if (Random.value < chance)
                        weights[l][x, y] += Random_neg_to_pos() * (variance * weights[l][x, y]);
                }
            }
        }
    }

    // statics

    public static double Random_neg_to_pos() {
        return (Random.value * 2) - 1;
    }

    public static double Epsilon(double x) {
        return 2 / (1 + System.Math.Exp(-x)) - 1;
    }
}