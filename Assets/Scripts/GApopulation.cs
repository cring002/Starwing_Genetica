﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GApopulation {

    public IDictionary<int, GAenemy> population;// = new IDictionary<int, GAenemy>();
    public int populationSize;
    public System.Random r = new System.Random();

 

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //constructor:creates the dict
    public GApopulation(List<GameObject> enemyClones)
    {
        this.populationSize = enemyClones.Count;
        this.population = new Dictionary<int, GAenemy>();
        createPopulation(enemyClones);
    }


    //creates population
    public void createPopulation(List<GameObject> enemyClones)
    {
        //float[] gene = new float[7]; //there are currently 7 chromosones
        //randomly 0-9 add values to gene array

        /*for each gameobj in the enemyClones list get the info needed to create the array.
        this array needs to be passed then in the creation of the GAenemy obj that consequently creates the GApopulation by 
        adding it to the Idictionary*/

        /*values in the gene array:
        0: health, 1:speed, 2:bulletSpeed (& bulletDamage) 
        3:playerSeekDistance, 4:playerFleeDistance, 5:enemiesAvoidDistance //not anymore: 5:playerFleeBuffer, 6:bulletFleeDistance */

        int currentPopulationIndex = 0;
        foreach (GameObject enemyClone in enemyClones)
        {
            float[] gene = new float[6];
            gene[0] = enemyClone.GetComponent<EnemyBrain>().GetGene()[0]; 
            gene[1] = enemyClone.GetComponent<EnemyBrain>().GetGene()[1];
            gene[2] = enemyClone.GetComponent<EnemyBrain>().GetGene()[2]; 

            gene[3] = enemyClone.GetComponent<EnemyBrain>().GetGene()[3];
            gene[4] = enemyClone.GetComponent<EnemyBrain>().GetGene()[4];
            gene[5] = enemyClone.GetComponent<EnemyBrain>().GetGene()[5];

            GAenemy e = new GAenemy(gene,0, enemyClone.GetComponent<EnemyBrain>().damageDealt);
            this.population[currentPopulationIndex] = e;

            currentPopulationIndex++;
        }

        /*Previous stuff
         * for (int geneIndex=0;geneIndex< gene.Length; geneIndex++)
        {
            gene[geneIndex] = randomValues();
        }

        for (int index = 0; index < this.populationSize; index++)
        {
            GAenemy e = new GAenemy(gene);
            this.population[index] = e;

        }*/
    }

    //selection: tournament selection with prob of 0.7
    public void selection()
    {
        int selectionPropbability = 70;
        int initialPopulationSize = this.populationSize;
        //IDictionary <int, GAenemy> selectedPopulation = this.population;
        IDictionary<int, GAenemy> selectedPopulation = new Dictionary<int, GAenemy>();

        int newPopulationIndex = 0;

        //while (selectedPopulation.Count < initialPopulationSize)//uncomment when only the selection is selected in nextGeneration

        while (selectedPopulation.Count < initialPopulationSize / 2)
        {
            int enemyOneIndex = r.Next(0, this.population.Count);
            int enemyTwoIndex = r.Next(0, this.population.Count);

            GAenemy e1 = new GAenemy(this.population[enemyOneIndex].GetGene(),
                this.population[enemyOneIndex].getLifeSpam(),
                this.population[enemyOneIndex].getPlayerDamage());

            GAenemy e2 = new GAenemy(this.population[enemyTwoIndex].GetGene(),
                this.population[enemyTwoIndex].getLifeSpam(),
                this.population[enemyTwoIndex].getPlayerDamage());

            if (r.Next(0, 100) > selectionPropbability)
            {   if (e1.getFitness() > e2.getFitness())
                {
                    selectedPopulation[newPopulationIndex] = e2;
                }else
                {
                    selectedPopulation[newPopulationIndex] = e1;
                }

            }
            else
            {
                if (e1.getFitness() > e2.getFitness())
                {
                    selectedPopulation[newPopulationIndex] = e1;
                }
                else
                {
                    selectedPopulation[newPopulationIndex] = e2;
                }

            }

            newPopulationIndex++;
        }

        //clear all pop from the initial dict
        this.population.Clear();

        //add all selected pop to the init dict
        for (int i = 0; i < selectedPopulation.Count; i++)
        {
            this.population[i] = selectedPopulation[i];
        }
   
    }

    //crossover (returns an offspring)
    public void crossOver()
    {
        IDictionary<int, GAenemy> offspringPopulation=new Dictionary<int, GAenemy>();
        int newPopulationSize = 0; //= this.population.Count * 2; //two times the original population. The parents are added to the population

        //while (this.population.Count  != newPopulationSize) //uncomment when only the mutation is selected in nextGeneration
        while (this.population.Count*2 != newPopulationSize)
        {
            //randomly pick 2 individuals from pop and crossover them
            GAenemy e1 = this.population[r.Next(0, this.population.Count)];
            GAenemy e2 = this.population[r.Next(0, this.population.Count)];

            //for each gene in the chromozone (NOTE: ADD all the genes in an array for easier acces) 0,2
            GAenemy newOffspring = newChild(e1, e2);
            offspringPopulation[newPopulationSize] = newOffspring;
            newPopulationSize++;
        }

        //clear all pop from the initial dict
        this.population.Clear();

        //add the offspring to the whole population
        //int initialPopulationSize = this.population.Count;
        for (int i =0; i < offspringPopulation.Count; i++)
        {
            this.population[i] = offspringPopulation[i]; 
        }
    }

    //mutation (returns a offspring)
    public void mutation()
    {   //create a new population to mutate
        IDictionary<int, GAenemy> mutatedPopulation = new Dictionary<int, GAenemy>();
        float[] mutatedGene = new float[6];


        //for all the genes in the chormosome, add a gausian.
        for (int index =0; index < this.population.Count; index++)
        {
            mutatedPopulation[index] = new GAenemy(mutatedGene,0,0);//the lifespam and damage dealt is set  to 0 as there won't be any fitness comparison and these values are going to be updated in-game

            mutatedPopulation[index].setHealth(this.population[index].getHealth() + Gaussian(0, 0.8));//gaussian is called with mean and sddev (sddev:0.7 to 1.5)
            mutatedPopulation[index].setSpeed(this.population[index].getSpeed() + Gaussian(0, 0.8));
            mutatedPopulation[index].setBulletSpeed(this.population[index].getBulletSpeed() + Gaussian(0, 0.8));

            
            mutatedPopulation[index].setPlayerFleeDistance(this.population[index].getPlayerFleeDistance() + Gaussian(0, 0.8));
            mutatedPopulation[index].setPlayerSeekDistance(this.population[index].getPlayerSeekDistance() + Gaussian(0, 0.8));
            mutatedPopulation[index].setEnemiesAvoidDistance(this.population[index].getEnemiesAvoidDistance() + Gaussian(0, 0.8));



            //this is for TEST ONLY, will be adjusted in game with game variables
            //the lifespam and playerdamage have to change as they are part of the fitness function
            //mutatedPopulation[index].ChangeLifeSpamRand();
            //mutatedPopulation[index].ChangePlayerDamageRand();

        }

        //clear all pop from the initial dict
        this.population.Clear();

        //add the offspring to the whole population
        //int initialPopulationSize = this.population.Count;
        for (int i = 0; i < mutatedPopulation.Count; i++)
        {
            this.population[i] = mutatedPopulation[i];
        }

    }

    public IDictionary<int, GAenemy> getDictionary() { return this.population; }
    

    //next generation
    public void generateNextGeneration()
    {
        //after the population is created, do selection, crossover, mutation 
        selection();
        crossOver();
        mutation();

    } 

    //OTHER
    //temp function for rand values
    private int randomValues()
    {
        return r.Next(0, 10);
    }

    GAenemy newChild(GAenemy e1, GAenemy e2)
    {
        int geneLen = e1.GetGene().Length; //store the lenght of a gene
        float[] newGene = new float[geneLen]; //create another array to store the new gene values

        //iterate and randomly decide which parent pass on its gene
        for (int geneIndex=0; geneIndex<e1.GetGene().Length; geneIndex++)
        {
            if (r.Next(0, 2) == 0)
            {
                newGene[geneIndex] = e1.GetGene()[geneIndex];
            }
            else
            {
                newGene[geneIndex] = e2.GetGene()[geneIndex];

            }
        }

        //create and return the child
        GAenemy newChild = new GAenemy(newGene, 0, 0);
        return newChild;
    }

    public static float Gaussian( double mean, double stddev)
    {
        // The method requires sampling from a uniform random of (0,1]
        // but Random.NextDouble() returns a sample of [0,1).
        System.Random r = new System.Random();
        double x1 = 1 - r.NextDouble();
        double x2 = 1 - r.NextDouble();

        double y1 = System.Math.Sqrt(-2.0 * System.Math.Log(x1)) * System.Math.Cos(2.0 * System.Math.PI * x2);
        return (float) (y1 * stddev + mean);
    }

    
}
