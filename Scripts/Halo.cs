using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Halo : MonoBehaviour
{

    private ParticleSystem particleSys;  // 粒子系统
    private ParticleSystem.Particle[] particleArr;  // 粒子数组  
    private ParticleInfo[] particles; // 粒子位置数组
    public Gradient colorGradient; // 粒子颜色改变器
    public int count = 10000;       // 粒子数量  
    public float size = 0.03f;      // 粒子大小  
    public float minRadius = 5.0f;  // 最小半径  
    public float maxRadius = 12.0f; // 最大半径  
    public bool clockwise = true;   // 顺时针|逆时针  
    public float speed = 2f;        // 速度  
    public float pingPong = 0.02f;  // 游离范围

    
    public class ParticleInfo
    {
        public float radius = 0f, angle = 0f, time = 0f;
        public ParticleInfo(float x, float y, float z)
        {
            radius = x;
            angle = y;
            time = z;
        }
    }

    void RandomlySpread()
    {
        for (int i = 0; i < count; i ++)
        {
            // 随机粒子距离中心的半径
            float midR = (maxRadius + minRadius) / 2;
            float minRate = Random.Range(1, midR / minRadius);
            float maxRate = Random.Range(midR / maxRadius, 1);
            float R = Random.Range(minRadius * minRate, maxRadius * maxRate);

            // 随机每个粒子的角度  
            float angle = Random.Range(0, 360);
            float theta = angle / 180 * Mathf.PI;

            // 随机每个粒子的游离起始时间  
            float time = Random.Range(0.0f, 360.0f);
            particles[i] = new ParticleInfo(R, angle, time);
            particleArr[i].position = new Vector3(particles[i].radius * Mathf.Cos(theta), 0f, particles[i].radius * Mathf.Sin(theta));
        }
        particleSys.SetParticles(particleArr, particleArr.Length);
    }

    // Start is called before the first frame update
    void Start()
    {
        //initialize
        particleArr = new ParticleSystem.Particle[count];
        particles = new ParticleInfo[count];

        // 初始化粒子系统
        particleSys = this.GetComponent<ParticleSystem>();
        particleSys.startSpeed = 0;            // 粒子位置由程序控制
        particleSys.startSize = size;          // 设置粒子大小
        particleSys.loop = false;
        particleSys.maxParticles = count;      // 设置最大粒子量
        particleSys.Emit(count);               // 发射粒子
        particleSys.GetParticles(particleArr);

        RandomlySpread();   // 初始化各粒子位置

    }

    // Update is called once per frame
    private int tier = 10;  
    void Update()
    {
        for (int i = 0; i < count; i++)
        {
            if (clockwise) 
                particles[i].angle -= (i % tier + 1) * (speed / particles[i].radius / tier);
            else          
                particles[i].angle += (i % tier + 1) * (speed / particles[i].radius / tier);

            particles[i].angle = (360.0f + particles[i].angle) % 360.0f;
            float theta = particles[i].angle / 180 * Mathf.PI;
            float x = 10 * Mathf.Sin(8 * theta) * Mathf.Cos(theta);
            float y = 10 * Mathf.Sin(8 * theta) * Mathf.Sin(theta);
            particleArr[i].position = new Vector3(x, 0f, y + Random.Range(-1f, 1f));

            // 粒子在半径方向上游离  
            particles[i].time += Time.deltaTime;
            particles[i].radius += Mathf.PingPong(particles[i].time / minRadius / maxRadius, pingPong) - pingPong / 2.0f;

            //改变粒子透明度
            particleArr[i].color = colorGradient.Evaluate(Random.value);

        }

        particleSys.SetParticles(particleArr, particleArr.Length);
    }
}
