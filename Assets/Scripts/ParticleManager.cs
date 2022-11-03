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

    public ParticleSystem[] particles; // ��ƼŬ�� �����ϴ� �迭

    private readonly Dictionary<string, ParticleSystem> particlesDic = new Dictionary<string, ParticleSystem>(); // ��ƼŬ���� string���� ������ �� �ְ� ���� ��ųʸ�

    private void Awake()
    {
        if (Instance != this) Destroy(this.gameObject); // �̹� ParticleManager�� ������ �� ParticleManager ����

        DontDestroyOnLoad(this.gameObject); // ���� ������ ���

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