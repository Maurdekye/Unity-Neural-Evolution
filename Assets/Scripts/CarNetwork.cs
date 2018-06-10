using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class CarNetwork : NetworkProprietor {

    // Public Tunables

    [Header("Tunable Values")]
    public float DrivingForce = 50;
    public float SteeringForce = 20;
    [Range(0,20)]
    public double Sensitivity = 3;
    [Range(2, 100)]
    public int Sensors = 10;
    [Range(2,20)]
    public int HiddenLayers = 5;
    public bool DrawSensorRays = false;


    // Private Fields

    Network neural_network;

    [Header("Debug")]
    public float steering = 0;
    public float accelerating = 0;
    double network_fitness = 0;
    int input_dimension;
    Vector3 initial_point;

    bool initialized = false;
    public bool active = true;
    public double[] input_data;

    public override void Initialize() {
        Start();
    }

    public void Start () {
        input_dimension = Sensors + 3;
        neural_network = new Network(new[]{ input_dimension, HiddenLayers, 2 });
        initial_point = transform.position;
        initialized = true;
	}

    void FixedUpdate () {
        if (initialized && neural_network != null)
            UpdateParameters();
        if (active) {
            Rigidbody phys = GetComponent<Rigidbody>();
            phys.AddForce(transform.forward * accelerating * DrivingForce);
            phys.AddTorque(transform.up * steering * SteeringForce);
        }
    }

    public void UpdateParameters() {

        // Gather sensory data

        // Visual data

        double[] sensor_data = new double[Sensors];
        float inter_angle = 1 / (float)Sensors;
        for (int s = 0; s < Sensors; s++) {
            float angle = 360 * inter_angle * s;
            Quaternion rotation = Quaternion.AngleAxis(angle, transform.up);
            Vector3 direction = rotation * transform.forward;

            RaycastHit collision;
            double sensor_val = -1;
            if (Physics.Raycast(transform.position, direction, out collision)) {
                sensor_val = LogFormat(collision.distance);
                float dist = collision.distance;
                if (DrawSensorRays) {
                    Debug.DrawRay(transform.position, direction * Mathf.Min((float)Sensitivity * 2, dist), Color.grey);
                    Debug.DrawRay(transform.position, direction * Mathf.Min((float)Sensitivity, dist), Color.black);
                }
            } else if (DrawSensorRays) {
                Debug.DrawRay(transform.position, direction * (float)Sensitivity * 2, Color.grey);
                Debug.DrawRay(transform.position, direction * (float)Sensitivity, Color.black);
            }

            sensor_data[s] = sensor_val;
        }

        // Kinesthetic data

        Rigidbody phys = GetComponent<Rigidbody>();

        double[] kinetic_data = {
            Network.Epsilon(Vector3.Dot(phys.velocity, transform.forward) / 20),
            Network.Epsilon(Vector3.Dot(phys.velocity, transform.right) / 20),
            Network.Epsilon(transform.InverseTransformDirection(phys.angularVelocity).y)
        };

        double[] network_inputs = new double[input_dimension];
        sensor_data.CopyTo(network_inputs, 0);
        kinetic_data.CopyTo(network_inputs, Sensors);

        input_data = network_inputs;

        // Apply network

        if (neural_network == null)
            Debug.Log("Network is null!");
        double[] driving_instructions = neural_network.Evaluate(network_inputs);

        // Update control parameters

        steering = (float)driving_instructions[0];
        accelerating = (float)driving_instructions[1];

        if (DrawSensorRays) {
            Vector3 from = transform.position;
            Vector3 next = from + transform.forward * accelerating * DrivingForce;
            Vector3 to = next + transform.right * steering * SteeringForce;
            Debug.DrawLine(from, next, Color.red, 0, false);
            Debug.DrawLine(next, to, Color.magenta, 0, false);
        }

        // Update fitness

        network_fitness = (transform.position - initial_point).magnitude;
    }

    public override Network network {
        get {
            return neural_network;
        }

        set {
            neural_network = value;
        }
    }

    public override double fitness {
        get {
            return network_fitness;
        }

        set {
            network_fitness = value;
        }
    }

    public double LogFormat(double x) {
        return Math.Pow(2, 1 - x / Sensitivity) - 1; 
    }

}
