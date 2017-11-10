﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAmanager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        testGA();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void testGA()
    {
        //Debug.Log("Start the testGA");
        //create population
        GApopulation testPopulation = new GApopulation(10); //this will also create a random population of enemies;
        //Debug.Log(2);

        PrintFitness(testPopulation);
        //Debug.Log("3");

        testPopulation.nextGeneration();
        PrintFitness(testPopulation);


        testPopulation.nextGeneration();
        PrintFitness(testPopulation);



        //next generation
        //next generation
    }

    public void PrintFitness(GApopulation p)
    {
        for (int index = 0; index < p.getDictionary().Count; index++)
        {
            Debug.Log(index + " fitness: " + p.getDictionary()[index].getFitness());
            Debug.Log("");
        }
    }
}