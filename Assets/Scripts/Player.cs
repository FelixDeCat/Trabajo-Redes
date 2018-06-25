using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class Player : NetworkBehaviour
{

    Rigidbody rb;
    Renderer myRender;

    public bool canMove = true;

    public CheckIsGrounded check;

    public int index;

    bool canshot;

    [SyncVar]
    public string player_name;
    public TextMesh txt_name;

    public List<Player> allPlayers = new List<Player>();

    public Vector3 mySpawnPosition;

    Vector3 extraVector;

    float timer = 5;
    bool timerGo;

    [Header("Movimiento")]
    public float speed;
    float auxFloat;
    Vector3 movehorizontal;
    Vector3 movevetical;

    [Header("Salto")]
    public float gravity = 0.75f;
    public float jumpForce = 5f;
    Vector3 moveFall;
    float inputJump;

    [Header("Rotacion")]
    public float horizontalSpeed = 2.0F;

    [Header("Otros")]
    float y_pos_to_death = -20;

    [Header("Disparo")]
    [SerializeField]
    Transform bullet_spawn_point;
    int maxBullets = 3;
    int currentBullets;

    void Start()
    {
        //if (!hasAuthority) enabled = false;
        myRender = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        auxFloat = speed;

        if (!hasAuthority) return;

        GameManager.instancia.TheAutority = this;

        canshot = true;
        currentBullets = maxBullets;
        GameManager.instancia.Cuadro.SetActive(false);
        txt_name.fontSize = txt_name.fontSize / 2;
        GameManager.instancia.pantalla_de_espera.SetActive(true);
    }

    void FixedUpdate()
    {
        if (!hasAuthority) return;
        if (canMove)
        {
            Jump();
            Move();
            Dead();
            Look();
        }
    }
    #region Movement, Jump & look
    bool Grounded()
    {
        bool aux = check.isGrounded;
        // bool aux = Physics.Raycast(transform.position, Vector3.down, 0.5f);
        return aux;
    }
    public void Jump()
    {
        inputJump = Input.GetAxisRaw("Jump");
        if (inputJump > 0 && Grounded()) { moveFall.y = jumpForce; }
        else if (inputJump == 0 && Grounded()) { moveFall.y = 0;  if (oneshotJump) { oneshotJump = false;  moveFall.y = jumpForce * 3; } }
        else { moveFall.y -= gravity; }
    }
    public void Move()
    {
        movevetical = transform.forward * Input.GetAxis("Vertical") * speed;
        movehorizontal = transform.right * Input.GetAxis("Horizontal") * speed;
        Vector3 movement = movehorizontal + movevetical + moveFall + extraVector;
        rb.velocity = movement;
    }
    public void Look()
    {
        float h = horizontalSpeed * Input.GetAxis("Mouse X");
        transform.Rotate(0, h, 0);
    }
    #endregion

    bool oneshot2;
    private void Update()
    {
        if (isServer) RutinaTimer();
        if (!hasAuthority) return;
        Shoot();
        CmdCheckIfEstanTodos();
    }

    float timerBullet;
    public void Shoot()
    {
        if (!hasAuthority) return;

        canshot = currentBullets <= 0 ? false : true;

        if (!canshot) return;

        if (Input.GetButtonDown("Fire1"))
        {
            currentBullets--;
            
            CmdShoot();
            Invoke("AddBullet", 5f);
        }

        Debug.Log("BULLET: " + currentBullets);

        UpdateGraphicsBullets();
    }
    void AddBullet() { if (currentBullets < maxBullets) currentBullets++; }

    void UpdateGraphicsBullets()
    {
        var g1 = GameManager.instancia.bullet1.gameObject;
        var g2 = GameManager.instancia.bullet2.gameObject;
        var g3 = GameManager.instancia.bullet3.gameObject;

        if (currentBullets == 0) { g1.SetActive(false); g2.SetActive(false); g3.SetActive(false); }
        if (currentBullets == 1) { g1.SetActive(true); g2.SetActive(false); g3.SetActive(false); }
        if (currentBullets == 2) { g1.SetActive(true); g2.SetActive(true); g3.SetActive(false); }
        if (currentBullets == 3) { g1.SetActive(true); g2.SetActive(true); g3.SetActive(true); }

    }

    void RutinaTimer()
    {
        if (timerGo)
        {
            if (timer >= 0)
            {
                timer = timer - 1 * Time.deltaTime;
                CmdEnviarElTimerATodos(((int)timer).ToString());
            }
            else
            {
                FindObjectOfType<NetworkManager>().ServerChangeScene("Game scene");
                timerGo = false;
                timer = 0;
            }
        }
    }

    private void OnDestroy() { if(!hasAuthority)  if(GameManager.instancia.pantalla_de_espera != null) GameManager.instancia.pantalla_de_espera.SetActive(false); }

    void Respawn()
    {
        transform.position = mySpawnPosition; Console.WriteLine("Estoy spawneando");
        extraVector = Vector3.zero;
    }

    void Dead()
    {
        if (transform.position.y < y_pos_to_death) { Console.WriteLine("Spawneo por muerte"); Respawn(); }
    }

    void CanMove()
    {
        allPlayers = FindObjectsOfType<Player>().ToList();
        allPlayers.ForEach(x => x.RpcCanMove());
        for (int i = 0; i < NetworkServer.connections.Count; i++)
        {
            allPlayers[i].RpcSetInitialData(GameManager.instancia.spawnpoint[i].position, i, "Player " + (i + 1));
        }
    }

    //////////////////////////////////// 
    /// CMD 
    //////////////////////////////////// 

    [Command]
    public void CmdEmpuje(int index, Vector3 dir)
    {
        allPlayers = FindObjectsOfType<Player>().ToList();
        var player = allPlayers.Where(x => x.index == index).First();
        player.RpcEmpuje(dir);
    }

    [Command]
    public void CmdMensaje(string s)
    {
        RpcMensaje(s);
    }
    bool oneshot;
    [Command]
    void CmdCheckIfEstanTodos()
    {
        var count = NetworkServer.connections.Count;
        if (!oneshot)
        {
            oneshot = true;
            if (count >= 2)
            {
                allPlayers = FindObjectsOfType<Player>().ToList();
                allPlayers.ForEach(x => x.RpcPintarme(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))));
                Invoke("CanMove", 0.5f);
            }
        }

    }
    [Command]
    void CmdEnviarElTimerATodos(string s)
    {
        allPlayers = FindObjectsOfType<Player>().ToList();
        allPlayers.ForEach(x => x.RpcMostrarTimer(s));
    }
    [Command]
    void CmdShoot()
    {
        var bl = Instantiate(GameManager.instancia.bullet, bullet_spawn_point.position, transform.rotation);
        NetworkServer.Spawn(bl);
    }

    [Command]
    public void CmdLlegoALaMeta(int index)
    {
        Console.WriteLine(index + "llego a la Meta");
        allPlayers = FindObjectsOfType<Player>().ToList();
        allPlayers.ForEach(x => x.RpcMostrarGanador("El ganador es el \nPlayer " + index));
        timerGo = true;
    }

    [Command]
    public void CmdRealizarAccion(int p, int accion, float param)
    {
        Console.WriteLine("Recibo accion del cliente");
        allPlayers = FindObjectsOfType<Player>().ToList();
        var player = allPlayers.Where(x => x.index == p).First();
        player.RpcRealizarAccion(accion, param);
    }

    //////////////////////////////////// 
    /// RPC
    //////////////////////////////////// 

    [ClientRpc]
    void RpcPintarme(Vector3 v3)
    {
        GetComponent<MeshRenderer>().materials[0].color = new Color(v3.x, v3.y, v3.z);
    }

    bool oneshotJump;
    [ClientRpc]
    void RpcRealizarAccion(int index, float param)
    {
        if (!hasAuthority) return;

        Console.WriteLine("Realizando accion: (" + index + ")");

        Console.WriteLine("Primero vale :" + speed.ToString());

        switch (index)
        {
            case 1:
                speed = speed + param;
                Invoke("Reset_Buffs", 2.5f);
                break;
            case 2:
                speed = speed - param;
                Invoke("Reset_Buffs", 2.5f);
                break;
            case 3:
                canshot = false;
                var g1 = GameManager.instancia.bullet1.gameObject;
                var g2 = GameManager.instancia.bullet2.gameObject;
                var g3 = GameManager.instancia.bullet3.gameObject;
                g1.SetActive(false); g2.SetActive(false); g3.SetActive(false); 
                Invoke("Reset_Buffs", 2f);
                break;
            case 4:
                if (!oneshotJump) oneshotJump = true;
                break;
        }
    }

    void Reset_Buffs()
    {
        speed = auxFloat;
        canshot = true;
        oneshotJump = false;
    }

    [ClientRpc]
    public void RpcMostrarTimer(string s)
    {
        GameManager.instancia.Cuadro.SetActive(true);
        GameManager.instancia.txt_timer.text = s;
    }
    [ClientRpc]
    public void RpcEmpuje(Vector3 dir)
    {
        extraVector = dir * 5;
        Invoke("ResetExtraVector", 2f);
    }
    void ResetExtraVector() { extraVector = Vector3.zero; }
    [ClientRpc]
    public void RpcCanMove()
    {
        GameManager.instancia.pantalla_de_espera.SetActive(false);
        canMove = true;
    }
    [ClientRpc]//Lo recibe el cliente desde el server
    public void RpcMensaje(string s) { Console.WriteLine(s); }
    [ClientRpc]
    public void RpcSetInitialData(Vector3 t, int i, string s)
    {
        Console.WriteLine("mi data es: " + t + " Index: " + i);
        index = i;
        player_name = s;
        txt_name.text = player_name;
        mySpawnPosition = t;
        Respawn();
    }
    [ClientRpc]
    public void RpcMostrarGanador(string s)
    {
        GameManager.instancia.anim_mensaje.Animar(s);
    }
}


public static class extensions
{
    public static UnityEngine.UI.Graphic pintar(this UnityEngine.UI.Graphic g) { g.color = Color.white; return g; }
    public static UnityEngine.UI.Graphic despintar(this UnityEngine.UI.Graphic g) { g.color = Color.grey; return g; }
}