using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSelectedCharacter : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private Transform spawnPoint;


    private void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        GameObject characterPrefab = characterPrefabs[selectedCharacter];
        GameObject characterSpawned = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);
    }
}
