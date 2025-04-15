using Fusion;
using UnityEngine;

public class FireBall : NetworkBehaviour
{
    [Networked] public PlayerProperties Owner { get; set; }

    private Vector3 shootDirection;
    [SerializeField] private int Damage = 30;
    [SerializeField] private int Speed = 20;
    [SerializeField] private int score = 10;
    [Networked] private TickTimer _tickTimer { get; set; }

    public void Setup(Vector3 direction)
    {
        _tickTimer = TickTimer.CreateFromSeconds(Runner, 1.5f);
        shootDirection = direction;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //var targetPlayer = other.gameObject.GetComponent<NetworkObject>().InputAuthority;
            //RpcFireBall(targetPlayer, 10);
            var targetPlayer = other.gameObject.GetComponent<PlayerProperties>();
            //var _player = gameObject.GetComponent<PlayerProperties>();
            if (targetPlayer != null)
            {
                targetPlayer.TakeDamage(Damage);
                Debug.Log("Nhan dame");
                Owner.IncreaseScore(score);
            }

        }
    }

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //public void RpcFireBall(PlayerRef player, int Damage)
    //{
    //    if (Runner.GetPlayerObject(player) == null) return;
    //    Runner.GetPlayerObject(player).GetComponent<PlayerProperties>().TakeDamage(Damage);
    //}

    public override void FixedUpdateNetwork()
    {
        if (_tickTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        transform.position += shootDirection * Speed * Time.fixedDeltaTime;
    }
}
