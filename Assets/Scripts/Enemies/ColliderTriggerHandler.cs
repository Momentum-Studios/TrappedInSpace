/**
 * file: ColliderTriggerHandler.cs
 * studio: Momentum Studios
 * authors: Justin Kim
 * class: CS 4700 - Game Development
 * 
 * assignment: Program 4
 * date last modified: 11/17/2022
 * 
 * purpose: This script acts as middle man between this gameobject and another
 * gameobject's script to check whether a certain gameobject with a certain name
 * is within the trigger collider
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTriggerHandler : MonoBehaviour
{
    [SerializeField] private string gameObjectName;

    public bool isInside { get; private set; }
    public GameObject gameObj { get; private set; }

    // if target gameobject enters trigger, then isInside = true
    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.name == gameObjectName)
        {
            isInside = true;
            gameObj = c.gameObject;
        }
    }

    // if target gameobject exits trigger, then isInside = false
    void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.name == gameObjectName)
        {
            isInside = false;
        }
    }
}
