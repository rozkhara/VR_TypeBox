using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager instance;

    public static ParticleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ParticleManager>();
            }

            return instance;
        }
    }

    public ParticleSystem[] particles; // 파티클들 지정하는 배열

    private readonly Dictionary<string, ParticleSystem> particlesDic = new Dictionary<string, ParticleSystem>(); // 파티클들을 string으로 관리할 수 있게 만든 딕셔너리

    private void Awake()
    {
        if (Instance != this) Destroy(this.gameObject); // 이미 ParticleManager가 있으면 이 ParticleManager 삭제

        DontDestroyOnLoad(this.gameObject); // 여러 씬에서 사용

        foreach (ParticleSystem particle in particles)
        {
            particlesDic.Add(particle.name, particle);
        }
    }

    public void PlayParticle(string name, Vector3 pos)
    {
        var particle = Instantiate(particlesDic[name]);

        particle.transform.position = pos;
        particle.Play();
    }
}