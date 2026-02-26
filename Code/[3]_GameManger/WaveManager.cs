using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManger
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }
        
        [Header("Current Status")]
        public int waveNumber = 1;
        public int remainingMonsters = 0;
        
        [Header("Wave Settings")]
        public List<GameObject> monsterTypes;
        public GameObject bossMonster; 
        public List<Transform> spawnPoints;
        public int baseMonsterCount = 3;
        public float spawnDelay = 2f;
        public int bossWave = 5;
        
        [HideInInspector]
        private bool gameRunning = true; 

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
        void Start()
        {
            if (gameRunning)
            {
                StartCoroutine(WaveLoopRoutine());
            }
        }
        
        IEnumerator WaveLoopRoutine()
        {
            while (gameRunning)
            {
                if (UIManager.Instance != null)
                {
                    yield return StartCoroutine(UIManager.Instance.DisplayWaveNotificationRoutine(waveNumber));
                }

                if (waveNumber % bossWave == 0)
                {
                    yield return StartCoroutine(SpawnBossWave());
                }
                else
                {
                    yield return StartCoroutine(SpawnNormalWave());
                }
                
                while (remainingMonsters > 0)
                {
                    yield return new WaitForSeconds(1f);
                }
                
                waveNumber++;
            }
        }
        IEnumerator SpawnNormalWave()
        {
            int monsterCount = baseMonsterCount + (waveNumber - 1) * 2;
            remainingMonsters = monsterCount; 

            for (int i = 0; i < monsterCount; i++)
            {
                yield return new WaitForSeconds(spawnDelay);
                
                GameObject selectedMonsterPrefab = monsterTypes[Random.Range(0, monsterTypes.Count)];
                
                Transform spawnLocation = spawnPoints[Random.Range(0, spawnPoints.Count)];
                
                GameObject newMonster = Instantiate(selectedMonsterPrefab, spawnLocation.position, Quaternion.identity);
                
            }
        }
        IEnumerator SpawnBossWave()
        {
            int monsterCount = 1;
            remainingMonsters = monsterCount; 
            
            Transform spawnLocation = spawnPoints[Random.Range(0, spawnPoints.Count)];
            
            GameObject newBoss = Instantiate(bossMonster, spawnLocation.position, Quaternion.identity);

            yield return null; 
        }
        public void MonsterDied()
        {
            remainingMonsters--;
        }
    }
}