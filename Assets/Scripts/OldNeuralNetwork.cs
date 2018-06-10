using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

public class NetworkException : System.Exception {
    public NetworkException(string message) : base(message) { }
}

public class Network {

    public int input_layers;
    public int hidden_layers;
    public int output_layers;

    public double[,] input_hidden_weights;
    public double[,] hidden_output_weights;

    // ctors

    public enum CopyMethod {
        SHALLOW, RANDOM, DEEP
    }

    public Network(int input_layers, int hidden_layers, int output_layers) {
        this.input_layers = input_layers;
        this.hidden_layers = hidden_layers;
        this.output_layers = output_layers;

        Init_Weights(null);
    }

    public Network(Network other) {
        this.input_layers = other.input_layers;
        this.hidden_layers = other.hidden_layers;
        this.output_layers = other.output_layers;

        Init_Weights(other);
    }

    public Network(Network other, CopyMethod method) {
        this.input_layers = other.input_layers;
        this.hidden_layers = other.hidden_layers;
        this.output_layers = other.output_layers;

        if (method == CopyMethod.DEEP) {
            Init_Weights(other);
        } else if (method == CopyMethod.RANDOM) {
            Init_Weights(null);
        } else if (method == CopyMethod.SHALLOW) {
            this.input_hidden_weights = new double[input_layers, hidden_layers];
            this.hidden_output_weights = new double[hidden_layers, output_layers];
        }
    }

    public void Init_Weights(Network other) {

        this.input_hidden_weights = new double[input_layers, hidden_layers];
        this.hidden_output_weights = new double[hidden_layers, output_layers];

        for (int i = 0; i < input_layers; i++) {
            for (int h = 0; h < hidden_layers; h++) {
                if (other == null)
                    input_hidden_weights[i, h] = Random_neg_to_pos();
                else
                    input_hidden_weights[i, h] = other.input_hidden_weights[i, h];
            }
        }

        for (int h = 0; h < hidden_layers; h++) {
            for (int o = 0; o < output_layers; o++) {
                if (other == null)
                    hidden_output_weights[h, o] = Random_neg_to_pos();
                else
                    hidden_output_weights[h, o] = other.hidden_output_weights[h, o];
            }
        }
    }

    // methods

    public double[] Evaluate(double[] inputs) {
        if (inputs.Length != input_layers) {
            throw new NetworkException($"Given # input layers ({inputs.Length}) does not correspond to required # input layers ({input_layers})");
        }

        double[] hidden_values = new double[hidden_layers];

        for (int h = 0; h < hidden_layers; h++) {

            double node_sum = 0;

            for (int i = 0; i < input_layers; i++) {
                node_sum += inputs[i] * input_hidden_weights[i, h];
            }

            hidden_values[h] = Epsilon(node_sum);
        }

        double[] output_values = new double[output_layers];

        for (int o = 0; o < output_layers; o++) {

            double node_sum = 0;

            for (int h = 0; h < hidden_layers; h++) {
                node_sum += hidden_values[h] * hidden_output_weights[h, o];
            }

            output_values[o] = Epsilon(node_sum);
        }

        return output_values;
    }

    public void Mutate(double variance, double chance) {

        for (int i = 0; i < input_layers; i++) {
            for (int h = 0; h < hidden_layers; h++) {
                if (Random.value < chance)
                    input_hidden_weights[i, h] += Random_neg_to_pos() * (variance * input_hidden_weights[i, h]);
            }
        }

        for (int h = 0; h < hidden_layers; h++) {
            for (int o = 0; o < output_layers; o++) {
                if (Random.value < chance)
                    hidden_output_weights[h, o] += Random_neg_to_pos() * (variance * hidden_output_weights[h, o]);
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
}*/