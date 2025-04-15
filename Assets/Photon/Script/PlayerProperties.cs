using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    private PlayerInfo _playerInfo { get; set; }

    [SerializeField] Image healthSlider;
    [SerializeField] Image manaSlider;

    [SerializeField] GameObject fireBallPrefab;
    [SerializeField] Transform fireBallSpawnPoint;
    private void OnHealthChanged()
    {
        healthSlider.fillAmount = _playerInfo.health / 100f;
        manaSlider.fillAmount = _playerInfo.mana / 60f;

        if (_playerInfo.health <= 0)
        {
            Debug.Log($"Player {Runner.LocalPlayer.PlayerId} is dead");
            Die();
        }
    }

    void Start()
    {
        if (HasStateAuthority)
        {
            _playerInfo = new PlayerInfo
            {
                health = 100,
                mana = 60
            };
        }
    }

    void Update()
    {
        if (HasStateAuthority)
        {
            if (Input.GetKeyDown(KeyCode.E) && _playerInfo.mana > 0)
            {
                Runner.Spawn(fireBallPrefab, fireBallSpawnPoint.position,
                    fireBallSpawnPoint.rotation, Runner.LocalPlayer, (runner, obj) =>
                    {
                        var fireBall = obj.GetComponent<FireBall>();
                        fireBall.Setup(fireBallSpawnPoint.forward.normalized);
                    });

                var currentHealth = _playerInfo.health;
                var currentMana = _playerInfo.mana;
                _playerInfo = new PlayerInfo
                {
                    health = currentHealth,
                    mana = currentMana - 10
                };
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (HasStateAuthority)
        {
            var currentHealth = _playerInfo.health;
            var currentMana = _playerInfo.mana;
            _playerInfo = new PlayerInfo
            {
                health = currentHealth - damage,
                mana = currentMana
            };
        }
    }

    private void Die()
    {
        _playerInfo = new PlayerInfo
        {
            health = 0,
            mana = 0
        };
        GetComponent<Animator>().SetBool("IsDead", true);
        GetComponent<CharacterController>().enabled = false;
    }

    public void ManaPoolFill()
    {
        if (HasStateAuthority)
        {
            var currentHealth = _playerInfo.health;
            var currentMana = _playerInfo.mana;
            _playerInfo = new PlayerInfo
            {
                health = currentHealth,
                mana = currentMana + 10
            };
        }
    }
}