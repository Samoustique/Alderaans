using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Mob : MonoBehaviour {

    public GameObject particle;
    public AudioClip sound;
    public int life;
    public int reward;
    public Texture lifeRemainingBehindTexture, lifeRemainingTexture;
    public Text txtReward;

    private NavMeshAgent myAgent;
    private float explosionLifeTime = 1.5F;
    private Camera mainCamera;
    private float lifeRatio, lifeWidth, lifeHeight, lifeBackgroundWidth;
    private int currentLife;
    private Vector2 vector;

    void Start () {
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.destination = GameObject.Find("home").transform.position;
        mainCamera = Camera.main;

        // life
        lifeBackgroundWidth = 50.0F;
        lifeHeight = 6.0F;
        currentLife = life;
        //lifeRemainingBehindTexture = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Textures/lifeBackground.jpg", typeof(Texture)) as Texture;
        //lifeRemainingTexture = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Textures/life.jpg", typeof(Texture)) as Texture;
    }
	
	void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "home"){
            GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(sound);
            GameManager.life--;
            DestroyObject();
        }
        else if (col.gameObject.tag == "Bullet")
        {
            Bullet bullet = col.gameObject.GetComponent("Bullet") as Bullet;
            currentLife -= bullet.damage;
        }
    }

    void Update()
    {
        // mob life
        if (currentLife > 0)
        {
            lifeRatio = (float) currentLife / life;
            lifeWidth = lifeRatio * lifeBackgroundWidth;
        }
        else
        {
            // mob is dead
            GameManager.gold += reward;
            txtReward.text = "+" + reward;
            GameManager.ShowUpReward(transform.position, txtReward.gameObject);

            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        Object explosion = Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(explosion, explosionLifeTime);
    }

    void OnDestroy()
    {
        Spawn.notifyMobIsDestroyed(this.gameObject);
    }

    void OnGUI()
    {
        if(lifeRatio > 0.0F && lifeRatio < 1.0F)
        {
            vector = mainCamera.WorldToScreenPoint(transform.position);
            Rect rect = new Rect(vector.x - (lifeBackgroundWidth / 2.0F), Screen.height - (vector.y + 30.0F), lifeBackgroundWidth, lifeHeight);
            Rect rect2 = new Rect(vector.x - (lifeBackgroundWidth / 2.0F), Screen.height - (vector.y + 30.0F), lifeWidth, lifeHeight);
            GUI.DrawTexture(rect, lifeRemainingBehindTexture, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(rect2, lifeRemainingTexture, ScaleMode.ScaleAndCrop);
        }
    }
}
